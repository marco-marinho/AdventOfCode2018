using static Helpers.Reader;
using System.Text.RegularExpressions;

namespace Day04;

internal static class Day04
{
    private static void Main()
    {
        var data = ReadFile("Data/Day04.txt");
        const string datePattern = "yyyy-MM-dd HH:mm";
        var events = new List<Event>();
        foreach (var line in data)
        {
            var buffer = line.Remove(0, 1).Split("] ");
            var date = DateTime.ParseExact(buffer[0], datePattern, null);
            int type;
            if (buffer[1].Contains("asleep"))
            {
                type = -2;
            }
            else if (buffer[1].Contains("wakes"))
            {
                type = -1;
            }
            else
            {
                type = int.Parse(Regex.Replace(buffer[1], "[^.0-9]", ""));
            }

            events.Add(new Event(date, type));
        }

        events.Sort();
        var curGuard = -1;
        var curAsleep = 0;
        var totalSleep = new Dictionary<int, int>();
        var minutesSleep = new Dictionary<int, Dictionary<int, int>>();
        foreach (var ev in events)
        {
            if (ev.Asleep())
            {
                curAsleep = ev.Minutes();
            }
            else if (ev.Awake())
            {
                var total = ev.Minutes() - curAsleep;
                totalSleep[curGuard] += total;
                foreach (var minutes in Enumerable.Range(curAsleep, total))
                {
                    minutesSleep[curGuard][minutes] = minutesSleep[curGuard].GetValueOrDefault(minutes, 0) + 1;
                }
            }
            else
            {
                curGuard = ev.Type;
                if (totalSleep.ContainsKey(curGuard)) continue;
                totalSleep[curGuard] = 0;
                minutesSleep[curGuard] = new Dictionary<int, int>();
            }
        }

        var mostSleep = totalSleep.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        var mostMinute = minutesSleep[mostSleep].Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        Console.WriteLine("Task 01: " + mostSleep * mostMinute);

        var idTask2 = 0;
        var maxTimes = 0;
        var maxMin = 0;
        foreach (var entry in minutesSleep)
        {
            foreach (var pair in entry.Value.Where(pair => pair.Value > maxTimes))
            {
                maxTimes = pair.Value;
                maxMin = pair.Key;
                idTask2 = entry.Key;
            }
        }
        Console.WriteLine("Task 02: " + idTask2 * maxMin);
    }
}

internal class Event : IComparable<Event>
{
    public DateTime Date;
    public int Type;

    public Event(DateTime date, int type)
    {
        Date = date;
        Type = type;
    }

    public bool Asleep()
    {
        return Type == -2;
    }

    public bool Awake()
    {
        return Type == -1;
    }

    public int Minutes()
    {
        return Date.Minute;
    }

    public int CompareTo(Event? other)
    {
        if (other == null) throw new ArgumentNullException();
        return DateTime.Compare(Date, other.Date);
    }
}