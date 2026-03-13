using Telegram.Bot.Types;
using TravelTelegramBot.Constants;
using TravelTelegramBot.Handlers.Interfaces;
using TravelTelegramBot.Services.Interfaces;

namespace TravelTelegramBot.Handlers.Commands;

public sealed class ManageBusesCommandHandler(IUserService user, IAdminWorkflowService adminWorkflowService) : ICommandHandler
{
    /// <inheritdoc/>
    public string CommandName => MenuButtonsText.ManageBuses;

    /// <inheritdoc/>
    public bool CanHandle(string text) => text == CommandName;

    /// <inheritdoc/>
    public async Task ExecuteAsync(long chatId, Message message, CancellationToken ct)
    {
        if (!user.IsAdmin(chatId)) return;

        await adminWorkflowService.EnterBusManagementAsync(chatId, ct);
    }
}
