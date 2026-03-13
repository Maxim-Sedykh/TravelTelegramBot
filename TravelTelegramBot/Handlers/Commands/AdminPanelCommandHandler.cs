using Telegram.Bot;
using Telegram.Bot.Types;
using TravelTelegramBot.Constants;
using TravelTelegramBot.Handlers.Interfaces;
using TravelTelegramBot.Services.Interfaces;

namespace TravelTelegramBot.Handlers.Commands;

/// <summary>
/// Обработчик панели администратора
/// </summary>
public sealed class AdminPanelCommandHandler : ICommandHandler
{
    /// <inheritdoc/>
    public string CommandName => MenuButtonsText.AdminPanel;

    private readonly ITelegramBotClient _bot;
    private readonly IUserService _user;
    private readonly IKeyboardService _keyboard;

    public AdminPanelCommandHandler(ITelegramBotClient bot, IUserService user, IKeyboardService keyboard)
    {
        _bot = bot; _user = user; _keyboard = keyboard;
    }

    /// <inheritdoc/>
    public bool CanHandle(string text) => text == CommandName;

    /// <inheritdoc/>
    public async Task ExecuteAsync(long chatId, Message message, CancellationToken ct)
    {
        if (!_user.IsAdmin(chatId)) return;
        await _bot.SendTextMessageAsync(chatId, "⚙️ Панель управления администратора:",
            replyMarkup: _keyboard.GetAdminMenu(), cancellationToken: ct);
    }
}
