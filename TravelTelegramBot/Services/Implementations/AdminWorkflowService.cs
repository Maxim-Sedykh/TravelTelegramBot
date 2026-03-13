using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TravelTelegramBot.Constants;
using TravelTelegramBot.Models;
using TravelTelegramBot.Services.Interfaces;

namespace TravelTelegramBot.Services.Implementations;

/// <summary>
/// Сервис для функционала администатора. Удаление добавление сущностей.
/// В будущем нужно вынести логику обработки состояний для мест, категорий, автобусов
/// </summary>
public sealed class AdminWorkflowService(
    ITelegramBotClient bot,
    IStorageService storage,
    IKeyboardService keyboard) : IAdminWorkflowService
{
    private readonly Dictionary<long, string> _states = new();

    /// <inheritdoc/>
    public void SetState(long chatId, string state) => _states[chatId] = state;

    /// <inheritdoc/>
    public void RemoveState(long chatId) => _states.Remove(chatId);

    /// <inheritdoc/>
    public bool TryGetState(long chatId, out string state) => _states.TryGetValue(chatId, out state!);

    /// <inheritdoc/>
    public async Task<bool> HandleStateAsync(long chatId, string text, string state, CancellationToken ct)
    {
        return state switch
        {
            UserStates.WaitingForNewCategory => await HandleAddCategory(chatId, text, ct),
            UserStates.ManagingCategories => await HandleCategoryManagement(chatId, text, ct),
            UserStates.SelectPlaceCategory => await HandlePlaceCategorySelection(chatId, text, ct),
            var s when s.StartsWith(UserStates.AdminPlacesInPrefix) => await HandlePlaceListActions(chatId, text, state, ct),
            var s when s.StartsWith(UserStates.WaitPlaceData) => await HandlePlaceSaving(chatId, text, state, ct),
            UserStates.AdminManageBuses => await HandleBusListActions(chatId, text, ct),
            UserStates.WaitBusStep1 => await HandleBusStep1(chatId, text, ct),
            var s when s.StartsWith(UserStates.WaitBusStep2) => await HandleBusStep2(chatId, text, state, ct),
            _ => false
        };
    }

    #region Entry Points

    /// <inheritdoc/>
    public async Task EnterCategoryManagementAsync(long chatId, CancellationToken ct)
    {
        SetState(chatId, UserStates.ManagingCategories);
        await bot.SendTextMessageAsync(chatId, "📁 Управление категориями:",
            replyMarkup: keyboard.GetSelectionMenu(storage.Data.Categories.Select(c => c.Name),
            MenuButtonsText.AddCategory),
            cancellationToken: ct);
    }

    /// <inheritdoc/>
    public async Task EnterPlaceManagementAsync(long chatId, CancellationToken ct)
    {
        SetState(chatId, UserStates.SelectPlaceCategory);
        await bot.SendTextMessageAsync(chatId, "📍 Выберите категорию для управления местами:",
            replyMarkup: keyboard.GetSelectionMenu(storage.Data.Categories.Select(c => c.Name)), cancellationToken: ct);
    }

    /// <inheritdoc/>
    public async Task EnterBusManagementAsync(long chatId, CancellationToken ct)
    {
        SetState(chatId, UserStates.AdminManageBuses);
        await bot.SendTextMessageAsync(chatId, "🚐 Управление маршрутами:",
            replyMarkup: keyboard.GetSelectionMenu(storage.Data.Buses.Select(b => b.Number), MenuButtonsText.AddBus), cancellationToken: ct);
    }

    #endregion

    #region Categories Logic

    /// <summary>
    /// Добавить категорию
    /// </summary>
    private async Task<bool> HandleAddCategory(long chatId, string text, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(text) || text == MenuButtonsText.Back) return false;

        storage.Data.Categories.Add(new Category { Name = text });
        storage.Save();

        await bot.SendTextMessageAsync(chatId, $"✅ Категория «{text}» добавлена.", cancellationToken: ct);
        await EnterCategoryManagementAsync(chatId, ct);
        return true;
    }

    /// <summary>
    /// Отправить меню для создания новой категории
    /// </summary>
    private async Task<bool> HandleCategoryManagement(long chatId, string text, CancellationToken ct)
    {
        if (text == MenuButtonsText.AddCategory)
        {
            SetState(chatId, UserStates.WaitingForNewCategory);
            await bot.SendTextMessageAsync(chatId,
                "📝 Введите название для новой категории:",
                replyMarkup: keyboard.GetBackKb(),
                cancellationToken: ct);

            return true;
        }

        var cat = storage.Data.Categories.FirstOrDefault(c => c.Name == text);
        if (cat != null)
        {
            storage.Data.Categories.Remove(cat);
            storage.Save();
            await bot.SendTextMessageAsync(chatId, $"🗑 Категория «{text}» удалена.", cancellationToken: ct);
        }

        await EnterCategoryManagementAsync(chatId, ct);
        return true;
    }

    #endregion

    #region Places Logic

    /// <summary>
    /// Вывести пользователю места по категории
    /// </summary>
    private async Task<bool> HandlePlaceCategorySelection(long chatId, string text, CancellationToken ct)
    {
        var cat = storage.Data.Categories.FirstOrDefault(c => c.Name == text);
        if (cat == null) return false;

        await ShowPlacesInCategoryAsync(chatId, cat.Name, ct);
        return true;
    }

    /// <summary>
    /// Обработать управление местами
    /// </summary>
    private async Task<bool> HandlePlaceListActions(long chatId, string text, string state, CancellationToken ct)
    {
        string catName = state.Split(':')[1];

        if (text == MenuButtonsText.AddPlace)
        {
            SetState(chatId, $"{UserStates.WaitPlaceData}:{catName}");
            await bot.SendTextMessageAsync(chatId,
                "Введите данные через запятую:\n\"Название, График, Перерыв, Адрес, Контакты, Доп. график, Коммент к графику, Примечание\"",
                replyMarkup: keyboard.GetBackKb(),
                cancellationToken: ct);
            return true;
        }

        var place = storage.Data.Places.FirstOrDefault(x => x.Category == catName && x.Name == text);
        if (place != null)
        {
            string rec = $"{place.Name}, {place.WorkTime}, {place.Break}, {place.Address}, {place.Contact}," +
                $" {place.HolidayWorkTime}, {place.HolidayCommentary}, {place.Commentary}";
            storage.Data.Places.Remove(place);
            storage.Save();

            await bot.SendTextMessageAsync(
                chatId,
                $"🗑 Удалено. Код для восстановления:\n<code>{rec}</code>",
                parseMode: ParseMode.Html,
                cancellationToken: ct);

            await ShowPlacesInCategoryAsync(chatId, catName, ct);
        }
        return true;
    }

    /// <summary>
    /// Обработать создание места
    /// </summary>
    private async Task<bool> HandlePlaceSaving(long chatId, string text, string state, CancellationToken ct)
    {
        var p = text.Split(',').Select(x => x.Trim()).ToArray();
        if (p.Length < 7) { await bot.SendTextMessageAsync(chatId, "❌ Ошибка формата", cancellationToken: ct); return true; }

        string catName = state.Split(':')[1];
        storage.Data.Places.Add(new Place 
        { 
            Category = catName, 
            Name = p[0], 
            WorkTime = p[1], 
            Break = p[2], 
            Address = p[3], 
            Contact = p[4], 
            HolidayWorkTime = p[5], 
            HolidayCommentary = p[6], 
            Commentary = p.Length > 7 ? p[7] : "-" 
        });

        storage.Save();

        await bot.SendTextMessageAsync(chatId, "✅ Место сохранено", cancellationToken: ct);
        await ShowPlacesInCategoryAsync(chatId, catName, ct);
        return true;
    }

    /// <summary>
    /// Показать все места в категории
    /// </summary>
    private async Task ShowPlacesInCategoryAsync(long chatId, string catName, CancellationToken ct)
    {
        SetState(chatId, UserStates.AdminPlacesInPrefix + catName);
        var names = storage.Data.Places.Where(p => p.Category == catName).Select(p => p.Name);
        await bot.SendTextMessageAsync(chatId, $"📍 Места в категории {catName}:",
            replyMarkup: keyboard.GetSelectionMenu(names, MenuButtonsText.AddPlace), cancellationToken: ct);
    }

    #endregion

    #region Buses Logic

    /// Обработать управление маршрутами
    private async Task<bool> HandleBusListActions(long chatId, string text, CancellationToken ct)
    {
        if (text == MenuButtonsText.AddBus)
        {
            SetState(chatId, UserStates.WaitBusStep1);
            await bot.SendTextMessageAsync(chatId, "Шаг 1. " +
                "Введите через запятую: \"Номер, Станция А, Станция Б\"",
                replyMarkup: keyboard.GetBackKb(),
                cancellationToken: ct);

            return true;
        }

        var bus = storage.Data.Buses.FirstOrDefault(x => x.Number == text);
        if (bus != null)
        {
            string rec = $"{bus.Number}, {bus.StationA}, {bus.StationB}";
            string b1 = bus.ScheduleForward.Any() ? string.Join(", ", bus.ScheduleForward) : "-";
            string b2 = bus.ScheduleBackward.Any() ? string.Join(", ", bus.ScheduleBackward) : "-";
            string b3 = bus.HolidayForward.Any() ? string.Join(", ", bus.HolidayForward) : "-";
            string b4 = bus.HolidayBackward.Any() ? string.Join(", ", bus.HolidayBackward) : "-";

            storage.Data.Buses.Remove(bus);
            storage.Save();
            await bot.SendTextMessageAsync(chatId, $"🗑 Маршрут удален.\n" +
                $"Код восстановления:\n<code>Шаг 1.\n{rec} \nШаг 2.\n{b1} | {b2} | {b3} | {b4}</code>",
                parseMode: ParseMode.Html);

            await EnterBusManagementAsync(chatId, ct);
        }
        return true;
    }

    /// <summary>
    /// Обработать первый шаг создания автобусов
    /// </summary>
    private async Task<bool> HandleBusStep1(long chatId, string text, CancellationToken ct)
    {
        var parts = text.Split(',').Select(x => x.Trim()).ToArray();
        if (parts.Length != 3) { await bot.SendTextMessageAsync(chatId, "❌ Ошибка!", cancellationToken: ct); return true; }

        storage.Data.Buses.Add(new BusRoute { Number = parts[0], StationA = parts[1], StationB = parts[2] });
        SetState(chatId, $"{UserStates.WaitBusStep2}:{parts[0]}");

        await bot.SendTextMessageAsync(chatId, $"✅ Данные {parts[0]} приняты." +
            $"\nШаг 2. Введите расписание через '|':",
            replyMarkup: keyboard.GetBackKb(),
            cancellationToken: ct);

        return true;
    }

    /// <summary>
    /// Обработать второй шаг создания маршрутов
    /// </summary>
    private async Task<bool> HandleBusStep2(long chatId, string text, string state, CancellationToken ct)
    {
        string busNumber = state.Split(':')[1];
        var bus = storage.Data.Buses.FirstOrDefault(b => b.Number == busNumber);
        var blocks = text.Split('|').Select(x => x.Trim()).ToArray();

        if (blocks.Length != 4 || bus == null) { await bot.SendTextMessageAsync(chatId, "❌ Ошибка!", cancellationToken: ct); return true; }

        bus.ScheduleForward = ParseTimes(blocks[0]); bus.ScheduleBackward = ParseTimes(blocks[1]);
        bus.HolidayForward = ParseTimes(blocks[2]); bus.HolidayBackward = ParseTimes(blocks[3]);

        storage.Save();
        await bot.SendTextMessageAsync(chatId, $"✅ Маршрут {busNumber} готов!", cancellationToken: ct);
        await EnterBusManagementAsync(chatId, ct);
        return true;
    }

    #endregion

    /// <summary>
    /// Спарсить время в массив строк
    /// </summary>
    private List<string> ParseTimes(string s) => s == "-" ? [] : s.Split(',').Select(x => x.Trim()).ToList();
}
