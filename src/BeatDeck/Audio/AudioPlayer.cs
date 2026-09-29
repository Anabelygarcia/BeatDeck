using System.Runtime.InteropServices;
using System.Text;

namespace BeatDeck.Audio
{
    /// <summary>
    /// Motor de audio mínimo integrado dentro de la aplicación.
    /// Se comunica directamente con la API multimedia nativa de Windows (winmm.dll / MCI):
    /// NO inicia ni incrusta Windows Media Player, la app "Música" de Windows ni ningún otro programa externo,
    /// y no requiere paquetes de NuGet adicionales.
    /// Debe invocarse únicamente desde el hilo de la interfaz de usuario (UI Thread).
    /// </summary>
    public sealed class AudioPlayer : IDisposable
    {
        // Interop nativo con la API de Windows (winmm.dll) para enviar comandos MCI (Media Control Interface).
        [DllImport("winmm.dll", CharSet = CharSet.Unicode)]
        private static extern int mciSendString(string command, StringBuilder? returnValue, int returnLength, IntPtr callback);

        // Interop nativo para obtener descripciones en texto de errores de MCI.
        [DllImport("winmm.dll", CharSet = CharSet.Unicode)]
        private static extern bool mciGetErrorString(int errorCode, StringBuilder buffer, int bufferLength);

        // Alias único asignado a la instancia del reproductor dentro de MCI.
        private const string Alias = "beatdeck_player";

        private bool _isOpen;
        private bool _paused;
        private int _volumePercent = 80;

        /// <summary>Indica si hay un archivo de audio abierto y cargado en el reproductor.</summary>
        public bool IsLoaded => _isOpen;

        /// <summary>Indica si la reproducción actual se encuentra pausada.</summary>
        public bool IsPaused => _paused;

        /// <summary>
        /// Devuelve true cuando el archivo cargado ha llegado a su fin por sí solo 
        /// (no pausado ni detenido manualmente por el usuario).
        /// </summary>
        public bool HasEnded => _isOpen && !_paused && Query("mode") == "stopped";

        /// <summary>Obtiene la duración total del archivo cargado en milisegundos.</summary>
        public int LengthMs => _isOpen && int.TryParse(Query("length"), out int v) ? v : 0;

        /// <summary>Obtiene la posición actual de reproducción en milisegundos.</summary>
        public int PositionMs => _isOpen && int.TryParse(Query("position"), out int v) ? v : 0;

        /// <summary>Obtiene o establece el porcentaje de volumen (0 a 100).</summary>
        public int VolumePercent
        {
            get => _volumePercent;
            set
            {
                _volumePercent = Math.Clamp(value, 0, 100);
                if (_isOpen) ApplyVolume();
            }
        }

        /// <summary>
        /// Abre un archivo de audio (.mp3, .wav, .wma, etc.).
        /// Lanza una excepción <see cref="InvalidOperationException"/> si ocurre un fallo al abrirlo.
        /// </summary>
        /// <param name="path">Ruta del archivo de audio.</param>
        public void Load(string path)
        {
            Close(); // Cierra cualquier archivo previamente abierto.

            // Intento estándar de apertura con MCI.
            int error = mciSendString($"open \"{path}\" alias {Alias}", null, 0, IntPtr.Zero);
            if (error != 0)
            {
                // Algunos sistemas requieren solicitar explícitamente el dispositivo basado en DirectShow (mpegvideo).
                error = mciSendString($"open \"{path}\" type mpegvideo alias {Alias}", null, 0, IntPtr.Zero);
                if (error != 0) throw new InvalidOperationException(DescribeError(error));
            }

            _isOpen = true;
            _paused = false;

            // Configura el formato de tiempo en milisegundos para consultas de duración y posición.
            Send($"set {Alias} time format milliseconds");
            ApplyVolume();
        }

        /// <summary>Inicia o reanuda la reproducción del archivo cargado.</summary>
        public void Play()
        {
            if (!_isOpen) return;
            Send($"play {Alias}");
            _paused = false;
        }

        /// <summary>Pausa la reproducción actual.</summary>
        public void Pause()
        {
            if (!_isOpen || _paused) return;
            Send($"pause {Alias}");
            _paused = true;
        }

        /// <summary>Reanuda la reproducción desde el estado pausado.</summary>
        public void Resume()
        {
            if (!_isOpen || !_paused) return;

            // Tras un seek mientras estaba pausado, el dispositivo puede quedar en estado "stopped", 
            // haciendo que 'resume' falle: en ese caso se recurre al comando 'play'.
            if (mciSendString($"resume {Alias}", null, 0, IntPtr.Zero) != 0)
                Send($"play {Alias}");

            _paused = false;
        }

        /// <summary>Detiene la reproducción y libera el archivo cargado.</summary>
        public void Stop() => Close();

        /// <summary>
        /// Salta a una posición específica de la canción expresada en milisegundos.
        /// </summary>
        /// <param name="positionMs">Posición destino en milisegundos.</param>
        public void Seek(int positionMs)
        {
            if (!_isOpen) return;

            // Delimita el rango de posición entre 0 y el final de la pista (con margen de seguridad).
            positionMs = Math.Clamp(positionMs, 0, Math.Max(0, LengthMs - 50));
            Send($"seek {Alias} to {positionMs}");

            // MCI detiene la reproducción tras ejecutar un seek; si no estaba pausado, reanuda la reproducción.
            if (!_paused) Send($"play {Alias}");
        }

        /// <summary>
        /// Obtiene la duración total de un archivo de audio en milisegundos sin reproducirlo,
        /// o devuelve null si el archivo no se puede leer o abrir.
        /// </summary>
        /// <param name="path">Ruta del archivo a inspeccionar.</param>
        public static int? TryGetLengthMs(string path)
        {
            const string probe = "beatdeck_probe"; // Alias temporal para la inspección.
            try
            {
                // Intenta abrir el archivo mediante MCI en modo estándar o mpegvideo.
                if (mciSendString($"open \"{path}\" alias {probe}", null, 0, IntPtr.Zero) != 0 &&
                    mciSendString($"open \"{path}\" type mpegvideo alias {probe}", null, 0, IntPtr.Zero) != 0)
                    return null;

                mciSendString($"set {probe} time format milliseconds", null, 0, IntPtr.Zero);
                var sb = new StringBuilder(64);
                int result = mciSendString($"status {probe} length", sb, sb.Capacity, IntPtr.Zero);
                mciSendString($"close {probe}", null, 0, IntPtr.Zero);

                return result == 0 && int.TryParse(sb.ToString(), out int ms) ? ms : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Libera los recursos de MCI asociados al reproductor.
        /// </summary>
        public void Dispose() => Close();

        // ------------------------------------------------------------------
        // Métodos auxiliares privados para comunicación y control de MCI
        // ------------------------------------------------------------------

        /// <summary>Detiene la reproducción activa y cierra el alias en MCI.</summary>
        private void Close()
        {
            if (_isOpen)
            {
                mciSendString($"stop {Alias}", null, 0, IntPtr.Zero);
                mciSendString($"close {Alias}", null, 0, IntPtr.Zero);
            }
            _isOpen = false;
            _paused = false;
        }

        /// <summary>
        /// Aplica la configuración de volumen.
        /// El rango de volumen en MCI es de 0 a 1000 (se multiplica el porcentaje por 10).
        /// </summary>
        private void ApplyVolume()
        {
            // No todos los dispositivos o códecs soportan setaudio, por lo que se ignoran errores.
            mciSendString($"setaudio {Alias} volume to {_volumePercent * 10}", null, 0, IntPtr.Zero);
        }

        /// <summary>Consulta un parámetro de estado específico a MCI (ej. "length", "position", "mode").</summary>
        private string Query(string what)
        {
            var sb = new StringBuilder(128);
            return mciSendString($"status {Alias} {what}", sb, sb.Capacity, IntPtr.Zero) == 0
                ? sb.ToString()
                : string.Empty;
        }

        /// <summary>Envía un comando a MCI y lanza una excepción si devuelve un código de error.</summary>
        private static void Send(string command)
        {
            int error = mciSendString(command, null, 0, IntPtr.Zero);
            if (error != 0) throw new InvalidOperationException(DescribeError(error));
        }

        /// <summary>Obtiene el mensaje de texto descriptivo a partir del código de error de MCI.</summary>
        private static string DescribeError(int error)
        {
            var sb = new StringBuilder(256);
            return mciGetErrorString(error, sb, sb.Capacity) ? sb.ToString() : $"Error de MCI {error}";
        }
    }
}