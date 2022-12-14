using static Helpers.Reader;

namespace Day25;

internal static class Day25
{
    private readonly record struct Star(int X, int Y, int Z, int W);

    private static void Main()
    {
        var data = ReadFile("Data/Day25.txt");
        var stars = new HashSet<Star>();
        foreach (var line in data)
        {
            var buff = Array.ConvertAll(line.Split(","), int.Parse);
            stars.Add(new Star(buff[0], buff[1], buff[2], buff[3]));
        }

        var constellations = new List<HashSet<Star>>();
        while (stars.Count > 0)
        {
            var first = stars.First();
            constellations.Add(new HashSet<Star> { first });
            stars.Remove(first);
            var added = true;
            while (added)
            {
                added = false;
                var constellation = constellations.Last();
                var toAdd = new List<Star>();
                foreach (var current in constellation)
                {
                    foreach (var star in stars)
                    {
                        if (Distance(current, star) <= 3)
                        {
                            toAdd.Add(star);
                        }
                    }
                }

                foreach (var entry in toAdd)
                {
                    added = true;
                    constellation.Add(entry);
                    stars.Remove(entry);
                }
            }
        }

        Console.WriteLine("Task 01: " + constellations.Count);
    }

    private static int Distance(Star first, Star second)
    {
        return Math.Abs(first.X - second.X) + Math.Abs(first.Y - second.Y) + Math.Abs(first.Z - second.Z) +
               Math.Abs(first.W - second.W);
    }
}