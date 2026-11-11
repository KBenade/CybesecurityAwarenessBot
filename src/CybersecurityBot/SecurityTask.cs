namespace CybersecurityBot;

internal sealed record SecurityTask(
    int Id,
    string Title,
    string Description,
    DateTime? ReminderAt,
    bool IsCompleted);
