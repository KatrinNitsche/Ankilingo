namespace AnkiLingoApp.Helpers
{
    public interface ISoundPlayer
    {
        Task Play(string soundFilePath);
    }
}
