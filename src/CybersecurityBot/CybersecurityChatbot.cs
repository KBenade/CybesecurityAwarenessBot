namespace CybersecurityBot;

/// <summary>Manages user input, validation, and cybersecurity responses.</summary>
internal sealed class CybersecurityChatbot
{
    private readonly ConsoleUi _consoleUi;
    private readonly ChatbotProfile _profile = new();

    public CybersecurityChatbot(ConsoleUi consoleUi)
    {
        _consoleUi = consoleUi;
    }

    public void Run()
    {
        _consoleUi.ShowSection("Welcome");
        _consoleUi.WriteBotMessage("Hello! I am your Cybersecurity Awareness Bot. I am here to help you stay safe online.");
        _profile.UserName = AskForName();
        _consoleUi.WriteBotMessage($"It is great to meet you, {_profile.UserName}!");
        _consoleUi.WriteBotMessage("Ask about passwords, phishing, or safe browsing. Type 'help' for examples or 'exit' to leave.");

        while (true)
        {
            _consoleUi.WritePrompt(string.Empty);
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                _consoleUi.WriteError("Please enter a question or type 'help'.");
                continue;
            }

            string response = GetResponse(input);
            if (response == "__EXIT__")
            {
                _consoleUi.WriteBotMessage($"Goodbye, {_profile.UserName}. Remember: think before you click!");
                return;
            }

            _consoleUi.WriteBotMessage(response);
        }
    }

    private string AskForName()
    {
        while (true)
        {
            _consoleUi.WritePrompt("What is your name? ");
            string? name = Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            _consoleUi.WriteError("Your name cannot be empty. Please try again.");
        }
    }

    private string GetResponse(string input)
    {
        string question = input.Trim().ToLowerInvariant();

        if (question is "exit" or "quit" or "bye")
        {
            return "__EXIT__";
        }

        if (question.Contains("how are you", StringComparison.Ordinal))
        {
            return "I am operating safely and ready to help you learn about cybersecurity.";
        }

        if (question.Contains("purpose", StringComparison.Ordinal) || question.Contains("what can i ask", StringComparison.Ordinal) || question == "help")
        {
            return "My purpose is to raise cybersecurity awareness. You can ask me about password safety, phishing emails, or safe browsing.";
        }

        if (question.Contains("password", StringComparison.Ordinal))
        {
            return "Use a different long passphrase for every account. A password manager can create and store strong passwords, and you should enable multi-factor authentication.";
        }

        if (question.Contains("phishing", StringComparison.Ordinal) || question.Contains("email", StringComparison.Ordinal) || question.Contains("scam", StringComparison.Ordinal))
        {
            return "Treat unexpected messages carefully. Check the sender and URL, do not open suspicious links or attachments, and verify requests through an official contact method.";
        }

        if (question.Contains("browse", StringComparison.Ordinal) || question.Contains("website", StringComparison.Ordinal) || question.Contains("link", StringComparison.Ordinal))
        {
            return "Browse safely by checking that a site's address is correct, avoiding unknown downloads, keeping software updated, and never entering sensitive details on a suspicious website.";
        }

        _consoleUi.WriteWarning("Unsupported question received.");
        return "I did not quite understand that. Could you rephrase, or ask about passwords, phishing, or safe browsing?";
    }
}
