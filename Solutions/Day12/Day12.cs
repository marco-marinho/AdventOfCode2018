using static Helpers.Reader;

namespace Day12;

internal static class Day12
{
    private static void Main()
    {
        var data = ReadFile("Data/Day12.txt");

        var status = new LinkedList<bool>();
        var transform = new Dictionary<(bool, bool, bool, bool, bool), bool>();
        
        var firstIdx = -3;
        status.AddFirst(false);
        status.AddFirst(false);
        status.AddFirst(false);
        foreach (var entry in data[0].Replace("initial state: ", ""))
        {
            status.AddLast(entry == '#');
        }
        status.AddLast(false);
        status.AddLast(false);
        status.AddLast(false);

        foreach (var entry in data.Skip(2))
        {
            var buffer = entry.Split(" => ");
            var chars = buffer[0].ToCharArray();
            transform[(chars[0] == '#', chars[1] == '#', chars[2] == '#', chars[3] == '#', chars[4] == '#')] =
                buffer[1] == "#";
        }
        for (var i = 0; i < 20; i++)
        {
            DoStep(ref status, ref transform, ref firstIdx);
        }

        var sum = CalcSum(ref status, firstIdx);
        Console.WriteLine("Task 01: " + sum);
        
        long localSum = 0;
        long prevSum = 0;
        for (var i = 0; i < 980; i++)
        {
            if (i >= 880) prevSum = CalcSum(ref status, firstIdx);
            DoStep(ref status, ref transform, ref firstIdx);
            if (i >= 880) localSum += CalcSum(ref status, firstIdx) - prevSum;
        }

        var avgInc = localSum / 100;
        sum = CalcSum(ref status, firstIdx) + (50000000000 - 1000) * avgInc;
        Console.WriteLine("Task 02: "+ sum);
    }

    private static long CalcSum(ref LinkedList<bool> status, int idx)
    {
        var sum = 0;
        var idxLocal = idx;
        var current = status.First;
        while (current != null)
        {
            if (current.Value) sum += idxLocal;
            idxLocal++;
            current = current.Next;
        }

        return sum;
    }

    private static void DoStep(ref LinkedList<bool> status, ref Dictionary<(bool, bool, bool, bool, bool), bool> transform, ref int idx)
    {
        var current = status.First!.Next!.Next!;
        var tmpList = new LinkedList<bool>();
        while (current.Next!.Next != null)
        {
            var nextState = transform.GetValueOrDefault(
                (current.Previous!.Previous!.Value, current.Previous.Value, current.Value, current.Next.Value,
                    current.Next.Next.Value), false);
            tmpList.AddLast(nextState);
            current = current.Next;
        }

        status = tmpList;
        status.AddFirst(false);
        status.AddFirst(false);
        status.AddLast(false);
        status.AddLast(false);

        if (status.Last!.Previous!.Previous!.Value) status.AddLast(false);
        if (!status.First!.Next!.Next!.Value) return;
        status.AddFirst(false);
        idx--;
    }

    private static void PrintState(LinkedList<bool> state)
    {
        var current = state.First;
        var output = "";
        while (current != null)
        {
            var symbol = current.Value ? "#" : ".";
            output += symbol;
            current = current.Next;
        }
        Console.WriteLine(output);
    }
    
}