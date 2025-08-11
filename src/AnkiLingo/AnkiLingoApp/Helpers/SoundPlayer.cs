using Microsoft.JSInterop;

namespace AnkiLingoApp.Helpers
{
    public class SoundPlayer : ISoundPlayer
    {
        private readonly IJSRuntime _jSRuntime;
        private const string JSFUNCTION_NAME = "playaudio";

        public SoundPlayer(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime ?? throw new ArgumentNullException(nameof(jSRuntime));
        }

        public async Task Play(string soundFilePath)
        {
            if (string.IsNullOrWhiteSpace(soundFilePath))
            {
                throw new ArgumentException("Sound file path cannot be null or empty.", nameof(soundFilePath));
            }
           
            await _jSRuntime.InvokeVoidAsync(JSFUNCTION_NAME, soundFilePath);
        }
    }
}
