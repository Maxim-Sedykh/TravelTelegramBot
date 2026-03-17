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

            Я помогу вам быстро найти расписание автобусов, информацию о заведениях и магазинах Макеевки.

            🔍 <b>Как пользоваться?</b>
            В сервисе есть функция быстрого поиска. Просто отправьте в чат номер маршрута или название места (например, «5А» или «Вектор»).

            👨‍💻 <b>Автор:</b> @maximka_se
            📩 <i>Заметили ошибку или хотите что-то добавить? Пишите мне в личные сообщения.</i>

            ☕️ <b>Поддержать проект:</b>
            Если бот вам полезен, вы можете помочь в его развитии:
            • Карта ПСБ: <code>2200 0305 4367 8297</code>
            • По номеру телефона: <code>+79493597126</code>
            """;

        await _bot.SendTextMessageAsync(
            chatId,
            welcomeText,
            parseMode: ParseMode.Html,
            replyMarkup: _keyboard.GetMainMenu(_storage.Data, isAdmin),
            cancellationToken: ct);
    }
}
