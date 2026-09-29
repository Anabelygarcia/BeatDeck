namespace BeatDeck.Models
{
    /// <summary>
    /// Registro inmutable que describe una sola pista de música dentro de la cola de reproducción del DJ.
    /// El parámetro <paramref name="FilePath"/> es opcional: si no se especifica, el reproductor
    /// integrado genera y reproduce un beat sintético basado en el BPM de la pista.
    /// </summary>
    /// <param name="Id">Identificador único asignado a la canción.</param>
    /// <param name="Title">Título de la canción.</param>
    /// <param name="Artist">Nombre del artista o creador.</param>
    /// <param name="Bpm">Tempo de la canción en pulsaciones por minuto (Beats Per Minute).</param>
    /// <param name="DurationSeconds">Duración total de la canción en segundos.</param>
    /// <param name="FilePath">Ruta local del archivo de audio (.mp3, .wav, etc.). Puede ser nulo.</param>
    public record Track(int Id, string Title, string Artist, int Bpm, int DurationSeconds, string? FilePath = null)
    {
        /// <summary>
        /// Devuelve una representación en texto formateado con los detalles principales de la canción.
        /// Ejemplo: "[ID: 001] Strobe - deadmau5 | 128 BPM (634s)"
        /// </summary>
        public override string ToString() =>
            $"[ID: {Id:D3}] {Title} - {Artist} | {Bpm} BPM ({DurationSeconds}s)";
    }
}