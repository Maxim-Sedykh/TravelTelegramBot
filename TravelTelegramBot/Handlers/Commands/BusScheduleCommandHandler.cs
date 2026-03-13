using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TravelTelegramBot.Constants;
using TravelTelegramBot.Handlers.Interfaces;
using TravelTelegramBot.Services.Interfaces;

namespace TravelTelegramBot.Handlers.Commands;

/// <summary>
/// Обработчик для получения расписания маршрута
/// </summary>
public sealed class BusScheduleCommandHandler : ICommandHandler
{
    /// <inheritdoc/>
    public string CommandName => MenuButtonsText.BusSchedule;

    private readonly ITelegramBotClient _bot;
    private readonly IStorageService _storage;
    private readonly IMessageService _messages;

    public BusScheduleCommandHandler(ITelegramBotClient bot, IStorageService storage, IMessageService messages)
    {
        _bot = bot;
        _storage = storage;
        _messages = messages;
    }

    /// <inheritdoc/>
    public bool CanHandle(string text) => text == CommandName;

    /// <inheritdoc/>
    public async Task ExecuteAsync(long chatId, Message message, CancellationToken ct)
    {
        if (_storage.Data.Buses.Count == 0)
        {
            await _bot.SendTextMessageAsync(chatId, "Расписание пока не заполнено.", cancellationToken: ct);
            return;
        }

        foreach (var bus in _storage.Data.Buses)
        {
            await _bot.SendTextMessageAsync(chatId, _messages.PrepareBusScheduleHtml(bus),
                parseMode: ParseMode.Html, cancellationToken: ct);
        }
    }
}
