using Telegram.Bot.Types.ReplyMarkups;
using TravelTelegramBot.Models;

namespace TravelTelegramBot.Services.Interfaces;

/// <summary>
/// Сервис для отображения интерфейса пользователю
/// </summary>
public interface IKeyboardService
{
    /// <summary>
    /// Получить главное меню
    /// </summary>
    /// <param name="data">Данные бота</param>
    /// <param name="isAdmin">Является ли бот админом</param>
    ReplyKeyboardMarkup GetMainMenu(BotData data, bool isAdmin);

    /// <summary>
    /// Получить меню админа
    /// </summary>
    ReplyKeyboardMarkup GetAdminMenu();

    /// <summary>
    /// Получить кнопку "Назад"
    /// </summary>
    /// <returns></returns>
    ReplyKeyboardMarkup GetBackKb();

    /// <summary>
    /// Получить меню элементов
    /// </summary>
    /// <param name="items">Элементы</param>
    /// <param name="extraButton">Дополнительные кнопки</param>
    ReplyKeyboardMarkup GetSelectionMenu(IEnumerable<string> items, string? extraButton = null);
}
