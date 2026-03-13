using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using TravelTelegramBot.Handlers;

namespace TravelTelegramBot.Services.Implementations;

/// <summary>
/// Фоновая таска для работы тг бота
/// </summary>
public sealed class BotBackgroundService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly UpdateHandler _updateHandler;

    public BotBackgroundService(ITelegramBotClient botClient, UpdateHandler updateHandler)
    {
        _botClient = botClient;
        _updateHandler = updateHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Бот запущен...");

        _botClient.StartReceiving(
            updateHandler: (bot, update, ct) => _updateHandler.HandleUpdateAsync(update, ct),
            pollingErrorHandler: (bot, ex, ct) => { Console.WriteLine(ex.Message); return Task.CompletedTask; },
            receiverOptions: new ReceiverOptions { AllowedUpdates = [] },
            cancellationToken: stoppingToken
        );

        await Task.Delay(-1, stoppingToken);
    }
}
