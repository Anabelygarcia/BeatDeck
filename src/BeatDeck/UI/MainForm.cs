using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using BeatDeck.Audio;
using BeatDeck.DataStructures;
using BeatDeck.Models;

namespace BeatDeck.UI
{
    /// <summary>
    /// Formulario principal de la aplicación BeatDeck.
    /// Gestiona la interfaz gráfica, la sincronización entre estructuras de datos
    /// para la cola de reproducción, la interacción con el reproductor de audio y los benchmarks.
    /// </summary>
    public partial class MainForm : Form
    {
        // Define las tres estructuras de datos soportadas para la cola.
        private enum QueueMode { Custom, NetLinkedList, NetList }

        // =========================================================================
        // 1. GESTIÓN DE COLAS Y ESTRUCTURAS DE DATOS PARALELAS
        // Mantiene tres colecciones paralelas. Los RadioButtons determinan sobre cuál se opera.
        // =========================================================================
        private readonly SinglyLinkedList<Track> _customQueue = new(); // Lista simplemente enlazada propia
        private readonly LinkedList<Track> _linkedQueue = new();       // Lista doblemente enlazada nativa de .NET
        private readonly List<Track> _listQueue = new();               // Arreglo dinámico estándar de .NET

        // =========================================================================
        // 2. REPRODUCTOR DE AUDIO Y ESTADOS INTERNOS
        // Controla la reproducción local sin depender de reproductores externos.
        // =========================================================================
        private readonly AudioPlayer _player = new();
        private bool _seeking;

        private int _nextId = 1;
        private bool _benchmarkRunning;

        private const string PlayText = "Reproducir";
        private const string PauseText = "Pausar";
        private const string UnknownArtist = "Artista desconocido";

        // Propiedad que evalúa qué modo/estructura de datos está seleccionada en la UI.
        private QueueMode ActiveMode =>
            rbCustom.Checked ? QueueMode.Custom :
            rbLinkedList.Checked ? QueueMode.NetLinkedList :
            QueueMode.NetList;

        public MainForm()
        {
            InitializeComponent();
            EnableFileDrop();
            ConfigureGrid();
            SeedData();
            RefreshView();

            _player.VolumePercent = tbVolume.Value;

            // Liberación de recursos al cerrar el formulario
            FormClosing += (_, _) =>
            {
                playbackTimer.Stop();
                _player.Dispose();
                ToneGenerator.CleanUp();
            };
        }

        // ------------------------------------------------------------------
        // Configuración inicial de la interfaz y datos demo
        // ------------------------------------------------------------------
        private void ConfigureGrid()
        {
            dgvQueue.AllowUserToAddRows = false;
            dgvQueue.AllowUserToDeleteRows = false;
            dgvQueue.ReadOnly = true;
            dgvQueue.RowHeadersVisible = false;
            dgvQueue.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvQueue.MultiSelect = false;

            dgvQueue.Columns.Add("colPos", "Pos");
            dgvQueue.Columns.Add("colId", "ID");
            dgvQueue.Columns.Add("colTrack", "Título / Artista");
            dgvQueue.Columns.Add("colBpm", "BPM");
            dgvQueue.Columns.Add("colDuration", "Duración");
            dgvQueue.Columns.Add("colAudio", "Audio");
            dgvQueue.Columns.Add("colMemory", "Memoria / Enlace");

            dgvQueue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvQueue.Columns["colPos"]!.FillWeight = 7;
            dgvQueue.Columns["colId"]!.FillWeight = 7;
            dgvQueue.Columns["colTrack"]!.FillWeight = 28;
            dgvQueue.Columns["colBpm"]!.FillWeight = 10;
            dgvQueue.Columns["colDuration"]!.FillWeight = 10;
            dgvQueue.Columns["colAudio"]!.FillWeight = 16;
            dgvQueue.Columns["colMemory"]!.FillWeight = 22;
        }

        private void SeedData()
        {
            var demo = new[]
            {
                new Track(_nextId++, "Strobe", "deadmau5", 128, 634),
                new Track(_nextId++, "Midnight City", "M83", 105, 243),
                new Track(_nextId++, "Animals", "Martin Garrix", 130, 304)
            };

            // Puebla las tres estructuras por igual con las canciones iniciales.
            foreach (var track in demo)
            {
                _customQueue.AddLast(track);
                _linkedQueue.AddLast(track);
                _listQueue.Add(track);
            }
        }

        // ------------------------------------------------------------------
        // Lectura y validación de entradas de usuario
        // ------------------------------------------------------------------
        private bool TryReadTrack(out Track track)
        {
            track = null!;
            string title = txtTitle.Text.Trim();
            string artist = txtArtist.Text.Trim();

            if (title.Length == 0)
            {
                ShowWarning("Escribe el título de la canción.", txtTitle);
                return false;
            }
            if (artist.Length == 0)
            {
                ShowWarning("Escribe el nombre del artista.", txtArtist);
                return false;
            }

            string? audioPath = string.IsNullOrWhiteSpace(txtAudioPath.Text) ? null : txtAudioPath.Text.Trim();
            if (audioPath != null && !File.Exists(audioPath))
            {
                ShowWarning("El archivo de audio seleccionado ya no existe. Elige otro archivo o deja el campo vacío.", btnBrowse);
                return false;
            }

            track = new Track(_nextId++, title, artist, (int)numBpm.Value, (int)numDuration.Value, audioPath);
            return true;
        }

        private static void ShowWarning(string message, Control controlToFocus)
        {
            MessageBox.Show(message, "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            controlToFocus.Focus();
        }

        private void ClearInputs()
        {
            txtTitle.Clear();
            txtArtist.Clear();
            txtAudioPath.Clear();
            txtTitle.Focus();
        }

        // =========================================================================
        // 3. OPERACIONES SOBRE LAS COLAS (Queue Actions)
        // Ejecutan modificaciones (agregar, avanzar, invertir, ordenar y purgar)
        // según la estructura seleccionada.
        // =========================================================================
        private void btnEnqueue_Click(object? sender, EventArgs e)
        {
            if (!TryReadTrack(out Track track)) return;

            AddToActiveQueue(track);

            ClearInputs();
            RefreshView();
        }

        /// <summary>Inserta un elemento al final de la cola activa.</summary>
        private void AddToActiveQueue(Track track)
        {
            switch (ActiveMode)
            {
                case QueueMode.Custom: _customQueue.AddLast(track); break;
                case QueueMode.NetLinkedList: _linkedQueue.AddLast(track); break;
                default: _listQueue.Add(track); break;
            }
        }

        /// <summary>Inserta una canción para ser reproducida a continuación (posición 1).</summary>
        private void btnPlayNext_Click(object? sender, EventArgs e)
        {
            if (!TryReadTrack(out Track track)) return;

            switch (ActiveMode)
            {
                case QueueMode.Custom:
                    _customQueue.PlayNext(track);
                    break;
                case QueueMode.NetLinkedList:
                    if (_linkedQueue.First is null) _linkedQueue.AddFirst(track);
                    else _linkedQueue.AddAfter(_linkedQueue.First, track);
                    break;
                default:
                    _listQueue.Insert(Math.Min(1, _listQueue.Count), track);
                    break;
            }

            ClearInputs();
            RefreshView();
        }

        private void btnAdvance_Click(object? sender, EventArgs e) => AdvanceQueue(notifyIfEmpty: true);

        /// <summary>Extrae el primer elemento de la cola activa y lo envía al reproductor.</summary>
        private bool AdvanceQueue(bool notifyIfEmpty)
        {
            Track? playing = null;

            switch (ActiveMode)
            {
                case QueueMode.Custom:
                    if (!_customQueue.IsEmpty) playing = _customQueue.Advance();
                    break;
                case QueueMode.NetLinkedList:
                    if (_linkedQueue.First is { } first)
                    {
                        playing = first.Value;
                        _linkedQueue.RemoveFirst();
                    }
                    break;
                default:
                    if (_listQueue.Count > 0)
                    {
                        playing = _listQueue[0];
                        _listQueue.RemoveAt(0);
                    }
                    break;
            }

            if (playing is null)
            {
                if (notifyIfEmpty)
                {
                    MessageBox.Show("No hay canciones pendientes en la cola.", "Fin del set",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return false;
            }

            lblNowPlaying.Text = $"▶ Sonando: {playing.Title} - {playing.Artist} ({playing.Bpm} BPM)";
            lblNowPlaying.ForeColor = Color.DarkGreen;
            RefreshView();
            StartPlayback(playing);
            return true;
        }

        /// <summary>Invierte el orden de la cola activa.</summary>
        private void btnReverse_Click(object? sender, EventArgs e)
        {
            switch (ActiveMode)
            {
                case QueueMode.Custom:
                    _customQueue.Reverse();
                    break;

                case QueueMode.NetLinkedList:
                    // Inversión manual de nodos para LinkedList<T> de .NET (al no poseer Reverse() nativo).
                    if (_linkedQueue.First is { } anchor)
                    {
                        while (anchor.Next is { } moved)
                        {
                            _linkedQueue.Remove(moved);
                            _linkedQueue.AddFirst(moved);
                        }
                    }
                    break;

                default:
                    _listQueue.Reverse();
                    break;
            }

            RefreshView();
        }

        /// <summary>Ordena la cola activa por BPM utilizando algoritmos estables o LINQ.</summary>
        private void btnSortBpm_Click(object? sender, EventArgs e)
        {
            switch (ActiveMode)
            {
                case QueueMode.Custom:
                    _customQueue.SortInPlace((a, b) => a.Bpm.CompareTo(b.Bpm));
                    break;

                case QueueMode.NetLinkedList:
                    {
                        var sorted = _linkedQueue.OrderBy(t => t.Bpm).ToList();
                        _linkedQueue.Clear();
                        foreach (var t in sorted) _linkedQueue.AddLast(t);
                        break;
                    }

                default:
                    {
                        var sorted = _listQueue.OrderBy(t => t.Bpm).ToList();
                        _listQueue.Clear();
                        _listQueue.AddRange(sorted);
                        break;
                    }
            }

            RefreshView();
        }

        /// <summary>Elimina canciones duplicadas comparando sus títulos.</summary>
        private void btnPurge_Click(object? sender, EventArgs e)
        {
            int removed;

            switch (ActiveMode)
            {
                case QueueMode.Custom:
                    removed = _customQueue.RemoveDuplicates(
                        (a, b) => string.Equals(a.Title, b.Title, StringComparison.OrdinalIgnoreCase));
                    break;

                case QueueMode.NetLinkedList:
                    {
                        var unique = _linkedQueue.DistinctBy(t => t.Title, StringComparer.OrdinalIgnoreCase).ToList();
                        removed = _linkedQueue.Count - unique.Count;
                        _linkedQueue.Clear();
                        foreach (var t in unique) _linkedQueue.AddLast(t);
                        break;
                    }

                default:
                    {
                        var unique = _listQueue.DistinctBy(t => t.Title, StringComparer.OrdinalIgnoreCase).ToList();
                        removed = _listQueue.Count - unique.Count;
                        _listQueue.Clear();
                        _listQueue.AddRange(unique);
                        break;
                    }
            }

            RefreshView();

            if (removed == 0)
            {
                MessageBox.Show("No se encontraron títulos duplicados.", "Eliminar duplicados",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ------------------------------------------------------------------
        // Renderizado e inspección de memoria en el DataGridView
        // ------------------------------------------------------------------
        private void rbMode_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is RadioButton { Checked: true }) RefreshView();
        }

        /// <summary>Actualiza la tabla con los elementos de la estructura activa y muestra la referencia de memoria/enlace.</summary>
        private void RefreshView()
        {
            dgvQueue.Rows.Clear();
            int position = 1;
            int totalSeconds = 0;

            switch (ActiveMode)
            {
                case QueueMode.Custom:
                    for (Node<Track>? node = _customQueue.Head; node != null; node = node.Next)
                    {
                        string next = node.Next is null ? "null" : NodeTag(node.Next);
                        AddRow(position++, node.Value, $"Nodo {NodeTag(node)} -> {next}");
                        totalSeconds += node.Value.DurationSeconds;
                    }
                    break;

                case QueueMode.NetLinkedList:
                    foreach (var track in _linkedQueue)
                    {
                        AddRow(position++, track, "LinkedListNode (ant./sig.)");
                        totalSeconds += track.DurationSeconds;
                    }
                    break;

                default:
                    foreach (var track in _listQueue)
                    {
                        AddRow(position, track, $"Posición del arreglo [{position - 1}]");
                        position++;
                        totalSeconds += track.DurationSeconds;
                    }
                    break;
            }

            lblStats.Text = $"Canciones en cola: {position - 1}  |  Duración total: {FormatDuration(totalSeconds)}";
        }

        private void AddRow(int position, Track track, string memoryInfo)
        {
            string audio = track.FilePath is null ? "Beat sintético" : Path.GetFileName(track.FilePath);
            dgvQueue.Rows.Add(position, track.Id, $"{track.Title} — {track.Artist}",
                $"{track.Bpm} BPM", FormatDuration(track.DurationSeconds), audio, memoryInfo);
        }

        // Genera una etiqueta hexadecimal estacional basada en el HashCode para simular direcciones de memoria.
        private static string NodeTag(object node) => $"0x{RuntimeHelpers.GetHashCode(node):X8}";

        private static string FormatDuration(int seconds)
        {
            var t = TimeSpan.FromSeconds(seconds);
            return t.TotalHours >= 1
                ? $"{(int)t.TotalHours}:{t.Minutes:D2}:{t.Seconds:D2}"
                : $"{t.Minutes}:{t.Seconds:D2}";
        }

        // =========================================================================
        // 4. IMPORTACIÓN DE ARCHIVOS (Dialogs, carpetas y Drag & Drop)
        // =========================================================================
        private static string BuildAudioFilter() =>
            $"Archivos de audio|{AudioFormats.Patterns}|Todos los archivos (*.*)|*.*";

        private void btnImportFiles_Click(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Title = "Importar canciones",
                Multiselect = true,
                Filter = BuildAudioFilter()
            };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            ImportPaths(dialog.FileNames);
        }

        private void btnImportFolder_Click(object? sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Elige una carpeta con música (se incluyen sus subcarpetas).",
                UseDescriptionForTitle = true
            };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            ImportPaths(new[] { dialog.SelectedPath });
        }

        /// <summary>Habilita la funcionalidad de arrastrar y soltar archivos sobre los paneles del formulario.</summary>
        private void EnableFileDrop()
        {
            var targets = new Control[] { this, pnlMiddle, grpQueue, dgvQueue, grpActions, grpTrack, grpPlayer };
            foreach (Control target in targets)
            {
                target.AllowDrop = true;
                target.DragEnter += FileDrop_DragEnter;
                target.DragDrop += FileDrop_DragDrop;
            }
        }

        private void FileDrop_DragEnter(object? sender, DragEventArgs e)
        {
            e.Effect = e.Data?.GetDataPresent(DataFormats.FileDrop) == true
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        private void FileDrop_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is not string[] paths) return;

            // Procesa de forma asíncrona para no bloquear el hilo de Windows Explorer.
            BeginInvoke(() => ImportPaths(paths));
        }

        private void ImportPaths(IEnumerable<string> paths)
        {
            var files = new List<string>();
            foreach (string path in paths)
            {
                if (Directory.Exists(path)) files.AddRange(AudioFormats.EnumerateFiles(path));
                else files.Add(path);
            }

            if (files.Count == 0)
            {
                MessageBox.Show("No se encontraron archivos de audio para importar.", "Importar canciones",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int imported = 0;
            int failed = 0;
            int bpm = (int)numBpm.Value;

            Cursor = Cursors.WaitCursor;
            try
            {
                foreach (string file in files)
                {
                    if (AudioImporter.TryRead(file) is not { } audio)
                    {
                        failed++;
                        continue;
                    }

                    AddToActiveQueue(new Track(_nextId++, audio.Title, audio.Artist ?? UnknownArtist,
                        bpm, audio.DurationSeconds, audio.FilePath));
                    imported++;
                }
            }
            finally
            {
                Cursor = Cursors.Default;
                RefreshView();
            }

            if (imported > 1 || failed > 0)
            {
                string message = $"Canciones importadas: {imported}.";
                if (failed > 0)
                {
                    message += $"{Environment.NewLine}No se pudieron leer: {failed} " +
                               "(formato no compatible con este equipo o archivo dañado).";
                }

                MessageBox.Show(message, "Importar canciones", MessageBoxButtons.OK,
                    failed > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
        }

        // =========================================================================
        // 5. CONTROL DEL REPRODUCTOR DE AUDIO
        // Maneja reproducción real o sintetizada (ToneGenerator), timers y controles.
        // =========================================================================
        private void btnBrowse_Click(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Title = "Elige un archivo de audio",
                Filter = BuildAudioFilter()
            };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            txtAudioPath.Text = dialog.FileName;
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
                txtTitle.Text = Path.GetFileNameWithoutExtension(dialog.FileName);

            // Obtiene la duración del archivo para rellenar automáticamente la UI.
            if (AudioPlayer.TryGetLengthMs(dialog.FileName) is { } ms)
                numDuration.Value = Math.Clamp((int)Math.Round(ms / 1000.0), (int)numDuration.Minimum, (int)numDuration.Maximum);
        }

        private void btnPlayPause_Click(object? sender, EventArgs e)
        {
            if (!_player.IsLoaded)
            {
                AdvanceQueue(notifyIfEmpty: true);
                return;
            }

            if (_player.IsPaused)
            {
                _player.Resume();
                btnPlayPause.Text = PauseText;
            }
            else
            {
                _player.Pause();
                btnPlayPause.Text = PlayText;
            }
        }

        private void btnStop_Click(object? sender, EventArgs e)
        {
            StopPlayback();
            lblPlayerTrack.Text = "Reproductor detenido";
        }

        private void btnNext_Click(object? sender, EventArgs e) => AdvanceQueue(notifyIfEmpty: true);

        private void tbVolume_Scroll(object? sender, EventArgs e)
        {
            _player.VolumePercent = tbVolume.Value;
            lblVolume.Text = $"Vol. {tbVolume.Value}%";
        }

        private void tbSeek_MouseDown(object? sender, MouseEventArgs e) => _seeking = true;

        private void tbSeek_MouseUp(object? sender, MouseEventArgs e)
        {
            int length = _player.LengthMs;
            if (_player.IsLoaded && length > 0)
                _player.Seek((int)(tbSeek.Value * (long)length / tbSeek.Maximum));
            _seeking = false;
        }

        /// <summary>Actualiza el progreso de reproducción y detecta el fin de la canción.</summary>
        private void playbackTimer_Tick(object? sender, EventArgs e)
        {
            if (!_player.IsLoaded) return;

            int length = _player.LengthMs;
            int position = _player.PositionMs;

            if (!_seeking)
                tbSeek.Value = length > 0 ? Math.Clamp((int)(position * (long)tbSeek.Maximum / length), 0, tbSeek.Maximum) : 0;
            lblTime.Text = $"{FormatMs(position)} / {FormatMs(length)}";

            if (_player.HasEnded)
            {
                // Al terminar, pasa automáticamente a la siguiente canción en cola.
                if (!AdvanceQueue(notifyIfEmpty: false))
                {
                    StopPlayback();
                    lblPlayerTrack.Text = "Fin del set";
                    lblNowPlaying.Text = "Fin del set";
                    lblNowPlaying.ForeColor = SystemColors.GrayText;
                }
            }
        }

        private void StartPlayback(Track track)
        {
            try
            {
                bool hasFile = track.FilePath is not null && File.Exists(track.FilePath);
                // Si no existe un archivo físico, genera un tono de audio sintético basado en BPM.
                string path = hasFile ? track.FilePath! : ToneGenerator.CreatePreviewWav(track);

                _player.Load(path);
                _player.Play();

                lblPlayerTrack.Text = hasFile
                    ? $"{track.Title} - {track.Artist}"
                    : $"{track.Title} - {track.Artist}  (beat sintetizado, {track.Bpm} BPM)";
                btnPlayPause.Text = PauseText;
                tbSeek.Value = 0;
                lblTime.Text = $"0:00 / {FormatMs(_player.LengthMs)}";
            }
            catch (Exception ex)
            {
                StopPlayback();
                lblPlayerTrack.Text = "No se pudo reproducir esta canción: " + ex.Message;
            }
        }

        private void StopPlayback()
        {
            _player.Stop();
            btnPlayPause.Text = PlayText;
            tbSeek.Value = 0;
            lblTime.Text = "0:00 / 0:00";
        }

        private static string FormatMs(int milliseconds)
        {
            var t = TimeSpan.FromMilliseconds(milliseconds);
            return $"{(int)t.TotalMinutes}:{t.Seconds:D2}";
        }

        // =========================================================================
        // 6. PRUEBA DE RENDIMIENTO (Benchmark de Complejidad Algorítmica)
        // Ejecuta pruebas de estrés en segundo plano para comparar la eficiencia O(1) vs O(n).
        // =========================================================================
        private async void btnBenchmark_Click(object? sender, EventArgs e)
        {
            if (_benchmarkRunning) return;

            _benchmarkRunning = true;
            btnBenchmark.Enabled = false;
            int n = (int)numBenchmarkSize.Value;
            txtBenchmarkResults.Text = $"Ejecutando prueba de estrés con {n:N0} canciones... espera un momento.";

            try
            {
                // Se ejecuta en un Task para no congelar la UI.
                txtBenchmarkResults.Text = await Task.Run(() => RunBenchmark(n));
            }
            catch (Exception ex)
            {
                txtBenchmarkResults.Text = "La prueba de rendimiento falló: " + ex.Message;
            }
            finally
            {
                btnBenchmark.Enabled = true;
                _benchmarkRunning = false;
            }
        }

        private readonly record struct BenchResult(
            double CustomMiddle, double LinkedMiddle, double ListMiddle,
            double CustomEnd, double LinkedEnd, double ListEnd);

        private static string RunBenchmark(int n)
        {
            var random = new Random(42);
            var tracks = new Track[n];
            for (int i = 0; i < n; i++)
                tracks[i] = new Track(i + 1, $"Track {i + 1}", "DJ Benchmark", random.Next(100, 150), 180);
            var head = new Track(0, "Head", "DJ Benchmark", 120, 200);

            // Calentamiento previo para evitar imprecisiones por compilación JIT.
            RunSuite(tracks.Take(Math.Min(n, 500)).ToArray(), head);
            GC.Collect();
            GC.WaitForPendingFinalizers();

            BenchResult r = RunSuite(tracks, head);

            var sb = new StringBuilder();
            static string Row(string label, double ms, string note) => $"  {label,-30}: {ms,9:F2} ms   [{note}]";

            sb.AppendLine($"=== RESULTADOS DE LA PRUEBA DE ESTRÉS ({n:N0} CANCIONES) ===");
            sb.AppendLine();
            sb.AppendLine("Inserción en medio (justo después de la cabeza, \"A continuación\"):");
            sb.AppendLine(Row("Lista enlazada simple propia", r.CustomMiddle, "O(1) cada una: se reenlazan 2 referencias"));
            sb.AppendLine(Row(".NET LinkedList<T>", r.LinkedMiddle, "O(1) cada una: AddAfter sobre un nodo"));
            sb.AppendLine(Row(".NET List<T> (arreglo)", r.ListMiddle, "O(n) cada una: Array.Copy desplaza la cola"));
            sb.AppendLine();
            sb.AppendLine("Inserción al final:");
            sb.AppendLine(Row("Lista enlazada simple propia", r.CustomEnd, "O(1) gracias a la referencia a la cola"));
            sb.AppendLine(Row(".NET LinkedList<T>", r.LinkedEnd, "O(1) con AddLast"));
            sb.AppendLine(Row(".NET List<T> (arreglo)", r.ListEnd, "O(1) amortizado; a veces se redimensiona el búfer"));
            sb.AppendLine();

            double ratio = r.ListMiddle / Math.Max(r.CustomMiddle, 0.001);
            if (ratio > 1.5)
            {
                sb.AppendLine($"Conclusión técnica: en inserciones en medio, List<T> fue unas {ratio:F1} veces más lenta que la " +
                              "lista enlazada propia. Cada Insert(1) obliga al CPU a desplazar todos los elementos " +
                              "siguientes (Array.Copy), mientras que la lista enlazada solo reconecta dos referencias. " +
                              "La diferencia crece con n, como predice la notación Big-O.");
            }
            else
            {
                sb.AppendLine("Conclusión técnica: con este tamaño la diferencia es pequeña. " +
                              "Aumenta el número de canciones y ejecuta en modo Release para ver cómo el costo O(n) de " +
                              "List<T>.Insert(1) crece frente a la inserción O(1) de la lista enlazada.");
            }

            return sb.ToString();
        }

        private static BenchResult RunSuite(Track[] data, Track head)
        {
            double customMiddle = Measure(() =>
            {
                var list = new SinglyLinkedList<Track>();
                list.AddLast(head);
                foreach (var t in data) list.PlayNext(t);
            });

            double linkedMiddle = Measure(() =>
            {
                var list = new LinkedList<Track>();
                list.AddLast(head);
                foreach (var t in data) list.AddAfter(list.First!, t);
            });

            double listMiddle = Measure(() =>
            {
                var list = new List<Track> { head };
                foreach (var t in data) list.Insert(1, t);
            });

            double customEnd = Measure(() =>
            {
                var list = new SinglyLinkedList<Track>();
                foreach (var t in data) list.AddLast(t);
            });

            double linkedEnd = Measure(() =>
            {
                var list = new LinkedList<Track>();
                foreach (var t in data) list.AddLast(t);
            });

            double listEnd = Measure(() =>
            {
                var list = new List<Track>();
                foreach (var t in data) list.Add(t);
            });

            return new BenchResult(customMiddle, linkedMiddle, listMiddle, customEnd, linkedEnd, listEnd);
        }

        private static double Measure(Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }
    }
}