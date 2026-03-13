namespace TravelTelegramBot.Constants;

/// <summary>
/// Содержит идентификаторы состояний пользователей для FSM.
/// </summary>
public static class UserStates
{
    public const string WaitingForNewCategory = "WAIT_NEW_CAT";
    public const string ManagingCategories = "MANAGE_CATS";
    public const string SelectPlaceCategory = "SELECT_PLACE_CAT";
    public const string WaitPlaceData = "WAIT_PLACE_DATA";
    public const string AdminManageBuses = "ADMIN_MANAGE_BUSES";
    public const string WaitBusStep1 = "WAIT_BUS_STEP_1";
    public const string WaitBusStep2 = "WAIT_BUS_STEP_2";
    public const string AdminPlacesInPrefix = "ADMIN_PLACES_IN:";
}
