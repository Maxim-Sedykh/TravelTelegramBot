using System.Text;
using TravelTelegramBot.Models;
using TravelTelegramBot.Services.Interfaces;

namespace TravelTelegramBot.Services.Implementations;

public sealed class MessageService : IMessageService
{
    /// <inheritdoc/>
    public string PreparePlaceHtml(Place p)
    {
        StringBuilder sb = new();
        sb.AppendLine($"🔹 <b>{Handle(p.Name)}</b>");
        sb.AppendLine($"🏠 {Handle(p.Address)}");
        sb.AppendLine($"⏰ График: <code>{Handle(p.WorkTime)}</code> (пер: {Handle(p.Break)})");
        if (Handle(p.HolidayWorkTime) != "-") sb.AppendLine($"🗓 {p.HolidayCommentary}: <code>{p.HolidayWorkTime}</code>");
        if (Handle(p.Contact) != "-") sb.AppendLine($"📞 {p.Contact}");
        if (Handle(p.Commentary) != "-") sb.AppendLine($"💬 <i>{p.Commentary}</i>");
        return sb.ToString() + "\n";
    }

    /// <inheritdoc/>
    public string PrepareBusScheduleHtml(BusRoute bus)
    {
        var now = DateTime.UtcNow.AddHours(3);
        bool isHolidayToday = now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday;

        StringBuilder sb = new();
        sb.AppendLine($"🚌 <b>Маршрут {bus.Number}</b>");
        sb.AppendLine(isHolidayToday ? "✨ <b>СЕГОДНЯ ВЫХОДНОЙ</b>" : "💼 <b>СЕГОДНЯ БУДНИЙ ДЕНЬ</b>");
        sb.AppendLine();

        if (isHolidayToday)
        {
            RenderDayType(sb, "✨ РАСПИСАНИЕ ВЫХОДНОГО ДНЯ", bus, true, now.TimeOfDay);
            sb.AppendLine("───────────────────");
            RenderDayType(sb, "💼 РАСПИСАНИЕ БУДНЕГО ДНЯ", bus, false, null);
        }
        else
        {
            RenderDayType(sb, "💼 РАСПИСАНИЕ БУДНЕГО ДНЯ", bus, false, now.TimeOfDay);
            sb.AppendLine("───────────────────");
            RenderDayType(sb, "✨ РАСПИСАНИЕ ВЫХОДНОГО ДНЯ", bus, true, null);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Отрисовка блока расписания (Туда и Обратно) для конкретного типа дня.
    /// </summary>
    private void RenderDayType(StringBuilder sb, string title, BusRoute bus, bool isHoliday, TimeSpan? currentTime)
    {
        sb.AppendLine($"<b>{title}</b>");

        var forwardTimes = isHoliday ? bus.HolidayForward : bus.ScheduleForward;
        var backwardTimes = isHoliday ? bus.HolidayBackward : bus.ScheduleBackward;

        RenderDir(sb, bus.StationA, bus.StationB, forwardTimes, currentTime);

        RenderDir(sb, bus.StationB, bus.StationA, backwardTimes, currentTime);
        sb.AppendLine();
    }

    /// <summary>
    /// Отрисовка конкретного направления
    /// </summary>
    private void RenderDir(StringBuilder sb, string from, string to, List<string> times, TimeSpan? now)
    {
        sb.AppendLine($"📍 {from} ➡️ {to}");

        if (times == null || !times.Any() || times.All(x => x == "-"))
        {
            sb.AppendLine("❌ <i>Рейсов нет</i>");
            return;
        }

        var tsList = times
            .Select(t => TimeSpan.TryParse(t, out var ts) ? ts : (TimeSpan?)null)
            .Where(x => x.HasValue)
            .Select(x => x.Value)
            .OrderBy(x => x)
            .ToList();

        if (now.HasValue)
        {
            var next = tsList.FirstOrDefault(t => t > now.Value);
            if (next != default)
            {
                int diff = (int)(next - now.Value).TotalMinutes;
                sb.AppendLine($"✅ Ближайший: <b>{next:hh\\:mm}</b> (через {diff} мин)");
            }
            else
            {
                sb.AppendLine("🌙 <i>Рейсы на сегодня окончены</i>");
            }
        }

        sb.AppendLine($"🕒 <code>{string.Join(", ", tsList.Select(t => t.ToString(@"hh\:mm")))}</code>");
    }

    private string Handle(string s) => string.IsNullOrWhiteSpace(s) ? "-" : s;
}
