using static Helpers.Reader;

namespace Day05;

internal static class Day05
{
    private static void Main()
    {
        var data = ReadFile("Data/Day05.txt")[0];

        var t01 = React(data);
        Console.WriteLine("Task 01: " + t01);
        var min = int.MaxValue;
        for (var chRemove = 'a'; chRemove <= 'z'; chRemove++)
        {
            var buff = data.Replace(chRemove.ToString(), "").Replace((chRemove).ToString().ToUpper(), "");
            var res = React(buff);
            if (res > min) continue;
            min = res;
        }
        Console.WriteLine("Task 02: "+ min);
    }

    private static int React(string polymer)
    {
        var chars = new List<char>();
        chars.AddRange(polymer);
        while (true)
        {
            var toRemove = new List<int>();
            for (var idx = 0; idx < chars.Count - 1; idx++)
            {
                if (Math.Abs(chars[idx] - chars[idx + 1]) != 32) continue;
                toRemove.Add(idx);
                toRemove.Add(idx + 1);
                idx += 2;
            }

            if (toRemove.Count == 0) break;
            foreach(var idx in toRemove.OrderByDescending(v => v))
            {
                chars.RemoveAt(idx);
            }
        }

        return chars.Count;
    }
    
}