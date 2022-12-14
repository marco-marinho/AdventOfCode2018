namespace Day14;

internal static class Day14
{
    private static void Main()
    {
        var numbers = new List<byte>();
        var idxFirst = 0;
        var idxSecond = 1;
        numbers.Add(3);
        numbers.Add(7);
        const int target = 760221;
        byte[] targetBytes = {7,6,0,2,2,1};
        var task1 = false;
        var task2 = false;

        while (!(task1 && task2))
        {
            var next = numbers[idxFirst] + numbers[idxSecond];
            var first = next / 10;
            var second = next % 10;
            if (first > 0)
            {
                numbers.Add((byte)first);
            }

            numbers.Add((byte)second);
            idxFirst = (numbers[idxFirst] + 1 + idxFirst) % numbers.Count;
            idxSecond = (numbers[idxSecond] + 1 + idxSecond) % numbers.Count;

            if (numbers.Count > target + 10 && !task1)
            {
                long score = 0;
                var offset = 0;
                for (long multiplier = 1000000000; multiplier > 0; multiplier /= 10)
                {
                    score += multiplier * numbers[target + offset];
                    offset++;
                }

                Console.WriteLine("Task 01: " + score);
                task1 = true;
            }

            if (numbers.Count < 8) continue;
            var foundFirst = true;
            var foundSecond = true;
            var offsetLen = numbers.Count - 7; 
            for (var idx = 0; idx < 6; idx++)
            {
                if (numbers[idx + offsetLen] != targetBytes[idx]) foundFirst = false;
                if (numbers[idx + offsetLen + 1] != targetBytes[idx]) foundSecond = false;
                if (!foundFirst && !foundSecond) break;
            }
            if (!foundFirst && !foundSecond) continue;
            if (foundFirst) Console.WriteLine("Task 02: " + (numbers.Count - 7));
            if (foundSecond) Console.WriteLine("Task 02: " + (numbers.Count - 6));
            task2 = true;
        }
    }
}