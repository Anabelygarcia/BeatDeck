namespace BeatDeck.Audio
{
    /// <summary>
    /// Registro inmutable con la metadatos obtenidos de un archivo de audio importado.
    /// <paramref name="Artist"/> será nulo si no se puede deducir del nombre del archivo.
    /// </summary>
    /// <param name="FilePath">Ruta completa del archivo de audio en el disco.</param>
    /// <param name="Title">Título extraído de la pista de audio.</param>
    /// <param name="Artist">Nombre del artista o null si no se detectó un formato "Artista - Título".</param>
    /// <param name="DurationSeconds">Duración de la pista redondeada a segundos.</param>
    public sealed record ImportedAudio(string FilePath, string Title, string? Artist, int DurationSeconds);

    /// <summary>
    /// Lee la información necesaria para agregar un archivo de audio a la cola de reproducción.
    /// Los archivos que el reproductor no puede abrir son rechazados aquí, garantizando que
    /// todo lo que se importe sea efectivamente reproducible.
    /// </summary>
    public static class AudioImporter
    {
        // Separador estándar utilizado para diferenciar el artista del título en los nombres de archivo.
        private const string ArtistTitleSeparator = " - ";

        /// <summary>
        /// Devuelve la información del archivo de audio, o null si el archivo no existe o Windows no puede abrirlo.
        /// El título y el artista se extraen del nombre del archivo: los nombres con formato "Artista - Título.mp3"
        /// se dividen automáticamente; cualquier otro formato se asigna completamente al título.
        /// </summary>
        /// <param name="path">Ruta del archivo de audio a evaluar e importar.</param>
        /// <returns>Objeto <see cref="ImportedAudio"/> con los metadatos o null si falla la lectura.</returns>
        public static ImportedAudio? TryRead(string path)
        {
            // 1. Verifica la existencia física del archivo.
            if (!File.Exists(path)) return null;

            // 2. Intenta abrir y obtener la duración real en milisegundos mediante el reproductor.
            if (AudioPlayer.TryGetLengthMs(path) is not { } lengthMs) return null;

            // 3. Extrae el nombre del archivo sin extensión.
            string name = Path.GetFileNameWithoutExtension(path).Trim();
            string title = name;
            string? artist = null;

            // 4. Intenta separar el Artista y el Título usando la secuencia " - ".
            int split = name.IndexOf(ArtistTitleSeparator, StringComparison.Ordinal);
            if (split > 0 && split < name.Length - ArtistTitleSeparator.Length)
            {
                artist = name[..split].Trim();                                       // Extrae el texto previo al separador
                title = name[(split + ArtistTitleSeparator.Length)..].Trim();        // Extrae el texto posterior al separador
            }

            // 5. Si el título queda vacío tras el procesamiento, usa el nombre completo del archivo como respaldo.
            if (title.Length == 0) title = Path.GetFileName(path);

            // 6. Convierte la duración de milisegundos a segundos (mínimo 1 segundo).
            int seconds = Math.Max(1, (int)Math.Round(lengthMs / 1000.0));
            return new ImportedAudio(path, title, artist, seconds);
        }
    }
}