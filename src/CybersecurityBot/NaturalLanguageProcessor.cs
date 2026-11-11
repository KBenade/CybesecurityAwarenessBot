using System.Globalization;
using System.Text.RegularExpressions;

namespace CybersecurityBot;

internal sealed class NaturalLanguageProcessor
{
    private static readonly Regex RequestPrefix = new(
        @"^(?:(?:please|could you|can you|would you|i want you to)\s+)*(?:(?:remind me to|remind me about|remind me(?:\s+(?:today|tomorrow|tonight))?\s+to|set (?:a )?reminder(?: to| for)?|add (?:a )?task(?: to| about|\s*[-:]|\s+for)?|create (?:a )?task(?: to| about|\s*[-:]|\s+for)?|make (?:a )?task(?: to| about)?|remember to|i need to|i should)\s*)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex DatePhrase = new(
        @"\b(?:today|tomorrow|tonight|next week|in\s+\d+\s+(?:days?|weeks?)|on\s+\d{1,2}(?:st|nd|rd|th)?(?:\s+[a-z]+)?(?:\s+\d{4})?)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public bool IsQuizRequest(string input)
    {
        string text = NormalizeInput(input);
        return text.Contains("quiz", StringComparison.Ordinal)
            || text.Contains("mini game", StringComparison.Ordinal)
            || text.Contains("test my knowledge", StringComparison.Ordinal)
            || text.Contains("challenge me", StringComparison.Ordinal)
            || text.Contains("cybersecurity game", StringComparison.Ordinal);
    }

    public bool IsActivitySummaryRequest(string input)
    {
        string text = NormalizeInput(input);
        return text.Contains("what have you done", StringComparison.Ordinal)
            || text.Contains("what did you do", StringComparison.Ordinal)
            || text.Contains("recent actions", StringComparison.Ordinal)
            || text.Contains("actions so far", StringComparison.Ordinal)
            || text.Contains("activity summary", StringComparison.Ordinal);
    }

    public bool TryParseTaskRequest(string input, out ParsedTaskRequest? request)
    {
        request = null;
        string text = input.Trim();
        string normalized = NormalizeInput(text);
        bool isTaskRequest = (normalized.Contains("task", StringComparison.Ordinal)
                && ContainsAny(normalized, "add", "create", "make", "new", "remind", "set"))
            || normalized.StartsWith("remind me", StringComparison.Ordinal)
            || normalized.StartsWith("set a reminder", StringComparison.Ordinal)
            || normalized.StartsWith("set reminder", StringComparison.Ordinal)
            || normalized.Contains("remind me", StringComparison.Ordinal)
            || normalized.Contains("set a reminder", StringComparison.Ordinal)
            || normalized.Contains("set reminder", StringComparison.Ordinal)
            || normalized.StartsWith("remember to", StringComparison.Ordinal);

        if (normalized.StartsWith("how do i", StringComparison.Ordinal)
            || normalized.StartsWith("what is", StringComparison.Ordinal)
            || normalized.StartsWith("explain", StringComparison.Ordinal))
        {
            isTaskRequest = false;
        }

        if (!isTaskRequest)
        {
            return false;
        }

        DateTime? reminderAt = TryParseReminderDate(text, out DateTime date) ? date : null;
        string title = RequestPrefix.Replace(text, string.Empty, count: 1).Trim();
        title = DatePhrase.Replace(title, string.Empty).Trim(' ', '.', ',', '!', '?', ':', ';', '-');

        if (title.Length == 0)
        {
            return false;
        }

        title = char.ToUpperInvariant(title[0]) + title[1..];
        if (title.Length > 200) title = title[..200].TrimEnd();
        request = new ParsedTaskRequest(title, reminderAt);
        return true;
    }

    public static bool TryParseReminderDate(string input, out DateTime reminderAt)
    {
        string text = NormalizeInput(input);
        DateTime today = DateTime.Today;

        if (text.Contains("tomorrow", StringComparison.Ordinal))
        {
            reminderAt = today.AddDays(1).AddHours(9);
            return true;
        }

        if (text.Contains("tonight", StringComparison.Ordinal))
        {
            reminderAt = today.AddHours(20);
            if (reminderAt <= DateTime.Now) reminderAt = today.AddDays(1).AddHours(20);
            return true;
        }

        if (text.Contains("today", StringComparison.Ordinal))
        {
            reminderAt = today.AddHours(17);
            if (reminderAt <= DateTime.Now) reminderAt = today.AddDays(1).AddHours(9);
            return true;
        }

        Match relativeDate = Regex.Match(text, @"\bin\s+(\d+)\s+(day|days|week|weeks)\b", RegexOptions.CultureInvariant);
        if (relativeDate.Success && int.TryParse(relativeDate.Groups[1].Value, out int amount))
        {
            int days = relativeDate.Groups[2].Value.StartsWith("week", StringComparison.Ordinal) ? amount * 7 : amount;
            reminderAt = today.AddDays(days).AddHours(9);
            return reminderAt > DateTime.Now;
        }

        if (text.Contains("next week", StringComparison.Ordinal))
        {
            reminderAt = today.AddDays(7).AddHours(9);
            return true;
        }

        string dateText = Regex.Replace(text, @"^(?:on|at)\s+", string.Empty, RegexOptions.CultureInvariant);
        if (DateTime.TryParse(dateText, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime parsed)
            && parsed > DateTime.Now)
        {
            reminderAt = parsed;
            return true;
        }

        reminderAt = default;
        return false;
    }

    internal static string NormalizeInput(string input)
    {
        string text = input.Trim().ToLowerInvariant();
        text = Regex.Replace(text, @"[^\p{L}\p{N}\s'-]", " ", RegexOptions.CultureInvariant);
        text = Regex.Replace(text, @"\s+", " ", RegexOptions.CultureInvariant);
        return text.Replace("two factor authentication", "two-factor authentication", StringComparison.Ordinal)
            .Replace("two-factor auth", "two-factor authentication", StringComparison.Ordinal)
            .Replace("2fa", "two-factor authentication", StringComparison.Ordinal)
            .Replace("mfa", "multi-factor authentication", StringComparison.Ordinal)
            .Replace("wi fi", "wifi", StringComparison.Ordinal)
            .Replace("e mail", "email", StringComparison.Ordinal);
    }

    private static bool ContainsAny(string text, params string[] terms) =>
        terms.Any(term => text.Contains(term, StringComparison.Ordinal));
}

internal sealed record ParsedTaskRequest(string Title, DateTime? ReminderAt);
