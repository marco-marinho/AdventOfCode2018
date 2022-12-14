using System.Text.RegularExpressions;
using static Helpers.Reader;
using Helpers;

namespace Day17;

internal static class Day17
{
    private static void Main()
    {

        const bool debugOutput = true;
        
        var rx = new Regex(@"x=(?<start>[0-9]+)\.*(?<end>[0-9]*)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var ry = new Regex(@"y=(?<start>[0-9]+)\.*(?<end>[0-9]*)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var data = ReadFile("Data/Day17.txt");
        var clay = new List<(int?, int?, int?, int?)>();
        foreach (var line in data)
        {
            var matchesX = rx.Match(line);
            var groupsX = matchesX.Groups;
            var startX = groupsX["start"];
            var endX = groupsX["end"];
            var xStart = int.Parse(startX.Value);
            int? xEnd = endX.Length > 0 ? int.Parse(endX.Value) : null;

            var matchesY = ry.Match(line);
            var groupsY = matchesY.Groups;
            var startY = groupsY["start"];
            var endY = groupsY["end"];
            var yStart = int.Parse(startY.Value);
            int? yEnd = endY.Length > 0 ? int.Parse(endY.Value) : null;
            clay.Add((xStart, xEnd, yStart, yEnd));
        }

        var xMin = clay.Min(entry =>
            (entry.Item1 ?? int.MaxValue) > (entry.Item2 ?? int.MaxValue) ? entry.Item2 : entry.Item1) ?? 0;
        var yMin = clay.Min(entry =>
            (entry.Item3 ?? int.MaxValue) > (entry.Item4 ?? int.MaxValue) ? entry.Item4 : entry.Item3) ?? 0;
        var xMax = clay.Max(entry => (entry.Item1 ?? 0) > (entry.Item2 ?? 0) ? entry.Item1 : entry.Item2) ??
                   int.MaxValue;
        var yMax = clay.Max(entry => (entry.Item3 ?? 0) > (entry.Item4 ?? 0) ? entry.Item3 : entry.Item4) ??
                   int.MaxValue;

        var status = ArrayHelper.GetNew2DArray(yMax + 1, xMax - xMin + 10, '.');
        status[0, 500 - xMin + 5] = '+';

        foreach (var entry in clay)
        {
            var xStart = (entry.Item1 ?? 0) - xMin;
            var xEnd = (entry.Item2 - xMin ?? xStart) + 1;
            var yStart = entry.Item3 ?? 0;
            var yEnd = (entry.Item4 ?? yStart) + 1;
            for (var i = xStart; i < xEnd; i++)
            {
                for (var j = yStart; j < yEnd; j++)
                {
                    status[j, i + 5] = '#';
                }
            }
        }

        var drips = new List<DripHead> { new(0, 500 - xMin + 5) };
        while (drips.Count != 0)
        {
            var nextDrips = new List<DripHead>();
            foreach (var drip in drips)
            {
                nextDrips.AddRange(drip.Drip(ref status));
            }

            drips = nextDrips;
        }

        if (debugOutput) PrintStatus(status);
        Console.WriteLine("Task 01: " + CountWet(status, yMin));
        Console.WriteLine("Task 02: " + CountStale(status, yMin));
    }

    private static void PrintStatus(char[,] status)
    {
        Console.WriteLine("");
        for (var i = 0; i < status.GetLength(0); i++)
        {
            var buff = ArrayHelper.GetRow(status, i);
            Console.WriteLine(string.Join("", buff));
        }

        Console.WriteLine("");
    }

    private static int CountWet(char[,] status, int yMin)
    {
        var count = 0;
        for (var i = yMin; i < status.GetLength(0); i++)
        {
            for (var j = 0; j < status.GetLength(1); j++)
            {
                if (status[i, j] is '~' or '|') count++;
            }
        }

        return count;
    }
    
    private static int CountStale(char[,] status, int yMin)
    {
        var count = 0;
        for (var i = yMin; i < status.GetLength(0); i++)
        {
            for (var j = 0; j < status.GetLength(1); j++)
            {
                if (status[i, j] is '~') count++;
            }
        }

        return count;
    }

    private record DripHead
    {
        private readonly Point2D _position;

        public DripHead(int x, int y)
        {
            _position = new Point2D(x, y);
        }

        public IEnumerable<DripHead> Drip(ref char[,] status)
        {
            var output = new List<DripHead>();
            if (_position.X + 1 >= status.GetLength(0)) return output;
            var next = status[_position.X + 1, _position.Y];
            switch (next)
            {
                case '.':
                    status[_position.X + 1, _position.Y] = '|';
                    output.Add(new DripHead(_position.X + 1, _position.Y));
                    break;
                case '#' or '~':
                {
                    char fill;
                    var bounds = GetBounds(ref status);
                    if (!bounds.Item1.Item2 && !bounds.Item2.Item2) fill = '~';
                    else fill = '|';
                    for (var j = bounds.Item1.Item1; j <= bounds.Item2.Item1; j++)
                    {
                        status[_position.X, j] = fill;
                    }

                    if (bounds.Item1.Item2) output.Add(new DripHead(_position.X, bounds.Item1.Item1));
                    if (bounds.Item2.Item2) output.Add(new DripHead(_position.X, bounds.Item2.Item1));
                    if (!bounds.Item1.Item2 && !bounds.Item2.Item2) output.Add(new DripHead(_position.X - 1, _position.Y));
                    break;
                }
            }

            return output;
        }

        private ((int, bool), (int, bool)) GetBounds(ref char[,] status)
        {
            var limX = status.GetLength(1);
            int boundLeft, boundRight;
            bool fallRight, fallLeft;
            var currentY = _position.Y;
            while (true)
            {
                if (currentY <= 0)
                {
                    boundLeft = 0;
                    fallLeft = false;
                    break;
                }

                var hasLeft = status[_position.X, currentY - 1] is '#';
                if (hasLeft)
                {
                    boundLeft = currentY;
                    fallLeft = false;
                    break;
                }

                var hasBellow = status[_position.X + 1, currentY] is '#' or '~';
                if (!hasBellow)
                {
                    boundLeft = currentY;
                    fallLeft = true;
                    break;
                }


                currentY--;
            }

            currentY = _position.Y;
            while (true)
            {
                if (currentY >= limX)
                {
                    boundRight = limX;
                    fallRight = false;
                    break;
                }

                var hasRight = status[_position.X, currentY + 1] is '#';
                if (hasRight)
                {
                    boundRight = currentY;
                    fallRight = false;
                    break;
                }

                var hasBellow = status[_position.X + 1, currentY] is '#' or '~';
                if (!hasBellow)
                {
                    boundRight = currentY;
                    fallRight = true;
                    break;
                }


                currentY++;
            }

            return ((boundLeft, fallLeft), (boundRight, fallRight));
        }
    }
}