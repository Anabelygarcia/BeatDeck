// ---------------------------------------------------------------------------
// BeatDeck v2.0 - Main form (WinForms layout)
// Authors: <Full name(s) of team members>
// TecNM control numbers: <XXXXXXXX, XXXXXXXX>
// Date: 2026-09-28 | Version: 2.0.0
// ---------------------------------------------------------------------------
#nullable disable

namespace BeatDeck.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            grpTrack = new GroupBox();
            lblTitle = new Label();
            txtTitle = new TextBox();
            lblArtist = new Label();
            txtArtist = new TextBox();
            lblBpm = new Label();
            numBpm = new NumericUpDown();
            lblDuration = new Label();
            numDuration = new NumericUpDown();
            lblAudio = new Label();
            txtAudioPath = new TextBox();
            btnBrowse = new Button();
            btnImportFiles = new Button();
            btnImportFolder = new Button();
            rbCustom = new RadioButton();
            rbLinkedList = new RadioButton();
            rbList = new RadioButton();
            grpPlayer = new GroupBox();
            lblPlayerTrack = new Label();
            btnPlayPause = new Button();
            btnStop = new Button();
            btnNext = new Button();
            tbSeek = new TrackBar();
            lblTime = new Label();
            lblVolume = new Label();
            tbVolume = new TrackBar();
            playbackTimer = new System.Windows.Forms.Timer(components);
            grpBenchmark = new GroupBox();
            lblBenchmarkSize = new Label();
            numBenchmarkSize = new NumericUpDown();
            btnBenchmark = new Button();
            txtBenchmarkResults = new TextBox();
            pnlMiddle = new Panel();
            grpQueue = new GroupBox();
            dgvQueue = new DataGridView();
            lblNowPlaying = new Label();
            lblStats = new Label();
            grpActions = new GroupBox();
            tlpActions = new TableLayoutPanel();
            btnEnqueue = new Button();
            btnPlayNext = new Button();
            btnAdvance = new Button();
            btnReverse = new Button();
            btnSortBpm = new Button();
            btnPurge = new Button();
            grpTrack.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numBpm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDuration).BeginInit();
            grpPlayer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tbSeek).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbVolume).BeginInit();
            grpBenchmark.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numBenchmarkSize).BeginInit();
            pnlMiddle.SuspendLayout();
            grpQueue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvQueue).BeginInit();
            grpActions.SuspendLayout();
            tlpActions.SuspendLayout();
            SuspendLayout();
            // 
            // grpTrack
            // 
            grpTrack.Controls.Add(lblTitle);
            grpTrack.Controls.Add(txtTitle);
            grpTrack.Controls.Add(lblArtist);
            grpTrack.Controls.Add(txtArtist);
            grpTrack.Controls.Add(lblBpm);
            grpTrack.Controls.Add(numBpm);
            grpTrack.Controls.Add(lblDuration);
            grpTrack.Controls.Add(numDuration);
            grpTrack.Controls.Add(lblAudio);
            grpTrack.Controls.Add(txtAudioPath);
            grpTrack.Controls.Add(btnBrowse);
            grpTrack.Controls.Add(btnImportFiles);
            grpTrack.Controls.Add(btnImportFolder);
            grpTrack.Controls.Add(rbCustom);
            grpTrack.Controls.Add(rbLinkedList);
            grpTrack.Controls.Add(rbList);
            grpTrack.Dock = DockStyle.Top;
            grpTrack.Location = new Point(0, 0);
            grpTrack.Margin = new Padding(4, 5, 4, 5);
            grpTrack.Name = "grpTrack";
            grpTrack.Padding = new Padding(4, 5, 4, 5);
            grpTrack.Size = new Size(1571, 223);
            grpTrack.TabIndex = 0;
            grpTrack.TabStop = false;
            grpTrack.Text = "Registro de canción";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(21, 53);
            lblTitle.Margin = new Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(60, 25);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Título:";
            // 
            // txtTitle
            // 
            txtTitle.Location = new Point(86, 47);
            txtTitle.Margin = new Padding(4, 5, 4, 5);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(284, 31);
            txtTitle.TabIndex = 0;
            // 
            // lblArtist
            // 
            lblArtist.AutoSize = true;
            lblArtist.Location = new Point(400, 53);
            lblArtist.Margin = new Padding(4, 0, 4, 0);
            lblArtist.Name = "lblArtist";
            lblArtist.Size = new Size(67, 25);
            lblArtist.TabIndex = 1;
            lblArtist.Text = "Artista:";
            // 
            // txtArtist
            // 
            txtArtist.Location = new Point(469, 47);
            txtArtist.Margin = new Padding(4, 5, 4, 5);
            txtArtist.Name = "txtArtist";
            txtArtist.Size = new Size(284, 31);
            txtArtist.TabIndex = 1;
            // 
            // lblBpm
            // 
            lblBpm.AutoSize = true;
            lblBpm.Location = new Point(783, 53);
            lblBpm.Margin = new Padding(4, 0, 4, 0);
            lblBpm.Name = "lblBpm";
            lblBpm.Size = new Size(52, 25);
            lblBpm.TabIndex = 2;
            lblBpm.Text = "BPM:";
            // 
            // numBpm
            // 
            numBpm.Location = new Point(843, 47);
            numBpm.Margin = new Padding(4, 5, 4, 5);
            numBpm.Maximum = new decimal(new int[] { 220, 0, 0, 0 });
            numBpm.Minimum = new decimal(new int[] { 60, 0, 0, 0 });
            numBpm.Name = "numBpm";
            numBpm.Size = new Size(100, 31);
            numBpm.TabIndex = 2;
            numBpm.Value = new decimal(new int[] { 124, 0, 0, 0 });
            // 
            // lblDuration
            // 
            lblDuration.AutoSize = true;
            lblDuration.Location = new Point(971, 53);
            lblDuration.Margin = new Padding(4, 0, 4, 0);
            lblDuration.Name = "lblDuration";
            lblDuration.Size = new Size(110, 25);
            lblDuration.TabIndex = 3;
            lblDuration.Text = "Duración (s):";
            // 
            // numDuration
            // 
            numDuration.Location = new Point(1100, 47);
            numDuration.Margin = new Padding(4, 5, 4, 5);
            numDuration.Maximum = new decimal(new int[] { 7200, 0, 0, 0 });
            numDuration.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numDuration.Name = "numDuration";
            numDuration.Size = new Size(114, 31);
            numDuration.TabIndex = 3;
            numDuration.Value = new decimal(new int[] { 210, 0, 0, 0 });
            // 
            // lblAudio
            // 
            lblAudio.AutoSize = true;
            lblAudio.Location = new Point(21, 113);
            lblAudio.Margin = new Padding(4, 0, 4, 0);
            lblAudio.Name = "lblAudio";
            lblAudio.Size = new Size(151, 25);
            lblAudio.TabIndex = 4;
            lblAudio.Text = "Archivo de audio:";
            // 
            // txtAudioPath
            // 
            txtAudioPath.Location = new Point(179, 107);
            txtAudioPath.Margin = new Padding(4, 5, 4, 5);
            txtAudioPath.Name = "txtAudioPath";
            txtAudioPath.PlaceholderText = "Opcional (si no, suena un beat)";
            txtAudioPath.ReadOnly = true;
            txtAudioPath.Size = new Size(370, 31);
            txtAudioPath.TabIndex = 5;
            txtAudioPath.TabStop = false;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(564, 105);
            btnBrowse.Margin = new Padding(4, 5, 4, 5);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(136, 42);
            btnBrowse.TabIndex = 4;
            btnBrowse.Text = "Examinar...";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnImportFiles
            // 
            btnImportFiles.Location = new Point(714, 105);
            btnImportFiles.Margin = new Padding(4, 5, 4, 5);
            btnImportFiles.Name = "btnImportFiles";
            btnImportFiles.Size = new Size(250, 42);
            btnImportFiles.TabIndex = 8;
            btnImportFiles.Text = "Importar canciones...";
            btnImportFiles.UseVisualStyleBackColor = true;
            btnImportFiles.Click += btnImportFiles_Click;
            // 
            // btnImportFolder
            // 
            btnImportFolder.Location = new Point(979, 105);
            btnImportFolder.Margin = new Padding(4, 5, 4, 5);
            btnImportFolder.Name = "btnImportFolder";
            btnImportFolder.Size = new Size(214, 42);
            btnImportFolder.TabIndex = 9;
            btnImportFolder.Text = "Importar carpeta...";
            btnImportFolder.UseVisualStyleBackColor = true;
            btnImportFolder.Click += btnImportFolder_Click;
            // 
            // rbCustom
            // 
            rbCustom.AutoSize = true;
            rbCustom.Checked = true;
            rbCustom.Location = new Point(21, 170);
            rbCustom.Margin = new Padding(4, 5, 4, 5);
            rbCustom.Name = "rbCustom";
            rbCustom.Size = new Size(329, 29);
            rbCustom.TabIndex = 5;
            rbCustom.TabStop = true;
            rbCustom.Text = "Lista enlazada simple propia (Nodos)";
            rbCustom.CheckedChanged += rbMode_CheckedChanged;
            // 
            // rbLinkedList
            // 
            rbLinkedList.AutoSize = true;
            rbLinkedList.Location = new Point(429, 170);
            rbLinkedList.Margin = new Padding(4, 5, 4, 5);
            rbLinkedList.Name = "rbLinkedList";
            rbLinkedList.Size = new Size(187, 29);
            rbLinkedList.TabIndex = 6;
            rbLinkedList.Text = ".NET LinkedList<T>";
            rbLinkedList.CheckedChanged += rbMode_CheckedChanged;
            // 
            // rbList
            // 
            rbList.AutoSize = true;
            rbList.Location = new Point(671, 170);
            rbList.Margin = new Padding(4, 5, 4, 5);
            rbList.Name = "rbList";
            rbList.Size = new Size(136, 29);
            rbList.TabIndex = 7;
            rbList.Text = ".NET List<T>";
            rbList.CheckedChanged += rbMode_CheckedChanged;
            // 
            // grpPlayer
            // 
            grpPlayer.Controls.Add(lblPlayerTrack);
            grpPlayer.Controls.Add(btnPlayPause);
            grpPlayer.Controls.Add(btnStop);
            grpPlayer.Controls.Add(btnNext);
            grpPlayer.Controls.Add(tbSeek);
            grpPlayer.Controls.Add(lblTime);
            grpPlayer.Controls.Add(lblVolume);
            grpPlayer.Controls.Add(tbVolume);
            grpPlayer.Dock = DockStyle.Bottom;
            grpPlayer.Location = new Point(0, 883);
            grpPlayer.Margin = new Padding(4, 5, 4, 5);
            grpPlayer.Name = "grpPlayer";
            grpPlayer.Padding = new Padding(4, 5, 4, 5);
            grpPlayer.Size = new Size(1571, 167);
            grpPlayer.TabIndex = 3;
            grpPlayer.TabStop = false;
            grpPlayer.Text = "Reproductor de música";
            // 
            // lblPlayerTrack
            // 
            lblPlayerTrack.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblPlayerTrack.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPlayerTrack.Location = new Point(21, 37);
            lblPlayerTrack.Margin = new Padding(4, 0, 4, 0);
            lblPlayerTrack.Name = "lblPlayerTrack";
            lblPlayerTrack.Size = new Size(2792, 37);
            lblPlayerTrack.TabIndex = 0;
            lblPlayerTrack.Text = "Nada cargado: pulsa Reproducir o \"Avanzar canción\" para iniciar la cola";
            // 
            // btnPlayPause
            // 
            btnPlayPause.Location = new Point(21, 87);
            btnPlayPause.Margin = new Padding(4, 5, 4, 5);
            btnPlayPause.Name = "btnPlayPause";
            btnPlayPause.Size = new Size(129, 53);
            btnPlayPause.TabIndex = 0;
            btnPlayPause.Text = "Reproducir";
            btnPlayPause.UseVisualStyleBackColor = true;
            btnPlayPause.Click += btnPlayPause_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(159, 87);
            btnStop.Margin = new Padding(4, 5, 4, 5);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(114, 53);
            btnStop.TabIndex = 1;
            btnStop.Text = "Detener";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnNext
            // 
            btnNext.Location = new Point(281, 87);
            btnNext.Margin = new Padding(4, 5, 4, 5);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(143, 53);
            btnNext.TabIndex = 2;
            btnNext.Text = "Siguiente >>";
            btnNext.UseVisualStyleBackColor = true;
            btnNext.Click += btnNext_Click;
            // 
            // tbSeek
            // 
            tbSeek.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbSeek.LargeChange = 50;
            tbSeek.Location = new Point(443, 87);
            tbSeek.Margin = new Padding(4, 5, 4, 5);
            tbSeek.Maximum = 1000;
            tbSeek.Name = "tbSeek";
            tbSeek.Size = new Size(1885, 69);
            tbSeek.TabIndex = 3;
            tbSeek.TickStyle = TickStyle.None;
            tbSeek.MouseDown += tbSeek_MouseDown;
            tbSeek.MouseUp += tbSeek_MouseUp;
            // 
            // lblTime
            // 
            lblTime.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblTime.Location = new Point(2342, 98);
            lblTime.Margin = new Padding(4, 0, 4, 0);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(157, 33);
            lblTime.TabIndex = 4;
            lblTime.Text = "0:00 / 0:00";
            lblTime.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblVolume
            // 
            lblVolume.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblVolume.Location = new Point(2514, 98);
            lblVolume.Margin = new Padding(4, 0, 4, 0);
            lblVolume.Name = "lblVolume";
            lblVolume.Size = new Size(100, 33);
            lblVolume.TabIndex = 5;
            lblVolume.Text = "Vol. 80%";
            lblVolume.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tbVolume
            // 
            tbVolume.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            tbVolume.Location = new Point(2621, 87);
            tbVolume.Margin = new Padding(4, 5, 4, 5);
            tbVolume.Maximum = 100;
            tbVolume.Name = "tbVolume";
            tbVolume.Size = new Size(193, 69);
            tbVolume.TabIndex = 4;
            tbVolume.TickStyle = TickStyle.None;
            tbVolume.Value = 80;
            tbVolume.Scroll += tbVolume_Scroll;
            // 
            // playbackTimer
            // 
            playbackTimer.Enabled = true;
            playbackTimer.Interval = 250;
            playbackTimer.Tick += playbackTimer_Tick;
            // 
            // grpBenchmark
            // 
            grpBenchmark.Controls.Add(lblBenchmarkSize);
            grpBenchmark.Controls.Add(numBenchmarkSize);
            grpBenchmark.Controls.Add(btnBenchmark);
            grpBenchmark.Controls.Add(txtBenchmarkResults);
            grpBenchmark.Dock = DockStyle.Bottom;
            grpBenchmark.Location = new Point(0, 600);
            grpBenchmark.Margin = new Padding(4, 5, 4, 5);
            grpBenchmark.Name = "grpBenchmark";
            grpBenchmark.Padding = new Padding(4, 5, 4, 5);
            grpBenchmark.Size = new Size(1571, 283);
            grpBenchmark.TabIndex = 2;
            grpBenchmark.TabStop = false;
            grpBenchmark.Text = "Rendimiento y telemetría";
            // 
            // lblBenchmarkSize
            // 
            lblBenchmarkSize.AutoSize = true;
            lblBenchmarkSize.Location = new Point(21, 53);
            lblBenchmarkSize.Margin = new Padding(4, 0, 4, 0);
            lblBenchmarkSize.Name = "lblBenchmarkSize";
            lblBenchmarkSize.Size = new Size(215, 25);
            lblBenchmarkSize.TabIndex = 0;
            lblBenchmarkSize.Text = "Canciones para la prueba:";
            // 
            // numBenchmarkSize
            // 
            numBenchmarkSize.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            numBenchmarkSize.Location = new Point(250, 47);
            numBenchmarkSize.Margin = new Padding(4, 5, 4, 5);
            numBenchmarkSize.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numBenchmarkSize.Minimum = new decimal(new int[] { 1000, 0, 0, 0 });
            numBenchmarkSize.Name = "numBenchmarkSize";
            numBenchmarkSize.Size = new Size(143, 31);
            numBenchmarkSize.TabIndex = 0;
            numBenchmarkSize.ThousandsSeparator = true;
            numBenchmarkSize.Value = new decimal(new int[] { 25000, 0, 0, 0 });
            // 
            // btnBenchmark
            // 
            btnBenchmark.Location = new Point(421, 43);
            btnBenchmark.Margin = new Padding(4, 5, 4, 5);
            btnBenchmark.Name = "btnBenchmark";
            btnBenchmark.Size = new Size(343, 47);
            btnBenchmark.TabIndex = 1;
            btnBenchmark.Text = "Ejecutar prueba de rendimiento";
            btnBenchmark.UseVisualStyleBackColor = true;
            btnBenchmark.Click += btnBenchmark_Click;
            // 
            // txtBenchmarkResults
            // 
            txtBenchmarkResults.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtBenchmarkResults.Font = new Font("Consolas", 9.5F);
            txtBenchmarkResults.Location = new Point(21, 107);
            txtBenchmarkResults.Margin = new Padding(4, 5, 4, 5);
            txtBenchmarkResults.Multiline = true;
            txtBenchmarkResults.Name = "txtBenchmarkResults";
            txtBenchmarkResults.ReadOnly = true;
            txtBenchmarkResults.ScrollBars = ScrollBars.Vertical;
            txtBenchmarkResults.Size = new Size(2790, 152);
            txtBenchmarkResults.TabIndex = 2;
            // 
            // pnlMiddle
            // 
            pnlMiddle.Controls.Add(grpQueue);
            pnlMiddle.Controls.Add(grpActions);
            pnlMiddle.Dock = DockStyle.Fill;
            pnlMiddle.Location = new Point(0, 223);
            pnlMiddle.Margin = new Padding(4, 5, 4, 5);
            pnlMiddle.Name = "pnlMiddle";
            pnlMiddle.Size = new Size(1571, 377);
            pnlMiddle.TabIndex = 1;
            // 
            // grpQueue
            // 
            grpQueue.Controls.Add(dgvQueue);
            grpQueue.Controls.Add(lblNowPlaying);
            grpQueue.Controls.Add(lblStats);
            grpQueue.Dock = DockStyle.Fill;
            grpQueue.Location = new Point(343, 0);
            grpQueue.Margin = new Padding(4, 5, 4, 5);
            grpQueue.Name = "grpQueue";
            grpQueue.Padding = new Padding(4, 5, 4, 5);
            grpQueue.Size = new Size(1228, 377);
            grpQueue.TabIndex = 1;
            grpQueue.TabStop = false;
            grpQueue.Text = "Lista de reproducción / Cola en vivo";
            // 
            // dgvQueue
            // 
            dgvQueue.BackgroundColor = SystemColors.Window;
            dgvQueue.ColumnHeadersHeight = 34;
            dgvQueue.Dock = DockStyle.Fill;
            dgvQueue.Location = new Point(4, 102);
            dgvQueue.Margin = new Padding(4, 5, 4, 5);
            dgvQueue.Name = "dgvQueue";
            dgvQueue.RowHeadersWidth = 62;
            dgvQueue.Size = new Size(1220, 223);
            dgvQueue.TabIndex = 0;
            // 
            // lblNowPlaying
            // 
            lblNowPlaying.Dock = DockStyle.Top;
            lblNowPlaying.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblNowPlaying.ForeColor = SystemColors.GrayText;
            lblNowPlaying.Location = new Point(4, 29);
            lblNowPlaying.Margin = new Padding(4, 0, 4, 0);
            lblNowPlaying.Name = "lblNowPlaying";
            lblNowPlaying.Size = new Size(1220, 73);
            lblNowPlaying.TabIndex = 1;
            lblNowPlaying.Text = "Nada suena todavía: pulsa \"Avanzar canción\"";
            lblNowPlaying.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblStats
            // 
            lblStats.Dock = DockStyle.Bottom;
            lblStats.Location = new Point(4, 325);
            lblStats.Margin = new Padding(4, 0, 4, 0);
            lblStats.Name = "lblStats";
            lblStats.Size = new Size(1220, 47);
            lblStats.TabIndex = 2;
            lblStats.Text = "Canciones en cola: 0";
            lblStats.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // grpActions
            // 
            grpActions.Controls.Add(tlpActions);
            grpActions.Dock = DockStyle.Left;
            grpActions.Location = new Point(0, 0);
            grpActions.Margin = new Padding(4, 5, 4, 5);
            grpActions.Name = "grpActions";
            grpActions.Padding = new Padding(4, 5, 4, 5);
            grpActions.Size = new Size(343, 377);
            grpActions.TabIndex = 0;
            grpActions.TabStop = false;
            grpActions.Text = "Acciones de la cola";
            // 
            // tlpActions
            // 
            tlpActions.ColumnCount = 1;
            tlpActions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpActions.Controls.Add(btnEnqueue, 0, 0);
            tlpActions.Controls.Add(btnPlayNext, 0, 1);
            tlpActions.Controls.Add(btnAdvance, 0, 2);
            tlpActions.Controls.Add(btnReverse, 0, 3);
            tlpActions.Controls.Add(btnSortBpm, 0, 4);
            tlpActions.Controls.Add(btnPurge, 0, 5);
            tlpActions.Dock = DockStyle.Fill;
            tlpActions.Location = new Point(4, 29);
            tlpActions.Margin = new Padding(4, 5, 4, 5);
            tlpActions.Name = "tlpActions";
            tlpActions.Padding = new Padding(11, 3, 11, 13);
            tlpActions.RowCount = 6;
            tlpActions.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));
            tlpActions.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));
            tlpActions.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));
            tlpActions.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));
            tlpActions.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));
            tlpActions.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));
            tlpActions.Size = new Size(335, 343);
            tlpActions.TabIndex = 0;
            // 
            // btnEnqueue
            // 
            btnEnqueue.Dock = DockStyle.Fill;
            btnEnqueue.Location = new Point(15, 8);
            btnEnqueue.Margin = new Padding(4, 5, 4, 5);
            btnEnqueue.MinimumSize = new Size(0, 47);
            btnEnqueue.Name = "btnEnqueue";
            btnEnqueue.Size = new Size(305, 47);
            btnEnqueue.TabIndex = 0;
            btnEnqueue.Text = "+ Agregar al final";
            btnEnqueue.UseVisualStyleBackColor = true;
            btnEnqueue.Click += btnEnqueue_Click;
            // 
            // btnPlayNext
            // 
            btnPlayNext.Dock = DockStyle.Fill;
            btnPlayNext.Location = new Point(15, 62);
            btnPlayNext.Margin = new Padding(4, 5, 4, 5);
            btnPlayNext.MinimumSize = new Size(0, 47);
            btnPlayNext.Name = "btnPlayNext";
            btnPlayNext.Size = new Size(305, 47);
            btnPlayNext.TabIndex = 1;
            btnPlayNext.Text = "Reproducir a continuación";
            btnPlayNext.UseVisualStyleBackColor = true;
            btnPlayNext.Click += btnPlayNext_Click;
            // 
            // btnAdvance
            // 
            btnAdvance.Dock = DockStyle.Fill;
            btnAdvance.Location = new Point(15, 116);
            btnAdvance.Margin = new Padding(4, 5, 4, 5);
            btnAdvance.MinimumSize = new Size(0, 47);
            btnAdvance.Name = "btnAdvance";
            btnAdvance.Size = new Size(305, 47);
            btnAdvance.TabIndex = 2;
            btnAdvance.Text = "Avanzar canción";
            btnAdvance.UseVisualStyleBackColor = true;
            btnAdvance.Click += btnAdvance_Click;
            // 
            // btnReverse
            // 
            btnReverse.Dock = DockStyle.Fill;
            btnReverse.Location = new Point(15, 170);
            btnReverse.Margin = new Padding(4, 5, 4, 5);
            btnReverse.MinimumSize = new Size(0, 47);
            btnReverse.Name = "btnReverse";
            btnReverse.Size = new Size(305, 47);
            btnReverse.TabIndex = 3;
            btnReverse.Text = "Invertir lista (en sitio)";
            btnReverse.UseVisualStyleBackColor = true;
            btnReverse.Click += btnReverse_Click;
            // 
            // btnSortBpm
            // 
            btnSortBpm.Dock = DockStyle.Fill;
            btnSortBpm.Location = new Point(15, 224);
            btnSortBpm.Margin = new Padding(4, 5, 4, 5);
            btnSortBpm.MinimumSize = new Size(0, 47);
            btnSortBpm.Name = "btnSortBpm";
            btnSortBpm.Size = new Size(305, 47);
            btnSortBpm.TabIndex = 4;
            btnSortBpm.Text = "Ordenar por curva de BPM";
            btnSortBpm.UseVisualStyleBackColor = true;
            btnSortBpm.Click += btnSortBpm_Click;
            // 
            // btnPurge
            // 
            btnPurge.Dock = DockStyle.Fill;
            btnPurge.Location = new Point(15, 278);
            btnPurge.Margin = new Padding(4, 5, 4, 5);
            btnPurge.MinimumSize = new Size(0, 47);
            btnPurge.Name = "btnPurge";
            btnPurge.Size = new Size(305, 47);
            btnPurge.TabIndex = 5;
            btnPurge.Text = "Eliminar duplicados";
            btnPurge.UseVisualStyleBackColor = true;
            btnPurge.Click += btnPurge_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1571, 1050);
            Controls.Add(pnlMiddle);
            Controls.Add(grpBenchmark);
            Controls.Add(grpPlayer);
            Controls.Add(grpTrack);
            Margin = new Padding(4, 5, 4, 5);
            MinimumSize = new Size(1276, 1006);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "BeatDeck v2.0 - Controlador de set para DJ [TecNM Monclova]";
            grpTrack.ResumeLayout(false);
            grpTrack.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numBpm).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDuration).EndInit();
            grpPlayer.ResumeLayout(false);
            grpPlayer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tbSeek).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbVolume).EndInit();
            grpBenchmark.ResumeLayout(false);
            grpBenchmark.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numBenchmarkSize).EndInit();
            pnlMiddle.ResumeLayout(false);
            grpQueue.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvQueue).EndInit();
            grpActions.ResumeLayout(false);
            tlpActions.ResumeLayout(false);
            ResumeLayout(false);
        }

        private GroupBox grpTrack;
        private Label lblTitle;
        private TextBox txtTitle;
        private Label lblArtist;
        private TextBox txtArtist;
        private Label lblBpm;
        private NumericUpDown numBpm;
        private Label lblDuration;
        private NumericUpDown numDuration;
        private Label lblAudio;
        private TextBox txtAudioPath;
        private Button btnBrowse;
        private Button btnImportFiles;
        private Button btnImportFolder;
        private RadioButton rbCustom;
        private RadioButton rbLinkedList;
        private RadioButton rbList;

        private GroupBox grpPlayer;
        private Label lblPlayerTrack;
        private Button btnPlayPause;
        private Button btnStop;
        private Button btnNext;
        private TrackBar tbSeek;
        private Label lblTime;
        private Label lblVolume;
        private TrackBar tbVolume;
        private System.Windows.Forms.Timer playbackTimer;

        private GroupBox grpBenchmark;
        private Label lblBenchmarkSize;
        private NumericUpDown numBenchmarkSize;
        private Button btnBenchmark;
        private TextBox txtBenchmarkResults;

        private Panel pnlMiddle;
        private GroupBox grpActions;
        private TableLayoutPanel tlpActions;
        private Button btnEnqueue;
        private Button btnPlayNext;
        private Button btnAdvance;
        private Button btnReverse;
        private Button btnSortBpm;
        private Button btnPurge;
        private GroupBox grpQueue;
        private DataGridView dgvQueue;
        private Label lblNowPlaying;
        private Label lblStats;
    }
}
