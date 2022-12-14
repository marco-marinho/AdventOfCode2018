using Helpers;
using static Helpers.Reader;

namespace Day20;

internal static class Day20
{
    public readonly record struct Entry(int x, int y, char value);

    public readonly record struct Position(int x, int y);

    private static void Main()
    {
        var regex = ReadFile("Data/Day20.txt")[0];
        var entries = ProcessString(new Position(0, 0), regex);
        entries.Add(new Entry(0, 0, 'X'));
        var minX = entries.Min(entry => entry.x);
        var maxX = entries.Max(entry => entry.x);
        var minY = entries.Min(entry => entry.y);
        var maxY = entries.Max(entry => entry.y);
        var grid = ArrayHelper.GetNew2DArray(maxX - minX + 3, maxY - minY + 3, '#');
        foreach (var entry in entries)
        {
            grid[entry.x - minX + 1, entry.y - minY + 1] = entry.value;
        }

        PrintStatus(grid);
        var distances = Djkistras(grid, new Position(0 - minX + 1, 0 - minY + 1));
        var maxDist = distances.Max(entry => entry.Item2);
        var minThousand = distances.Count(entry => entry.Item2 >= 1000);
        Console.WriteLine("Task 01: " + maxDist);
        Console.WriteLine("Task 02: " + minThousand);
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

    private static List<(Position, int)> Djkistras(char[,] grid, Position start)
    {
        var numRows = grid.GetLength(0) - 2;
        var numCols = grid.GetLength(1) - 2;
        var visited = new HashSet<Position>();
        var toVisit = new HashSet<(Position, int)>();
        var result = new List<(Position, int)>();
        toVisit.Add((start, 0));
        var changes = new[] { (0, 2), (0, -2), (2, 0), (-2, 0) };

        while (toVisit.Count != 0)
        {
            var minDist = toVisit.Min(entry => entry.Item2);
            var next = toVisit.First(entry => entry.Item2 == minDist);
            toVisit.Remove(next);
            visited.Add(next.Item1);
            result.Add(next);
            var currentPos = next.Item1;
            var curDistance = next.Item2;
            foreach (var change in changes)
            {
                var (xdif, ydif) = change;
                var buff = new Position(x: currentPos.x + xdif, y: currentPos.y + ydif);
                if (buff.x > numRows || buff.y > numCols || buff.y < 0 || buff.x < 0 || visited.Contains(buff))
                {
                    continue;
                }

                if (grid[buff.x - (xdif / 2), buff.y - (ydif / 2)] == '|' ||
                    grid[buff.x - (xdif / 2), buff.y - (ydif / 2)] == '-')
                {
                    toVisit.Add((buff, curDistance + 1));
                }
            }
        }

        return result;
    }

    private static HashSet<Entry> ProcessString(Position startInput, string regex)
    {
        var output = new HashSet<Entry>();
        var positions = new HashSet<Position> { startInput };
        var stack = new List<(HashSet<Position>, HashSet<Position>)>();
        var start = new HashSet<Position> { startInput };
        var ends = new HashSet<Position>();
        foreach (var entry in regex)
        {
            var tempPositions = new HashSet<Position>();
            switch (entry)
            {
                case 'N':
                    foreach (var position in positions)
                    {
                        output.Add(new Entry(position.x - 1, position.y, '-'));
                        output.Add(new Entry(position.x - 2, position.y, '.'));
                        tempPositions.Add(position with { x = position.x - 2 });
                    }

                    positions = tempPositions;
                    break;
                case 'S':
                    foreach (var position in positions)
                    {
                        output.Add(new Entry(position.x + 1, position.y, '-'));
                        output.Add(new Entry(position.x + 2, position.y, '.'));
                        tempPositions.Add(position with { x = position.x + 2 });
                    }

                    positions = tempPositions;
                    break;
                case 'E':
                    foreach (var position in positions)
                    {
                        output.Add(new Entry(position.x, position.y + 1, '|'));
                        output.Add(new Entry(position.x, position.y + 2, '.'));
                        tempPositions.Add(position with { y = position.y + 2 });
                    }

                    positions = tempPositions;
                    break;
                case 'W':
                    foreach (var position in positions)
                    {
                        output.Add(new Entry(position.x, position.y - 1, '|'));
                        output.Add(new Entry(position.x, position.y - 2, '.'));
                        tempPositions.Add(position with { y = position.y - 2 });
                    }

                    positions = tempPositions;
                    break;
                case '|':
                    ends.UnionWith(positions);
                    positions = new HashSet<Position>(start);
                    break;
                case '(':
                    stack.Add((start, ends));
                    start = new HashSet<Position>(positions);
                    ends = new HashSet<Position>();
                    break;
                case ')':
                    positions.UnionWith(ends);
                    (start, ends) = stack.Last();
                    stack.RemoveAt(stack.Count - 1);
                    break;
            }
        }

        return output;
    }
}