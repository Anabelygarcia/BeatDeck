namespace BeatDeck.Audio
{
    /// <summary>
    /// Catálogo de extensiones de archivos de audio que el reproductor integrado intenta abrir.
    /// La reproducción efectiva de cada archivo depende de los códecs instalados en Windows;
    /// el importador lo verifica intentando abrir cada archivo individualmente.
    /// </summary>
    public static class AudioFormats
    {
        // Lista de extensiones de audio soportadas por el catálogo.
        private static readonly string[] Extensions =
        {
            ".mp3", ".wav", ".wma", ".aac", ".m4a", ".flac", ".ogg", ".oga", ".opus",
            ".aif", ".aiff", ".aifc", ".mid", ".midi", ".mp2", ".mp4", ".3gp", ".amr",
            ".ac3", ".mka", ".au", ".snd"
        };

        // Conjunto Hash (HashSet) para realizar búsquedas O(1) ignorando mayúsculas y minúsculas.
        private static readonly HashSet<string> ExtensionSet =
            new(Extensions, StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Obtiene los patrones de búsqueda con comodines separados por punto y coma 
        /// para usarlos en filtros de diálogos de archivos (ej. "*.mp3;*.wav").
        /// </summary>
        public static string Patterns { get; } = string.Join(";", Extensions.Select(e => "*" + e));

        /// <summary>
        /// Evalúa si la extensión de la ruta dada coincide con alguna del catálogo de extensiones soportadas.
        /// </summary>
        /// <param name="path">Ruta del archivo a verificar.</param>
        /// <returns>True si la extensión está dentro del catálogo; de lo contrario, False.</returns>
        public static bool IsSupported(string path) => ExtensionSet.Contains(Path.GetExtension(path));

        /// <summary>
        /// Encuentra recursivamente todos los archivos con extensión de audio conocida dentro de una carpeta y sus subcarpetas.
        /// </summary>
        /// <param name="folder">Ruta del directorio principal a explorar.</param>
        /// <returns>Colección ordenada alfabéticamente con las rutas completas de los archivos compatibles encontrados.</returns>
        public static IEnumerable<string> EnumerateFiles(string folder)
        {
            var options = new EnumerationOptions
            {
                RecurseSubdirectories = true, // Incluye subcarpetas en la búsqueda.
                IgnoreInaccessible = true      // Omitir carpetas o archivos sin permisos de acceso.
            };

            return Directory.EnumerateFiles(folder, "*", options)
                .Where(IsSupported)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase);
        }
    }
}