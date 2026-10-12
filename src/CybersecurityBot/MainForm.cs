using System.Drawing.Drawing2D;

namespace CybersecurityBot;

internal sealed class MainForm : Form
{
    private readonly CybersecurityChatbot _chatbot = new();
    private readonly VoiceGreeting _voiceGreeting = new();
    private readonly FlowLayoutPanel _conversationPanel;
    private TextBox _messageInput = null!;
    private Button _sendButton = null!;
    private Label _statusLabel = null!;

    public MainForm()
    {
        Text = "Cybersecurity Awareness Bot";
        MinimumSize = new Size(980, 680);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(14, 24, 35);
        Font = new Font("Segoe UI", 10F);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(22),
            BackColor = BackColor
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 172));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 118));
        Controls.Add(root);

        root.Controls.Add(BuildHeader(), 0, 0);

        _conversationPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(4, 14, 4, 14),
            BackColor = Color.FromArgb(20, 35, 49)
        };
        root.Controls.Add(_conversationPanel, 0, 1);

        root.Controls.Add(BuildInputArea(), 0, 2);

        Shown += (_, _) =>
        {
            _voiceGreeting.PlayAsync();
            AddBotMessage(_chatbot.GetWelcomeMessage());
            _messageInput.Focus();
        };
    }

    private Control BuildHeader()
    {
        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            BackColor = Color.FromArgb(9, 42, 55),
            Padding = new Padding(22),
            Margin = new Padding(0, 0, 0, 14)
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 118));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));

        header.Controls.Add(new CyberBadgeControl { Dock = DockStyle.Fill }, 0, 0);

        var copy = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            BackColor = Color.Transparent,
            Padding = new Padding(18, 4, 8, 4)
        };
        copy.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        copy.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        copy.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        copy.Controls.Add(new Label
        {
            Text = "Cybersecurity Awareness Bot",
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 21F),
            AutoEllipsis = true
        }, 0, 0);

        copy.Controls.Add(new Label
        {
            Text = "Stay alert. Stay safe online.",
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(174, 230, 214),
            Font = new Font("Segoe UI", 12.5F),
            AutoEllipsis = true
        }, 0, 1);

        copy.Controls.Add(new Label
        {
            Text = "Ask about passwords, phishing, email scams, links, websites, and safer browsing habits.",
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(212, 224, 230),
            Font = new Font("Segoe UI", 10.5F),
            AutoEllipsis = true
        }, 0, 2);

        header.Controls.Add(copy, 1, 0);

        var replayButton = CreateCommandButton("Replay voice", Color.FromArgb(242, 185, 80));
        replayButton.Dock = DockStyle.Top;
        replayButton.Margin = new Padding(0, 18, 0, 0);
        replayButton.Click += (_, _) => _voiceGreeting.PlayAsync();
        header.Controls.Add(replayButton, 2, 0);

        return header;
    }

    private Control BuildInputArea()
    {
        var inputArea = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(0, 14, 0, 0),
            BackColor = BackColor
        };
        inputArea.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        inputArea.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        inputArea.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        inputArea.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var quickActions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent
        };
        AddQuickAction(quickActions, "Help", "help");
        AddQuickAction(quickActions, "Passwords", "How can I make my passwords safer?");
        AddQuickAction(quickActions, "Phishing", "How do I spot phishing emails?");
        AddQuickAction(quickActions, "Safe browsing", "How can I browse safely?");
        inputArea.Controls.Add(quickActions, 0, 0);

        _statusLabel = new Label
        {
            Text = "Ready",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = Color.FromArgb(169, 190, 202)
        };
        inputArea.Controls.Add(_statusLabel, 1, 0);

        _messageInput = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(235, 242, 245),
            ForeColor = Color.FromArgb(18, 30, 39),
            Font = new Font("Segoe UI", 11F),
            Margin = new Padding(0, 6, 12, 0),
            ScrollBars = ScrollBars.Vertical
        };
        _messageInput.KeyDown += MessageInput_KeyDown;
        inputArea.Controls.Add(_messageInput, 0, 1);

        _sendButton = CreateCommandButton("Send", Color.FromArgb(79, 205, 159));
        _sendButton.Dock = DockStyle.Fill;
        _sendButton.Margin = new Padding(0, 6, 0, 0);
        _sendButton.Click += (_, _) => SendCurrentMessage();
        inputArea.Controls.Add(_sendButton, 1, 1);

        return inputArea;
    }

    private void AddQuickAction(FlowLayoutPanel panel, string label, string prompt)
    {
        var button = CreateCommandButton(label, Color.FromArgb(47, 86, 106));
        button.AutoSize = true;
        button.MinimumSize = new Size(96, 30);
        button.Height = 30;
        button.Margin = new Padding(0, 0, 8, 0);
        button.Click += (_, _) =>
        {
            _messageInput.Text = prompt;
            SendCurrentMessage();
        };
        panel.Controls.Add(button);
    }

    private static Button CreateCommandButton(string text, Color backColor)
    {
        return new Button
        {
            Text = text,
            FlatStyle = FlatStyle.Flat,
            BackColor = backColor,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 10F),
            Cursor = Cursors.Hand
        };
    }

    private void MessageInput_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter && !e.Shift)
        {
            e.SuppressKeyPress = true;
            SendCurrentMessage();
        }
    }

    private void SendCurrentMessage()
    {
        string message = _messageInput.Text.Trim();

        if (string.IsNullOrWhiteSpace(message))
        {
            _statusLabel.Text = "Enter a message first";
            return;
        }

        _messageInput.Clear();
        AddUserMessage(message);

        if (!_chatbot.HasUserName)
        {
            _chatbot.SetUserName(message);
            AddBotMessage(_chatbot.GetIntroduction());
            _statusLabel.Text = $"Chatting with {_chatbot.UserName}";
            return;
        }

        string response = _chatbot.GetResponse(message);
        AddBotMessage(response);
        _statusLabel.Text = "Response added";
    }

    private void AddBotMessage(string message) => AddMessage("Bot", message, Color.FromArgb(35, 77, 71), Color.White, ContentAlignment.MiddleLeft);

    private void AddUserMessage(string message) => AddMessage("You", message, Color.FromArgb(224, 242, 239), Color.FromArgb(18, 30, 39), ContentAlignment.MiddleRight);

    private void AddMessage(string sender, string message, Color backColor, Color foreColor, ContentAlignment alignment)
    {
        int width = Math.Max(420, _conversationPanel.ClientSize.Width - 42);
        var bubble = new Label
        {
            AutoSize = false,
            Width = width,
            MinimumSize = new Size(width, 46),
            MaximumSize = new Size(width, 0),
            Padding = new Padding(16, 10, 16, 10),
            Margin = new Padding(8, 4, 8, 10),
            BackColor = backColor,
            ForeColor = foreColor,
            Font = new Font("Segoe UI", 10.5F),
            TextAlign = alignment,
            Text = $"{sender}: {message}"
        };

        Size preferredSize = TextRenderer.MeasureText(
            bubble.Text,
            bubble.Font,
            new Size(width - 32, 0),
            TextFormatFlags.WordBreak);
        bubble.Height = Math.Max(46, preferredSize.Height + 24);

        _conversationPanel.Controls.Add(bubble);
        _conversationPanel.ScrollControlIntoView(bubble);
    }

    private sealed class CyberBadgeControl : Control
    {
        public CyberBadgeControl()
        {
            DoubleBuffered = true;
            MinimumSize = new Size(96, 96);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = new Rectangle(8, 8, Width - 16, Height - 16);
            using var outerBrush = new SolidBrush(Color.FromArgb(79, 205, 159));
            using var innerBrush = new SolidBrush(Color.FromArgb(12, 53, 65));
            using var linePen = new Pen(Color.FromArgb(242, 185, 80), 5F);

            Point[] shield =
            {
                new(bounds.Left + bounds.Width / 2, bounds.Top),
                new(bounds.Right, bounds.Top + bounds.Height / 5),
                new(bounds.Right - 10, bounds.Top + bounds.Height * 3 / 5),
                new(bounds.Left + bounds.Width / 2, bounds.Bottom),
                new(bounds.Left + 10, bounds.Top + bounds.Height * 3 / 5),
                new(bounds.Left, bounds.Top + bounds.Height / 5)
            };

            e.Graphics.FillPolygon(outerBrush, shield);

            var inner = Rectangle.Inflate(bounds, -22, -22);
            e.Graphics.FillEllipse(innerBrush, inner);

            var shackle = new Rectangle(inner.Left + 18, inner.Top + 12, inner.Width - 36, inner.Height / 2);
            e.Graphics.DrawArc(linePen, shackle, 180, 180);
            e.Graphics.DrawLine(linePen, shackle.Left, shackle.Top + shackle.Height / 2, shackle.Left, shackle.Bottom);
            e.Graphics.DrawLine(linePen, shackle.Right, shackle.Top + shackle.Height / 2, shackle.Right, shackle.Bottom);

            var body = new Rectangle(inner.Left + 14, inner.Top + inner.Height / 2, inner.Width - 28, inner.Height / 3);
            using var bodyBrush = new SolidBrush(Color.FromArgb(242, 185, 80));
            using GraphicsPath bodyPath = CreateRoundedRectangle(body, 8);
            e.Graphics.FillPath(bodyBrush, bodyPath);
        }

        private static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            var path = new GraphicsPath();

            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
