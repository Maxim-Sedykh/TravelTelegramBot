using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TravelTelegramBot.Constants;
using TravelTelegramBot.Handlers.Interfaces;
using TravelTelegramBot.Services.Interfaces;

namespace TravelTelegramBot.Handlers;

/// <summary>
/// Глобальный обработчик сообщения пользователя
/// </summary>
public sealed class UpdateHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly IUserService _user;
    private readonly IStorageService _storage;
    private readonly IAdminWorkflowService _adminWorkflowService;
    private readonly IKeyboardService _keyboard;
    private readonly IMessageService _messageService;
    private readonly IEnumerable<ICommandHandler> _commands;

    public UpdateHandler(
        ITelegramBotClient bot,
        IUserService user,
        IStorageService storage,
        IAdminWorkflowService adminWorkflow,
        IKeyboardService keyboard,
        IMessageService messageService,
        IEnumerable<ICommandHandler> commands)
    {
        _bot = bot;
        _user = user;
        _storage = storage;
        _adminWorkflowService = adminWorkflow;
        _keyboard = keyboard;
        _messageService = messageService;
        _commands = commands;
    }

    /// <summary>
    /// Обработать сообщение пользователя
    /// </summary>
    /// <param name="update">Сообщение пользователя</param>
    /// <param name="ct">Токен отмены операции</param>
    public async Task HandleUpdateAsync(Update update, CancellationToken ct)
    {
        if (update.Message is not { Text: { } text } message) return;
        long chatId = message.Chat.Id;
        bool isAdmin = _user.IsAdmin(chatId);

        _user.UpdateLog(message);

        if (text == MenuButtonsText.Back)
        {
            _adminWorkflowService.RemoveState(chatId);
            await _bot.SendTextMessageAsync(
                chatId,
                "🏠 Главное меню.",
                replyMarkup: _keyboard.GetMainMenu(_storage.Data, isAdmin),
                cancellationToken: ct);

            return;
        }

        if (isAdmin && _adminWorkflowService.TryGetState(chatId, out var state))
        {
            if (await _adminWorkflowService.HandleStateAsync(chatId, text, state, ct)) return;
        }

        var handler = _commands.FirstOrDefault(h => h.CanHandle(text));
        if (handler != null)
        {
            await handler.ExecuteAsync(chatId, message, ct);
            return;
        }

        await HandleDefaultLogic(chatId, text, isAdmin, ct);
    }

    /// <summary>
    /// Обработка сообщения по умолчанию.
    /// Присылаем список мест, или если пользователь что-то ищет - то присылаем найденные места или маршруты
    /// </summary>
    /// <param name="chatId">Id пользователя</param>
    /// <param name="text">Текст пользователя</param>
    /// <param name="isAdmin">Является ли пользователь админом</param>
    /// <param name="ct">Токен отмены операции</param>
    private async Task HandleDefaultLogic(long chatId, string text, bool isAdmin, CancellationToken ct)
    {
        var cat = _storage.Data.Categories.FirstOrDefault(c => c.Name.Equals(text, StringComparison.OrdinalIgnoreCase));
        if (cat != null)
        {
            var places = _storage.Data.Places.Where(p => p.Category == cat.Name);
            string res = $"📂 <b>{cat.Name}</b>\n\n" + string.Join("", places.Select(_messageService.PreparePlaceHtml));
            await _bot.SendTextMessageAsync(chatId, res, parseMode: ParseMode.Html, cancellationToken: ct);
            return;
        }

        var fp = _storage.Data.Places.Where(p => p.Name.Contains(text, StringComparison.OrdinalIgnoreCase));
        var fb = _storage.Data.Buses.Where(b => b.Number.Contains(text, StringComparison.OrdinalIgnoreCase));

        foreach (var p in fp) await _bot.SendTextMessageAsync(chatId,
            _messageService.PreparePlaceHtml(p),
            parseMode: ParseMode.Html,
            cancellationToken: ct);

        foreach (var b in fb) await _bot.SendTextMessageAsync(chatId,
            _messageService.PrepareBusScheduleHtml(b),
            parseMode: ParseMode.Html,
            cancellationToken: ct);
    }
}
