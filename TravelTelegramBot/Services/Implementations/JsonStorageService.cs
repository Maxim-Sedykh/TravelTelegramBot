using System.Text.Encodings.Web;
using System.Text.Json;
using TravelTelegramBot.Models;
using TravelTelegramBot.Services.Interfaces;

namespace TravelTelegramBot.Services.Implementations;

public sealed class JsonStorageService : IStorageService
{
    private const string FilePath = "data.json";

    /// <inheritdoc/>
    public BotData Data { get; private set; } = new();

    public JsonStorageService() => Load();

    /// <inheritdoc/>
    public void Save()
    {
        var options = new JsonSerializerOptions 
        { 
            WriteIndented = true, 
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping 
        };
        File.WriteAllText(FilePath, JsonSerializer.Serialize(Data, options));
    }

    /// <inheritdoc/>
    public void Load()
    {
        if (File.Exists(FilePath))
        {
            Data = JsonSerializer.Deserialize<BotData>(File.ReadAllText(FilePath)) ?? new BotData();
        }
    }
}
