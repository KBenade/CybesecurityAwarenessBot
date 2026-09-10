using System;
using System.IO;
using System.Media;

namespace CybersecurityBot;

/// <summary>Plays the recorded WAV greeting when the application starts.</summary>
internal sealed class VoiceGreeting
{
    private const string GreetingFileName = "GreetingVoice.wav";

    public void Play()
    {
        string audioPath = Path.Combine(AppContext.BaseDirectory, "Assets", GreetingFileName);

        if (!File.Exists(audioPath))
        {
            return;
        }

        try
        {
            using var player = new SoundPlayer(audioPath);
            player.PlaySync();
        }
        catch (InvalidOperationException)
        {
            // The conversation remains usable if audio is unavailable on a device.
        }
    }
}
