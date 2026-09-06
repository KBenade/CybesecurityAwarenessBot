namespace CybersecurityBot;

/// <summary>Contains the colour and layout rules for the console interface.</summary>
internal sealed class ConsoleUi
{
    private const string Divider = "==================================================================================";

    public void ShowBanner()
    {
        try
        {
            Console.Clear();
        }
        catch (IOException)
        {
            // Console.Clear is not supported when output is redirected.
        }

        string logo = @"                                                                               
                                                                               
  ▄█████ ▄▄ ▄▄ ▄▄▄▄  ▄▄▄▄▄ ▄▄▄▄   ▄▄▄▄ ▄▄▄▄▄  ▄▄▄▄ ▄▄ ▄▄ ▄▄▄▄  ▄▄ ▄▄▄▄▄▄ ▄▄ ▄▄ 
  ██     ▀███▀ ██▄██ ██▄▄  ██▄█▄ ███▄▄ ██▄▄  ██▀▀▀ ██ ██ ██▄█▄ ██   ██   ▀███▀ 
  ▀█████   █   ██▄█▀ ██▄▄▄ ██ ██ ▄▄██▀ ██▄▄▄ ▀████ ▀███▀ ██ ██ ██   ██     █   
                                                                               
                                                                               
                                                                               
▄████▄ ▄▄   ▄▄  ▄▄▄  ▄▄▄▄  ▄▄▄▄▄ ▄▄  ▄▄ ▄▄▄▄▄  ▄▄▄▄  ▄▄▄▄   █████▄  ▄▄▄ ▄▄▄▄▄▄ 
██▄▄██ ██ ▄ ██ ██▀██ ██▄█▄ ██▄▄  ███▄██ ██▄▄  ███▄▄ ███▄▄   ██▄▄██ ██▀██  ██   
██  ██  ▀█▀█▀  ██▀██ ██ ██ ██▄▄▄ ██ ▀██ ██▄▄▄ ▄▄██▀ ▄▄██▀   ██▄▄█▀ ▀███▀  ██   
                                                                               ";
        WriteLine(Divider, ConsoleColor.DarkCyan);
        WriteLine("          Stay alert. Stay safe online.", ConsoleColor.Gray);
        WriteLine(Divider, ConsoleColor.DarkCyan);
        Console.WriteLine(logo);
        WriteLine(Divider, ConsoleColor.DarkCyan);
        Console.WriteLine();
    }

    public void ShowSection(string heading)
    {
        Console.WriteLine();
        WriteLine($"--- {heading} ---", ConsoleColor.Cyan);
    }

    public void WriteBotMessage(string message) => WriteLine($"Bot: {message}", ConsoleColor.Green);

    public void WriteWarning(string message) => WriteLine($"! {message}", ConsoleColor.Yellow);

    public void WriteError(string message) => WriteLine($"! {message}", ConsoleColor.Red);

    public void WritePrompt(string prompt)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"You: {prompt}");
        Console.ResetColor();
    }

    private static void WriteLine(string text, ConsoleColor colour)
    {
        Console.ForegroundColor = colour;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}
