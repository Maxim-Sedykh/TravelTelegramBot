namespace TravelTelegramBot.Models;

/// <summary>
/// Основной контейнер данных бота.
/// </summary>
public sealed class BotData
{
    public List<Category> Categories { get; set; } = new();

    public List<Place> Places { get; set; } = new();

    public List<BusRoute> Buses { get; set; } = new();

    public List<UserLog> Users { get; set; } = new();

    public List<long> AdminIds { get; set; } = new();
}
