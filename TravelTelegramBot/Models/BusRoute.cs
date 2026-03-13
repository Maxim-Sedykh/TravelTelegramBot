namespace TravelTelegramBot.Models;

/// <summary>
/// Маршрут автобуса с расписанием.
/// </summary>
public sealed class BusRoute
{
    public string Number { get; set; } = string.Empty;

    public string Name => $"{StationA} - {StationB}";

    public string StationA { get; set; } = string.Empty;

    public string StationB { get; set; } = string.Empty;

    public List<string> ScheduleForward { get; set; } = new();

    public List<string> ScheduleBackward { get; set; } = new();

    public List<string> HolidayForward { get; set; } = new();

    public List<string> HolidayBackward { get; set; } = new();
}

