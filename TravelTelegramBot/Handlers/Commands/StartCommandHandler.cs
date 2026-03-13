using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TravelTelegramBot.Handlers.Interfaces;
using TravelTelegramBot.Services.Interfaces;

namespace TravelTelegramBot.Handlers.Commands;

/// <summary>
/// Обработчик для команды /start
/// Главное меню бота
/// </summary>
public sealed class StartCommandHandler : ICommandHandler
{
    /// <inheritdoc/>
    public string CommandName => "/start";

    private readonly ITelegramBotClient _bot;
    private readonly IStorageService _storage;
    private readonly IKeyboardService _keyboard;

    public StartCommandHandler(ITelegramBotClient bot, IStorageService storage, IKeyboardService keyboard)
    {
        _bot = bot;
        _storage = storage;
        _keyboard = keyboard;
    }

    /// <inheritdoc/>
    public bool CanHandle(string text) => text == CommandName;

    /// <inheritdoc/>
    public async Task ExecuteAsync(long chatId, Message message, CancellationToken ct)
    {
        bool isAdmin = _storage.Data.AdminIds.Contains(chatId);

        string welcomeText = $"""
            👋 <b>Добро пожаловать в MakeevkaTravelBot!</b>

            Здесь можно узнать расписание автобусов и информацию о заведениях.
            Введите название места или номер автобуса для быстрого поиска.
            Введите название магазина или маршрута для быстрого поиска.

            👨‍💻 <b>Автор бота:</b> @maximka_se
            📩 <i>Если нужно что-то добавить, удалить или вы нашли ошибку — пишите мне.</i>
            """;

        await _bot.SendTextMessageAsync(
            chatId,
            welcomeText,
            parseMode: ParseMode.Html,
            replyMarkup: _keyboard.GetMainMenu(_storage.Data, isAdmin),
            cancellationToken: ct);
    }
}
