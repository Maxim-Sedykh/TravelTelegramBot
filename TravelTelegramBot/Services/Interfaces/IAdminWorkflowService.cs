namespace TravelTelegramBot.Services.Interfaces;

/// <summary>
/// Сервис для админских дел
/// </summary>
public interface IAdminWorkflowService
{
    /// <summary>
    /// Обработать состояние пользователя
    /// </summary>
    /// <param name="chatId">Id пользователя</param>
    /// <param name="text">Текст пользователя</param>
    /// <param name="state">Состояние пользователя</param>
    /// <param name="ct">Токен отмены операции</param>
    Task<bool> HandleStateAsync(long chatId, string text, string state, CancellationToken ct);

    /// <summary>
    /// Присвоить новое состояние пользователю
    /// </summary>
    /// <param name="chatId">Id пользователя</param>
    /// <param name="state">Новое состояние</param>
    void SetState(long chatId, string state);

    /// <summary>
    /// Убрать состояние пользователя
    /// </summary>
    /// <param name="chatId">Id пользователя</param>
    void RemoveState(long chatId);

    /// <summary>
    /// Попробовать получить состояние пользователя
    /// </summary>
    /// <param name="chatId">Id пользователя</param>
    /// <param name="state">Полученное состояние в виде выходного параметра</param>
    /// <returns>True - если удалось получить состояние пользователя</returns>
    bool TryGetState(long chatId, out string state);

    /// <summary>
    /// Отправить пользователю меню управления категориями
    /// </summary>
    Task EnterCategoryManagementAsync(long chatId, CancellationToken ct);

    /// <summary>
    /// Отправить пользователю меню управления местами
    /// </summary>
    Task EnterPlaceManagementAsync(long chatId, CancellationToken ct);

    /// <summary>
    /// Отправить пользователю меню управления маршрутами
    /// </summary>
    Task EnterBusManagementAsync(long chatId, CancellationToken ct);
}
