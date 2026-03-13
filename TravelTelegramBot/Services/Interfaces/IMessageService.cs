using TravelTelegramBot.Models;

namespace TravelTelegramBot.Services.Interfaces;

/// <summary>
/// Сервис для отображения сущностей пользователю
/// </summary>
public interface IMessageService
{
    /// <summary>
    /// Вывести место в виде HTML отформатированного текста пользователю
    /// </summary>
    /// <param name="p">Место</param>
    /// <returns>Строка с HTML</returns>
    string PreparePlaceHtml(Place p);

    /// <summary>
    /// Вывести маршрут в виде HTML отформатированного текста пользователю
    /// </summary>
    /// <param name="p">Маршрут</param>
    /// <returns>Строка с HTML</returns>
    string PrepareBusScheduleHtml(BusRoute bus);
}
