using TravelTelegramBot.Models;

namespace TravelTelegramBot.Services.Interfaces;

/// <summary>
/// Сервис для взаимодействия с данными бота
/// Данные хранятся в json-файле
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Данные по сущностям
    /// </summary>
    BotData Data { get; }

    /// <summary>
    /// Сохранить данные <see cref="Data"/> в файл
    /// </summary>
    void Save();

    /// <summary>
    /// Загрузить данные из файла в объект <see cref="Data"/>
    /// </summary>
    void Load();
}
