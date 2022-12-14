using static Helpers.ArrayHelper;

namespace Day11;

internal static class Day11
{
    private static void Main()
    {
        var power = GetNew2DArray(300, 300, 0);
        const int serial = 7511;
        for (var x = 0; x < 300; x++)
        {
            for (var y = 0; y < 300; y++)
            {
                power[x, y] = CalculatePower(x + 1, y + 1, serial);
            }
        }

        var sums = new Dictionary<(int, int, int), int>();
        GetSums(ref power, ref sums, 3);
        var ans = sums.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        Console.WriteLine("Task 01: " + ans);
        
        for (var size = 4; size < 100; size++)
        {
            GetSums(ref power, ref sums, size);
        }
        ans = sums.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        Console.WriteLine("Task 02: " + ans);
    }

    private static void GetSums(ref int[,] power, ref Dictionary<(int, int, int), int> sums, int size)
    {
        for (var x = 0; x < 300 - size + 1; x++)
        {
            for (var y = 0; y < 300 - size + 1; y++)
            {
                sums[(x+1, y+1, size)] = SumSquare(ref power, x, y, size, ref sums);
            }
        }
    }

    private static int SumSquare(ref int[,] square, int x, int y, int size, ref Dictionary<(int, int, int), int> sums)
    {
        var sum = 0;
        
        if (sums.ContainsKey((x+1,y+1,size-1)))
        {
            sum += sums[(x+1,(y+1),(size-1))];
            for (var i = x; i < x + size - 1; i++)
            {
                sum += square[i, y + size - 1];
            } 
            for (var j = y; j < y + size - 1; j++)
            {
                sum += square[x + size - 1, j];
            }

            sum += square[x + size - 1, y + size - 1];
            return sum;
        }
        
        for (var i = x; i < x + size; i++)
        {
            for (var j = y; j < y + size; j++)
            {
                sum += square[i, j];
            }
        }

        return sum;
    }


    private static int CalculatePower(int x, int y, int serial)
    {
        var rackId = x + 10;
        var powerLevel = (rackId * y + serial) * rackId / 100 % 10 - 5;
        return powerLevel;
    }
}