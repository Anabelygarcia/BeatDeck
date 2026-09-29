using BeatDeck.Models;

namespace BeatDeck.Audio
{
    /// <summary>
    /// Construye un bucle electrónico corto (bombo/kick, contratiempo/hi-hat, bajo y un pequeño arpegio) al BPM de la pista
    /// y lo guarda como un archivo WAV. Permite que cada pista en la cola genere sonido incluso si el usuario
    /// no adjuntó un archivo de audio real. El ritmo se genera desde cero: no se utiliza audio externo.
    /// </summary>
    public static class ToneGenerator
    {
        private const int SampleRate = 22050; // Frecuencia de muestreo en Hz
        private static readonly int[] Scale = { 0, 3, 5, 7, 10, 12, 15 }; // Intervalos de escala tipo pentatónica menor

        // Directorio temporal donde se almacenan las previsualizaciones generadas.
        private static string CacheDirectory => Path.Combine(Path.GetTempPath(), "BeatDeck");

        /// <summary>
        /// Devuelve la ruta de un archivo WAV de previsualización para la pista (se crea en el primer uso y luego se almacena en caché).
        /// </summary>
        /// <param name="track">Objeto pista con los datos de ID, BPM y duración.</param>
        /// <param name="maxSeconds">Límite máximo de duración en segundos para la previsualización.</param>
        /// <returns>Ruta completa del archivo WAV en el directorio caché.</returns>
        public static string CreatePreviewWav(Track track, int maxSeconds = 45)
        {
            int seconds = Math.Clamp(track.DurationSeconds, 5, maxSeconds);
            Directory.CreateDirectory(CacheDirectory);
            string path = Path.Combine(CacheDirectory, $"track_{track.Id}_{track.Bpm}_{seconds}.wav");

            // Si el archivo ya existe en caché, reutiliza la versión almacenada.
            if (File.Exists(path)) return path;

            // Sintetiza el audio PCM y lo escribe en el archivo WAV.
            WriteWav(path, Synthesize(track, seconds));
            return path;
        }

        /// <summary>
        /// Elimina los archivos WAV temporales creados en caché. Se invoca al cerrar la aplicación.
        /// </summary>
        public static void CleanUp()
        {
            try
            {
                if (Directory.Exists(CacheDirectory)) Directory.Delete(CacheDirectory, recursive: true);
            }
            catch
            {
                // Archivos temporales: no se realiza ninguna acción si aún están en uso por algún proceso.
            }
        }

        /// <summary>
        /// Genera el buffer de muestras de audio PCM sintetizando los instrumentos (kick, hi-hat, bajo y arpegio).
        /// </summary>
        private static float[] Synthesize(Track track, int seconds)
        {
            var pcm = new float[seconds * SampleRate];
            double beat = 60.0 / Math.Clamp(track.Bpm, 40, 240); // Duración de un pulso/tiempo en segundos
            var random = new Random(track.Id * 7919 + track.Bpm);
            double root = 55.0 * Math.Pow(2, (track.Id % 12) / 12.0); // Tonalidad base diferente para cada pista (A1 = 55Hz)

            for (int b = 0; b * beat < seconds; b++)
            {
                double t = b * beat;

                // Sintetiza bombo en el tiempo principal y hi-hat en el contratiempo.
                AddKick(pcm, t);
                AddHat(pcm, t + beat / 2, random);

                // Bajo en cada tiempo (con variación de nota según la escala).
                int bassStep = b % 4 == 0 ? 0 : Scale[random.Next(3)];
                AddNote(pcm, t, root * Math.Pow(2, bassStep / 12.0), beat * 0.9, 0.35f);

                // Arpegio de dos notas por tiempo.
                for (int h = 0; h < 2; h++)
                {
                    double freq = root * 4 * Math.Pow(2, Scale[random.Next(Scale.Length)] / 12.0);
                    AddNote(pcm, t + h * beat / 2, freq, beat * 0.45, 0.12f);
                }
            }

            // Recorte suave (Soft clipping via Math.Tanh) y desvanecimiento final (fade-out) para evitar clics de audio.
            int fade = SampleRate / 3;
            for (int i = 0; i < pcm.Length; i++)
            {
                float x = (float)Math.Tanh(pcm[i]);
                int remaining = pcm.Length - i;
                if (remaining < fade) x *= remaining / (float)fade;
                pcm[i] = x;
            }
            return pcm;
        }

        /// <summary>
        /// Agrega un sonido de bombo (kick) con envolvente de frecuencia descendente rápida.
        /// </summary>
        private static void AddKick(float[] pcm, double startSeconds)
        {
            int start = (int)(startSeconds * SampleRate);
            int length = (int)(0.28 * SampleRate);
            double phase = 0;
            for (int i = 0; i < length && start + i < pcm.Length; i++)
            {
                double t = i / (double)SampleRate;
                double freq = 45 + 105 * Math.Exp(-t * 22); // Caída rápida de tono: el golpe inicial o "thump"
                phase += 2 * Math.PI * freq / SampleRate;
                pcm[start + i] += (float)(Math.Sin(phase) * Math.Exp(-t * 9) * 0.9);
            }
        }

        /// <summary>
        /// Agrega un sonido de contratiempo (hi-hat) utilizando ruido blanco con envolvente corta.
        /// </summary>
        private static void AddHat(float[] pcm, double startSeconds, Random random)
        {
            int start = (int)(startSeconds * SampleRate);
            int length = (int)(0.06 * SampleRate);
            for (int i = 0; i < length && start + i < pcm.Length; i++)
            {
                double t = i / (double)SampleRate;
                double noise = random.NextDouble() * 2 - 1;
                pcm[start + i] += (float)(noise * Math.Exp(-t * 90) * 0.22);
            }
        }

        /// <summary>
        /// Agrega una nota senoidal con una envolvente de ataque y decaimiento.
        /// </summary>
        private static void AddNote(float[] pcm, double startSeconds, double freq, double lengthSeconds, float gain)
        {
            int start = (int)(startSeconds * SampleRate);
            int length = (int)(lengthSeconds * SampleRate);
            for (int i = 0; i < length && start + i < pcm.Length; i++)
            {
                double t = i / (double)SampleRate;
                double envelope = (1 - Math.Exp(-t * 400)) * Math.Exp(-t * 5); // Envolvente de ataque rápido y decaimiento
                pcm[start + i] += (float)(Math.Sin(2 * Math.PI * freq * t) * envelope * gain);
            }
        }

        /// <summary>
        /// Escribe los datos de audio en formato WAV PCM de 16 bits monoaural.
        /// </summary>
        private static void WriteWav(string path, float[] samples)
        {
            using var writer = new BinaryWriter(File.Create(path));
            int dataBytes = samples.Length * 2; // 2 bytes por muestra (16 bits)

            writer.Write("RIFF"u8);
            writer.Write(36 + dataBytes);
            writer.Write("WAVE"u8);
            writer.Write("fmt "u8);
            writer.Write(16);                   // Tamaño de la cabecera PCM
            writer.Write((short)1);             // Formato PCM (sin compresión)
            writer.Write((short)1);             // Canal mono (1 canal)
            writer.Write(SampleRate);           // Frecuencia de muestreo (Hz)
            writer.Write(SampleRate * 2);       // Tasa de bytes por segundo (Byte Rate)
            writer.Write((short)2);             // Alineación de bloque (Block Align: canales * bytes por muestra)
            writer.Write((short)16);            // Bits por muestra (Bits per Sample)
            writer.Write("data"u8);
            writer.Write(dataBytes);

            // Convierte cada muestra flotante [-1.0, 1.0] a un entero de 16 bits con signo.
            foreach (float s in samples)
                writer.Write((short)(Math.Clamp(s, -1f, 1f) * short.MaxValue));
        }
    }
}