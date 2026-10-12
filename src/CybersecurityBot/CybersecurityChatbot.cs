namespace CybersecurityBot;

/// <summary>Manages user input, validation, and cybersecurity responses.</summary>
internal sealed class CybersecurityChatbot
{
    private readonly ChatbotProfile _profile = new();
    private readonly List<KeywordResponse> _keywordResponses =
    [
        new(
            ["password", "passphrase", "login", "credential"],
            "Password safety",
            "Use a different long passphrase for every account. A password manager can create and store strong passwords, and you should enable multi-factor authentication."),
        new(
            ["scam", "fraud", "fake", "con", "spoof"],
            "Scam awareness",
            "Scam messages often create urgency or ask for money, codes, or personal details. Pause, check the source, and confirm the request through an official website or phone number."),
        new(
            ["privacy", "personal information", "data", "tracking", "location"],
            "Privacy protection",
            "Protect your privacy by sharing less personal information online, reviewing app permissions, limiting location access, and using strong privacy settings on social media accounts."),
        new(
            ["phishing", "email", "attachment"],
            "Phishing emails",
            "Treat unexpected messages carefully. Check the sender and URL, do not open suspicious links or attachments, and verify requests through an official contact method."),
        new(
            ["browse", "website", "link", "browsing", "download"],
            "Safe browsing",
            "Browse safely by checking that a site's address is correct, avoiding unknown downloads, keeping software updated, and never entering sensitive details on a suspicious website.")
    ];

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
        return $"It is great to meet you, {_profile.UserName}! Ask about passwords, scams, privacy, phishing, or safe browsing. Type 'help' for examples or 'exit' to leave.";
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
            return "My purpose is to raise cybersecurity awareness. You can ask me about password safety, scams, privacy, phishing emails, or safe browsing.";
        }

        foreach (KeywordResponse keywordResponse in _keywordResponses)
        {
            if (keywordResponse.Matches(question))
            {
                return $"{keywordResponse.Topic}: {keywordResponse.Response}";
            }
        }

        return "I did not quite understand that. Could you rephrase, or ask about passwords, scams, privacy, phishing, or safe browsing?";
    }

    private sealed record KeywordResponse(IReadOnlyList<string> Keywords, string Topic, string Response)
    {
        public bool Matches(string question)
        {
            return Keywords.Any(keyword => question.Contains(keyword, StringComparison.Ordinal));
        }
    }
}
