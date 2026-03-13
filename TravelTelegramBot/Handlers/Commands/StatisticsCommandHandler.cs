using System.Net;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TravelTelegramBot.Constants;
using TravelTelegramBot.Handlers.Interfaces;
using TravelTelegramBot.Services.Interfaces;

namespace TravelTelegramBot.Handlers.Commands;

/// <summary>
/// Обработчик для получения статистики пользователей бота
/// </summary>
public sealed class StatisticsCommandHandler : ICommandHandler
{
    /// <inheritdoc/>
    public string CommandName => MenuButtonsText.Statistics;

    private readonly ITelegramBotClient _bot;
    private readonly IStorageService _storage;
    private readonly IUserService _user;

    public StatisticsCommandHandler(ITelegramBotClient bot, IStorageService storage, IUserService user)
    {
        _bot = bot;
        _storage = storage;
        _user = user;
    }

    /// <inheritdoc/>
    public bool CanHandle(string text) => text == CommandName;

    /// <inheritdoc/>
    public async Task ExecuteAsync(long chatId, Message message, CancellationToken ct)
    {
        if (!_user.IsAdmin(chatId)) return;

        var topUsers = _storage.Data.Users.OrderByDescending(u => u.LastActive).Take(20);
        StringBuilder sb = new("📊 Последние пользователи:\n\n");
        foreach (var u in topUsers)
        {
            sb.AppendLine($"👤 {WebUtility.HtmlEncode(u.FirstName)} (UserId: {u.UserId}) (UserName: {u.Username}) - {u.LastActive:dd.MM HH:mm}");
        }
        await _bot.SendTextMessageAsync(chatId, sb.ToString(), parseMode: ParseMode.Html, cancellationToken: ct);
    }
}
