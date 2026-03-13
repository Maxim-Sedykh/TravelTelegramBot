using Telegram.Bot.Types.ReplyMarkups;
using TravelTelegramBot.Constants;
using TravelTelegramBot.Models;
using TravelTelegramBot.Services.Interfaces;

namespace TravelTelegramBot.Services.Implementations;

public sealed class KeyboardService : IKeyboardService
{
    /// <inheritdoc/>
    public ReplyKeyboardMarkup GetMainMenu(BotData data, bool isAdmin)
    {
        var buttons = data.Categories.Select(c => new KeyboardButton(c.Name)).Chunk(2).ToList();

        buttons.Add([new KeyboardButton(MenuButtonsText.BusSchedule)]);

        if (isAdmin) buttons.Add(new[] { new KeyboardButton(MenuButtonsText.AdminPanel) });

        buttons.Add([new KeyboardButton(MenuButtonsText.Plug)]);

        return new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
    }

    /// <inheritdoc/>
    public ReplyKeyboardMarkup GetAdminMenu() => new([
        [new KeyboardButton(MenuButtonsText.Categories), new KeyboardButton(MenuButtonsText.Places)],
        [new KeyboardButton(MenuButtonsText.ManageBuses), new KeyboardButton(MenuButtonsText.Statistics)],
        [new KeyboardButton(MenuButtonsText.Back)],
        [new KeyboardButton(MenuButtonsText.Plug)]
    ]){ ResizeKeyboard = true };

    /// <inheritdoc/>
    public ReplyKeyboardMarkup GetBackKb() => new([[new KeyboardButton(MenuButtonsText.Back)], [new KeyboardButton(MenuButtonsText.Plug)]]) { ResizeKeyboard = true };

    /// <inheritdoc/>
    public ReplyKeyboardMarkup GetSelectionMenu(IEnumerable<string> items, string? extraButton = null)
    {
        var buttons = items.Select(i => new KeyboardButton(i)).Chunk(2).ToList();
        if (extraButton != null) buttons.Add([new KeyboardButton(extraButton)]);
        buttons.AddRange([new KeyboardButton(MenuButtonsText.Back)], [new KeyboardButton(MenuButtonsText.Plug)]);
        return new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
    }
}
