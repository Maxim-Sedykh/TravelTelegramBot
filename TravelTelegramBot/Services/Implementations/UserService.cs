using Telegram.Bot.Types;
using TravelTelegramBot.Models;
using TravelTelegramBot.Services.Interfaces;

namespace TravelTelegramBot.Services.Implementations;

public sealed class UserService : IUserService
{
    private readonly IStorageService _storage;

    public UserService(IStorageService storage) => _storage = storage;

    /// <inheritdoc/>
    public bool IsAdmin(long userId) => _storage.Data.AdminIds.Contains(userId);

    /// <inheritdoc/>
    public void UpdateLog(Message m)
    {
        var u = _storage.Data.Users.FirstOrDefault(x => x.UserId == m.Chat.Id);
        if (u == null)
        {
            _storage.Data.Users.Add(new UserLog()
            {
                UserId = m.Chat.Id,
                FirstName = m.From?.FirstName ?? "User",
                Username = m.From?.Username,
                LastActive = DateTime.Now
            });
        }
        else
        {
            u.LastActive = DateTime.Now;
            u.FirstName = m.From?.FirstName ?? u.FirstName;
        }
        _storage.Save();
    }
}
