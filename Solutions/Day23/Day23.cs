using System.Text.RegularExpressions;
using static Helpers.Reader;

namespace Day23;

internal static class Day23
{
    private readonly record struct Bot(long X, long Y, long Z, long Power);
    
    private static void Main()
    {
        var regEx = new Regex(@"<(?<pos>[0-9\-,]+)>, r=(?<power>[0-9]+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        var data = ReadFile("Data/Day23.txt");
        var bots = new List<Bot>();
        foreach (var line in data)
        {
            var matches = regEx.Match(line);
            var groups = matches.Groups;
            var pos = Array.ConvertAll(groups["pos"].Value.Split(","), long.Parse);
            var power = long.Parse(groups["power"].Value);
            bots.Add(new Bot(pos[0], pos[1], pos[2], power));
        }

        var reference = bots.MaxBy(entry => entry.Power);
        var inRange = CountInRange(reference, bots);
        Console.WriteLine("Task 01: " + inRange);

        var xMin = bots.MinBy(entry => entry.X).X;
        var yMin = bots.MinBy(entry => entry.Y).Y;
        var zMin = bots.MinBy(entry => entry.Z).Z;
        var xMax = bots.MaxBy(entry => entry.X).X;
        var yMax = bots.MaxBy(entry => entry.Y).Y;
        var zMax = bots.MaxBy(entry => entry.Z).Z;
        var rangeX = xMax - xMin;
        var rangeY = yMax - yMin;
        var rangeZ = zMax - zMin;
        var resolution = (int)Math.Pow(2, 26);
        var current = (0L, 0L, 0L);
        var searchCenter = (x:(xMax - xMin) / 2, y:(yMax - yMin) / 2, z:(zMax - zMin) / 2);
        var inRangeCurrent = 0;
        var distanceCurrent = long.MaxValue;
        while (resolution >= 1)
        {
            var (startX, endX) = ((searchCenter.x - rangeX / 2), (searchCenter.x + rangeX / 2));
            var (startY, endY) = ((searchCenter.y - rangeY / 2), (searchCenter.y + rangeY / 2));
            var (startZ, endZ) = ((searchCenter.z - rangeZ / 2), (searchCenter.z + rangeZ / 2));
            for (var x = startX; x < endX; x+=resolution)
            {
                for (var y = startY; y < endY; y+=resolution)
                {
                    for (var z = startZ; z < endZ; z+=resolution)
                    {
                        var inRangeBuff = CountInRangePoint((x, y, z), bots);
                        if (inRangeBuff > inRangeCurrent)
                        {
                            inRangeCurrent = inRangeBuff;
                            current = (x, y, z);
                            distanceCurrent = Distance(new Bot(0L, 0L, 0L, 0L), new Bot(x, y, z, 0L));
                        }

                        if (inRangeBuff == inRangeCurrent &&
                            Distance(new Bot(0L, 0L, 0L, 0L), new Bot(x, y, z, 0L)) < distanceCurrent)
                        {
                            current = (x, y, z);
                            distanceCurrent = Distance(new Bot(0L, 0L, 0L, 0L), new Bot(x, y, z, 0L));
                        }
                    }
                }
            }

            searchCenter = current;
            resolution /= 2; rangeX /= 2; rangeY /= 2; rangeZ /= 2;
        }
        Console.WriteLine("Task 02: " + distanceCurrent);
    }

    private static int CountInRangePoint((long x, long y, long z) point, List<Bot> bots)
    {
        var inRange = 0;
        foreach (var bot in bots)
        {
            if (IsInRange(bot, new Bot(point.x, point.y, point.z, 0))) inRange++;
        }

        return inRange;
    }

    private static int CountInRange(Bot reference, List<Bot> bots)
    {
        var inRange = 0;
        foreach (var bot in bots)
        {
            if (IsInRange(reference, bot)) inRange++;
        }

        return inRange;
    }
    
    private static bool IsInRange(Bot reference, Bot target)
    {
        return Distance(reference, target) <= reference.Power;
    }

    private static long Distance(Bot first, Bot second)
    {
        return Math.Abs(first.X - second.X) + Math.Abs(first.Y - second.Y) + Math.Abs(first.Z - second.Z);
    }
}