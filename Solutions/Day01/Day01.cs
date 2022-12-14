using static Helpers.Reader;

namespace Day01
{
    internal static class Day01
    {
        private static void Main()
        {
            var data = ReadFile("Data/Day01.txt");
            var found = new HashSet<int>();
            var frequency = data.Aggregate(0, (current, line) => current + int.Parse(line));

            var freqTwice = 0;
            var foundFlag = false;
            while (!foundFlag)
            {
                foreach (var line in data)
                {
                    var freqBuff = int.Parse(line);
                    freqTwice += freqBuff;
                    if (found.Contains(freqTwice))
                    {
                        foundFlag = true;
                        break;
                    }
                    found.Add(freqTwice);
                }
            }
            Console.WriteLine("Task 01: " + frequency);
            Console.WriteLine("Task 02: " + freqTwice);
        }
    }
}