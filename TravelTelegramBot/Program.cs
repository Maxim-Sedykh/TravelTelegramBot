using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using TravelTelegramBot.Handlers;
using TravelTelegramBot.Handlers.Commands;
using TravelTelegramBot.Handlers.Interfaces;
using TravelTelegramBot.Services.Implementations;
using TravelTelegramBot.Services.Interfaces;

namespace TravelTelegramBot;

/// <summary>
/// Точка входа и основной контроллер бота.
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                var botToken = context.Configuration.GetSection("BotConfiguration")["BotToken"];

                services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken!));
                services.AddSingleton<IStorageService, JsonStorageService>();
                services.AddSingleton<IUserService, UserService>();
                services.AddSingleton<IKeyboardService, KeyboardService>();
                services.AddSingleton<IMessageService, MessageService>();
                services.AddSingleton<IAdminWorkflowService, AdminWorkflowService>();
                services.AddSingleton<UpdateHandler>();

                services.AddSingleton<ICommandHandler, StartCommandHandler>();
                services.AddSingleton<ICommandHandler, BusScheduleCommandHandler>();
                services.AddSingleton<ICommandHandler, AdminPanelCommandHandler>();
                services.AddSingleton<ICommandHandler, StatisticsCommandHandler>();
                services.AddSingleton<ICommandHandler, ManageCategoriesCommandHandler>();
                services.AddSingleton<ICommandHandler, ManagePlacesCommandHandler>();
                services.AddSingleton<ICommandHandler, ManageBusesCommandHandler>();

                services.AddHostedService<BotBackgroundService>();
            })
            .Build();

        await host.RunAsync();
    }
}