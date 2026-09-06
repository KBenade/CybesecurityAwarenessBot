namespace CybersecurityBot;

internal static class Program
{
    private static void Main()
    {
        Console.Title = "Cybersecurity Awareness Bot";
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var consoleUi = new ConsoleUi();
        var voiceGreeting = new VoiceGreeting();
        var chatbot = new CybersecurityChatbot(consoleUi);

        consoleUi.ShowBanner();
        voiceGreeting.Play();
        chatbot.Run();
    }
}
