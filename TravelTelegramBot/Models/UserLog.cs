namespace TravelTelegramBot.Models;

/// <summary>
/// Данные о пользователе для статистики.
/// </summary>
public sealed class UserLog
{
    public long UserId { get; set; }

    public string? FirstName { get; set; } = string.Empty;

    public string? Username { get; set; } = string.Empty;

    public DateTime LastActive { get; set; }
}