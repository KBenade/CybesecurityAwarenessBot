namespace CybersecurityBot;

/// <summary>Manages user input, validation, and cybersecurity responses.</summary>
internal sealed class CybersecurityChatbot
{
    private readonly ChatbotProfile _profile = new();
    private KeywordResponse? _currentTopic;
    private int? _lastResponseIndex;
    private readonly List<KeywordResponse> _keywordResponses =
    [
        new(
            ["password", "passphrase", "login", "credential"],
            "Password safety",
            [
                "Use a different long passphrase for every account. A password manager can create and store strong passwords, and you should enable multi-factor authentication.",
                "Make each password long and unique. A password manager helps you avoid reusing passwords, and multi-factor authentication adds another layer of protection."
            ]),
        new(
            ["scam", "fraud", "fake", "con", "spoof"],
            "Scam awareness",
            [
                "Scam messages often create urgency or ask for money, codes, or personal details. Pause, check the source, and confirm the request through an official website or phone number.",
                "Be wary of unexpected requests for payment, verification codes, or personal information. Contact the organisation using details you find independently."
            ]),
        new(
            ["privacy", "personal information", "data", "tracking", "location"],
            "Privacy protection",
            [
                "Protect your privacy by sharing less personal information online, reviewing app permissions, limiting location access, and using strong privacy settings on social media accounts.",
                "Review what personal details you share and which apps can access your location, camera, and contacts. Turn off permissions an app does not need."
            ]),
        new(
            ["phishing", "email", "attachment"],
            "Phishing emails",
            [
                "Treat unexpected messages carefully. Check the sender and URL, do not open suspicious links or attachments, and verify requests through an official contact method.",
                "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations, so verify the sender and contact the organisation directly.",
                "Hover over links to inspect their real destination before opening them. If a message pressures you to act quickly, verify it through a separate, trusted channel.",
                "Unexpected attachments can contain malware. Confirm with the sender using a known contact method before opening an attachment you were not expecting."
            ]),
        new(
            ["browse", "website", "link", "browsing", "download"],
            "Safe browsing",
            [
                "Browse safely by checking that a site's address is correct, avoiding unknown downloads, keeping software updated, and never entering sensitive details on a suspicious website.",
                "Before entering sensitive information, check the website address carefully. Avoid unexpected downloads and keep your browser and device updated."
            ])
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

        string? sentimentResponse = GetSentimentResponse(question);
        if (sentimentResponse is not null)
        {
            TryRememberFavouriteTopic(question, out _);
            KeywordResponse? mentionedTopic = _keywordResponses.FirstOrDefault(response => response.Matches(question));
            KeywordResponse topic = mentionedTopic ?? _currentTopic ?? _keywordResponses[0];
            _currentTopic = topic;
            string tip = GetTopicResponse(topic, avoidRepeat: mentionedTopic is null);
            return $"{sentimentResponse} {tip}";
        }

        if (TryRememberFavouriteTopic(question, out string? favouriteTopic))
        {
            return $"Great! I will remember that you are interested in {favouriteTopic}. It is an important part of staying safe online.";
        }

        if (IsFollowUp(question))
        {
            if (_currentTopic is null)
            {
                return "I can explain more about passwords, scams, privacy, phishing, or safe browsing. Which topic would you like to explore?";
            }

            return GetTopicResponse(_currentTopic, avoidRepeat: true);
        }

        foreach (KeywordResponse keywordResponse in _keywordResponses)
        {
            if (keywordResponse.Matches(question))
            {
                _currentTopic = keywordResponse;
                return GetTopicResponse(keywordResponse, avoidRepeat: false);
            }
        }

        return "I did not quite understand that. Could you rephrase, or ask about passwords, scams, privacy, phishing, or safe browsing?";
    }

    private string GetTopicResponse(KeywordResponse topic, bool avoidRepeat)
    {
        int responseIndex;
        if (avoidRepeat && topic.Responses.Count > 1 && _lastResponseIndex is int previousIndex)
        {
            responseIndex = Random.Shared.Next(topic.Responses.Count - 1);
            if (responseIndex >= previousIndex)
            {
                responseIndex++;
            }
        }
        else
        {
            responseIndex = Random.Shared.Next(topic.Responses.Count);
        }

        _lastResponseIndex = responseIndex;
        string response = topic.Responses[responseIndex];
        if (string.Equals(_profile.FavouriteTopic, topic.Topic, StringComparison.Ordinal))
        {
            return $"As someone interested in {topic.Topic.ToLowerInvariant()}, {char.ToLowerInvariant(response[0])}{response[1..]}";
        }

        return $"{topic.Topic}: {response}";
    }

    private bool TryRememberFavouriteTopic(string question, out string? favouriteTopic)
    {
        favouriteTopic = null;
        bool expressesInterest = question.Contains("interested in", StringComparison.Ordinal)
            || question.Contains("favourite topic", StringComparison.Ordinal)
            || question.Contains("favorite topic", StringComparison.Ordinal)
            || question.Contains("i like", StringComparison.Ordinal);

        if (!expressesInterest)
        {
            return false;
        }

        KeywordResponse? topic = _keywordResponses.FirstOrDefault(response => response.Matches(question));
        if (topic is null)
        {
            return false;
        }

        _profile.FavouriteTopic = topic.Topic;
        _currentTopic = topic;
        _lastResponseIndex = null;
        favouriteTopic = topic.Topic.ToLowerInvariant();
        return true;
    }

    private static string? GetSentimentResponse(string question)
    {
        if (ContainsAny(question, "worried", "scared", "anxious", "afraid", "nervous"))
        {
            return "It is completely understandable to feel worried. Scams and online threats can be convincing, but a few careful steps can help you stay safe. Here is a tip:";
        }

        if (ContainsAny(question, "frustrated", "annoyed", "upset", "angry"))
        {
            return "I understand that this can be frustrating. We can take it one step at a time. Here is a practical tip:";
        }

        if (ContainsAny(question, "overwhelmed", "confused", "unsure", "uncertain", "don't understand", "do not understand"))
        {
            return "That can feel like a lot to deal with, but you do not have to solve everything at once. Here is one simple tip:";
        }

        if (ContainsAny(question, "curious", "interested", "wondering"))
        {
            return "Great question. Learning about online safety is a useful way to protect yourself. Here is a tip:";
        }

        return null;
    }

    private static bool ContainsAny(string text, params string[] phrases)
    {
        return phrases.Any(phrase => text.Contains(phrase, StringComparison.Ordinal));
    }

    private static bool IsFollowUp(string question)
    {
        string[] followUpPhrases =
        [
            "another tip", "another one", "more tips", "tell me more", "explain more",
            "more details", "give me more", "elaborate", "i am confused", "i'm confused",
            "i do not understand", "i don't understand", "can you explain", "what do you mean"
        ];

        return followUpPhrases.Any(phrase => question.Contains(phrase, StringComparison.Ordinal));
    }

    private sealed record KeywordResponse(IReadOnlyList<string> Keywords, string Topic, IReadOnlyList<string> Responses)
    {
        public bool Matches(string question)
        {
            return Keywords.Any(keyword => question.Contains(keyword, StringComparison.Ordinal));
        }
    }
}
