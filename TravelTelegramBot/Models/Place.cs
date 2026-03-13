namespace TravelTelegramBot.Models;

/// <summary>
/// Информация о заведении или месте.
/// </summary>
public sealed class Place
{
    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string WorkTime { get; set; } = string.Empty;

    public string Break { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Contact { get; set; } = string.Empty;

    public string HolidayWorkTime { get; set; } = string.Empty;

    public string HolidayCommentary { get; set; } = string.Empty;

    public string Commentary { get; set; } = string.Empty;
}