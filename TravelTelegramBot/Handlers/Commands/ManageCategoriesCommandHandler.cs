using Telegram.Bot.Types;
using TravelTelegramBot.Constants;
using TravelTelegramBot.Handlers.Interfaces;
using TravelTelegramBot.Services.Interfaces;

namespace TravelTelegramBot.Handlers.Commands;

public sealed class ManageCategoriesCommandHandler(IUserService user, IAdminWorkflowService adminWorkflowService) : ICommandHandler
{
    /// <inheritdoc/>
    public string CommandName => MenuButtonsText.Categories;

    /// <inheritdoc/>
    public bool CanHandle(string text) => text == CommandName;

    /// <inheritdoc/>
    public async Task ExecuteAsync(long chatId, Message message, CancellationToken ct)
    {
        if (!user.IsAdmin(chatId)) return;

        await adminWorkflowService.EnterCategoryManagementAsync(chatId, ct);
    }
}
