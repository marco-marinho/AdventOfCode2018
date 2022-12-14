using Helpers;
using static Helpers.Reader;

namespace Day18;

internal static class Day18
{
    private static void Main()
    {
        var data = ReadFile("Data/Day18.txt");
        var field = ArrayHelper.GetNew2DArray(data.Length, data[0].Length, '.');
        var limit = (x: data.Length, y: data[0].Length);
        for (var i = 0; i < limit.x; i++)
        {
            for (var j = 0; j < limit.y; j++)
            {
                field[i, j] = data[i][j];
            }
        }

        var history = new Dictionary<string, int>();
        var foundCycle = false;
        for (var i = 0; i < 1000000000; i++)
        {
            if (history.ContainsKey(FieldToString(field)) && !foundCycle)
            {
                var repeatLen = i - history[FieldToString(field)];
                var left = (1000000000 - i) % repeatLen;
                i = 1000000000 - left;
                foundCycle = true;
            }
            else
            {
                history[FieldToString(field)] = i;
            }

            if (i == 9)
            {
                Console.WriteLine("Task 01: "+ Count(field, '|') * Count(field, '#'));
            }

            field = Step(field, limit);
        }

        Console.WriteLine("Task 02: "+ Count(field, '|') * Count(field, '#'));
    }

    private static string FieldToString(char[,] field)
    {
        var output = "";
        for (var i = 0; i < field.GetLength(0); i++)
        {
            var buff = ArrayHelper.GetRow(field, i);
            output += string.Join("", buff);
        }

        return output;
    }

    private static int Count(char[,] status, char target)
    {
        var count = 0;
        for (var i = 0; i < status.GetLength(0); i++)
        {
            for (var j = 0; j < status.GetLength(1); j++)
            {
                if (status[i, j] == target) count++;
            }
        }

        return count;
    }

    private static char[,] Step(char[,] field, (int x, int y) limit)
    {
        var output = ArrayHelper.GetNew2DArray(limit.x, limit.y, '.');
        for (var i = 0; i < limit.x; i++)
        {
            for (var j = 0; j < limit.y; j++)
            {
                var adjacent = CountAdjacent(field, i, j, limit);
                var current = field[i, j];
                switch (current)
                {
                    case '.':
                        if (adjacent.GetValueOrDefault('|', 0) >= 3) output[i, j] = '|';
                        else output[i, j] = '.';
                        break;
                    case '|':
                        if (adjacent.GetValueOrDefault('#', 0) >= 3) output[i, j] = '#';
                        else output[i, j] = '|';
                        break;
                    case '#':
                        if (adjacent.GetValueOrDefault('#', 0) >= 1 && adjacent.GetValueOrDefault('|', 0) >= 1)
                            output[i, j] = '#';
                        else output[i, j] = '.';
                        break;
                }
            }
        }

        return output;
    }

    private static Dictionary<char, int> CountAdjacent(char[,] field, int x, int y, (int, int) limits)
    {
        var output = new Dictionary<char, int>();
        var adjacent = GetAdjacent(x, y, limits);
        foreach (var entry in adjacent)
        {
            var (i, j) = entry;
            output[field[i, j]] = output.GetValueOrDefault(field[i, j], 0) + 1;
        }

        return output;
    }

    private static List<(int, int)> GetAdjacent(int i, int j, (int x, int y) limits)
    {
        var output = new List<(int, int)>();
        for (var offsetX = -1; offsetX < 2; offsetX++)
        {
            for (var offsetY = -1; offsetY < 2; offsetY++)
            {
                var x = i + offsetX;
                var y = j + offsetY;
                if (x < 0 || y < 0 || x >= limits.x || y >= limits.y || (x == i && y == j)) continue;
                output.Add((x, y));
            }
        }

        return output;
    }
}