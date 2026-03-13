using Telegram.Bot.Types;

namespace TravelTelegramBot.Services.Interfaces;

/// <summary>
/// Сервис для взаимодействия с пользователями бота
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Записать лог о действии пользователя
    /// </summary>
    /// <param name="message">Сообщение пользователя</param>
    void UpdateLog(Message message);

    /// <summary>
    /// Является ли пользователь админом
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <returns>true - является</returns>
    bool IsAdmin(long userId);
}
