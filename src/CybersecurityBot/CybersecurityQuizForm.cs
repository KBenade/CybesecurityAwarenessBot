namespace CybersecurityBot;

internal sealed class CybersecurityQuizForm : Form
{
    private static readonly IReadOnlyList<QuizQuestion> Questions =
    [
        new("What should you do if an unexpected email asks for your password?", ["Reply with your password", "Report it as phishing", "Send a photo of your ID", "Forward it to friends"], 1, "Legitimate services should not ask you to send your password by email. Report the message through the service's official channel."),
        new("True or false: Reusing one password across many accounts is safe if it is very long.", ["True", "False"], 1, "If one site is breached, reused passwords can put your other accounts at risk. Use a unique password for every account."),
        new("Which link is safest to use when signing in to your bank?", ["A link in an unexpected text", "A link from a search advert", "The address you typed or saved yourself", "A shortened link from a stranger"], 2, "Use a known address or bookmark. This reduces the chance of landing on a convincing fake website."),
        new("True or false: A website padlock proves the business is trustworthy.", ["True", "False"], 1, "A padlock means the connection is encrypted. It does not prove the site itself is honest."),
        new("A caller asks for the one-time code sent to your phone. What should you do?", ["Share it to stop your account being locked", "Ask them to read it back", "Do not share it; contact the organisation using a trusted number", "Post it in the support chat"], 2, "One-time codes are like temporary passwords. A real support agent should not need you to disclose one."),
        new("True or false: You should install an attachment you were not expecting if it appears to come from a colleague.", ["True", "False"], 1, "Sender accounts can be spoofed or compromised. Check with the colleague using a separate, trusted channel first."),
        new("Which is a good way to protect an online account?", ["Use a unique password and enable multi-factor authentication", "Use your birthday as a password", "Share the password with a friend", "Turn off security alerts"], 0, "A unique password limits damage from other breaches, and multi-factor authentication adds another check at sign-in."),
        new("What is the safest response to a message demanding urgent payment or secrecy?", ["Pay immediately", "Pause and verify the request through an independently found contact", "Reply with personal details", "Open every attachment"], 1, "Urgency and secrecy are common pressure tactics. Verify the request through a trusted, separate channel."),
        new("True or false: Apps should only receive permissions they need for their features.", ["True", "False"], 0, "Review app permissions and turn off access to location, contacts, camera, or microphone when it is not needed."),
        new("What should you do before connecting to public Wi-Fi?", ["Turn off device updates", "Avoid sensitive activity unless using a trusted secure connection", "Share files with everyone nearby", "Disable your screen lock"], 1, "Public networks can be untrusted. Avoid sensitive transactions or use a trusted VPN and secure website connection."),
        new("A friend sends an unusual request for money from a new number. What should you do?", ["Send the money quickly", "Reply with your bank PIN", "Call your friend using a number you already know", "Forward the message to a group"], 2, "Impersonation scams often use urgency. Verify the person's identity through a known contact method before acting."),
        new("Which action helps keep your device safer?", ["Ignore all software updates", "Install updates from official sources", "Disable screen locking", "Use unknown download links"], 1, "Updates from official sources often fix security weaknesses. Avoid update links sent in unexpected messages.")
    ];

    private readonly Label _progressLabel = new();
    private readonly Label _questionLabel = new();
    private readonly Label _feedbackLabel = new();
    private readonly FlowLayoutPanel _answerPanel = new();
    private RadioButton[] _answerButtons = [];
    private readonly Button _actionButton;
    private int _questionIndex;
    private int _score;
    private bool _showingFeedback;
    private bool _showingFinalScore;

    public CybersecurityQuizForm()
    {
        Text = "Cybersecurity Mini-Game";
        MinimumSize = new Size(650, 500);
        Size = new Size(760, 580);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.FromArgb(14, 24, 35);
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 10F);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(24),
            BackColor = BackColor
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 110));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 110));
        Controls.Add(root);

        _progressLabel.Dock = DockStyle.Fill;
        _progressLabel.Font = new Font("Segoe UI Semibold", 11F);
        _progressLabel.ForeColor = Color.FromArgb(174, 230, 214);
        root.Controls.Add(_progressLabel, 0, 0);

        _questionLabel.Dock = DockStyle.Fill;
        _questionLabel.Font = new Font("Segoe UI Semibold", 15F);
        _questionLabel.Padding = new Padding(0, 10, 0, 8);
        root.Controls.Add(_questionLabel, 0, 1);

        _answerPanel.Dock = DockStyle.Fill;
        _answerPanel.FlowDirection = FlowDirection.TopDown;
        _answerPanel.WrapContents = false;
        _answerPanel.BackColor = Color.FromArgb(20, 35, 49);
        _answerPanel.Padding = new Padding(14);
        root.Controls.Add(_answerPanel, 0, 2);

        var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(0, 10, 0, 0) };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        _feedbackLabel.Dock = DockStyle.Fill;
        _feedbackLabel.ForeColor = Color.FromArgb(212, 224, 230);
        _feedbackLabel.Padding = new Padding(0, 8, 12, 0);
        footer.Controls.Add(_feedbackLabel, 0, 0);
        _actionButton = new Button
        {
            Text = "Submit answer",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(79, 205, 159),
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 10F),
            Cursor = Cursors.Hand
        };
        _actionButton.Click += (_, _) => HandleAction();
        footer.Controls.Add(_actionButton, 1, 0);
        root.Controls.Add(footer, 0, 3);

        _answerButtons = [];
        ShowQuestion();
    }

    private void ShowQuestion()
    {
        _showingFeedback = false;
        _feedbackLabel.Text = string.Empty;
        _progressLabel.Text = $"Question {_questionIndex + 1} of {Questions.Count}   •   Score: {_score}";
        QuizQuestion question = Questions[_questionIndex];
        _questionLabel.Text = question.Prompt;
        _answerPanel.Controls.Clear();

        _answerButtons = new RadioButton[question.Options.Count];
        for (int i = 0; i < question.Options.Count; i++)
        {
            var option = new RadioButton
            {
                Text = question.Options[i],
                AutoSize = false,
                Width = Math.Max(400, _answerPanel.ClientSize.Width - 36),
                Height = 46,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F),
                Margin = new Padding(4, 3, 4, 5),
                Tag = i
            };
            _answerButtons[i] = option;
            _answerPanel.Controls.Add(option);
        }

        _actionButton.Text = "Submit answer";
        _actionButton.Enabled = true;
    }

    private void HandleAction()
    {
        if (_showingFinalScore)
        {
            _questionIndex = 0;
            _score = 0;
            _showingFinalScore = false;
            ShowQuestion();
            return;
        }

        if (_showingFeedback)
        {
            if (_questionIndex < Questions.Count - 1)
            {
                _questionIndex++;
                ShowQuestion();
            }
            else
            {
                ShowFinalScore();
            }

            return;
        }

        RadioButton? selected = _answerButtons.FirstOrDefault(button => button.Checked);
        if (selected?.Tag is not int selectedIndex)
        {
            _feedbackLabel.Text = "Choose an answer before continuing.";
            _feedbackLabel.ForeColor = Color.FromArgb(255, 193, 120);
            return;
        }

        QuizQuestion question = Questions[_questionIndex];
        bool correct = selectedIndex == question.CorrectOptionIndex;
        if (correct) _score++;

        foreach (RadioButton answer in _answerButtons) answer.Enabled = false;
        _feedbackLabel.Text = $"{(correct ? "Correct!" : "Not quite.")} {question.Explanation}";
        _feedbackLabel.ForeColor = correct ? Color.FromArgb(142, 225, 177) : Color.FromArgb(255, 193, 150);
        _progressLabel.Text = $"Question {_questionIndex + 1} of {Questions.Count}   •   Score: {_score}";
        _actionButton.Text = _questionIndex == Questions.Count - 1 ? "See final score" : "Next question";
        _showingFeedback = true;
    }

    private void ShowFinalScore()
    {
        _questionLabel.Text = $"Quiz complete! You scored {_score} out of {Questions.Count}.";
        _progressLabel.Text = "Final score";
        _answerPanel.Controls.Clear();

        string message = _score switch
        {
            >= 11 => "Great job! You have strong cybersecurity habits. Keep them up.",
            >= 8 => "Well done! Review the explanations for a few useful reminders.",
            >= 5 => "Good start. Keep learning and practise these safety habits.",
            _ => "Keep learning to stay safer online. Try the quiz again after reviewing the tips."
        };

        _feedbackLabel.Text = message;
        _feedbackLabel.ForeColor = Color.FromArgb(174, 230, 214);
        _actionButton.Text = "Play again";
        _showingFinalScore = true;
    }

    private sealed record QuizQuestion(
        string Prompt,
        IReadOnlyList<string> Options,
        int CorrectOptionIndex,
        string Explanation);
}
