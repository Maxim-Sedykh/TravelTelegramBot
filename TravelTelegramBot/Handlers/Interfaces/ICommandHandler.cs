using Telegram.Bot.Types;

namespace TravelTelegramBot.Handlers.Interfaces;

/// <summary>
/// Интерфейс для обработчика сообщений в виде команды
/// </summary>
public interface ICommandHandler
{
    /// <summary>
    /// Команда пользователя которая соответствует обработчику
    /// </summary>
    string CommandName { get; }

    /// <summary>
    /// Соответствует ли текст обработчику, может ли он делать работу.
    /// </summary>
    /// <param name="text">Текст пользователя</param>
    /// <returns>true -> да, false -> нет</returns>
    bool CanHandle(string text);

    /// <summary>
    /// Обработать сообщение пользователя
    /// </summary>
    /// <param name="chatId">Id пользователя</param>
    /// <param name="message">Сообщение пользователя</param>
    /// <param name="ct">Токен отмены операции</param>
    Task ExecuteAsync(long chatId, Message message, CancellationToken ct);
}
