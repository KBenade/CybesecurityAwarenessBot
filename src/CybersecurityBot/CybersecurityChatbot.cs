namespace CybersecurityBot;

/// <summary>Manages user input, validation, and cybersecurity responses.</summary>
internal sealed class CybersecurityChatbot
{
    private readonly ChatbotProfile _profile = new();

    public string UserName => _profile.UserName;

    public bool HasUserName { get; private set; }

    public void SetUserName(string name)
    {
        string cleanName = name.Trim();
        _profile.UserName = string.IsNullOrWhiteSpace(cleanName) ? "friend" : cleanName;
        HasUserName = true;
    }

    public string GetWelcomeMessage()
    {
        return "Hello! I am your Cybersecurity Awareness Bot. I am here to help you stay safe online. What is your name?";
    }

    public string GetIntroduction()
    {
        return $"It is great to meet you, {_profile.UserName}! Ask about passwords, phishing, or safe browsing. Type 'help' for examples or 'exit' to leave.";
    }

    public string GetResponse(string input)
    {
        string question = input.Trim().ToLowerInvariant();

        if (question is "exit" or "quit" or "bye")
        {
            return $"Goodbye, {_profile.UserName}. Remember: think before you click!";
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

        if (question.Contains("browse", StringComparison.Ordinal) || question.Contains("website", StringComparison.Ordinal) || question.Contains("link", StringComparison.Ordinal) || question.Contains("browsing", StringComparison.Ordinal))
        {
            return "Browse safely by checking that a site's address is correct, avoiding unknown downloads, keeping software updated, and never entering sensitive details on a suspicious website.";
        }

        return "I did not quite understand that. Could you rephrase, or ask about passwords, phishing, or safe browsing?";
    }
}
