using static Helpers.Reader;

namespace Day02
{
    internal static class Day02
    {
        private static void Main()
        {
            var data = ReadFile("Data/Day02.txt");
            Task01(data);
            Task02(data);
        }

        private static void Task01(string[] data)
        {
            var twoTimes = 0;
            var threeTimes = 0;
            foreach (var line in data)
            {
                var letterDict = new Dictionary<char, int>();
                foreach (var letter in line)
                {
                    letterDict[letter] = letterDict.GetValueOrDefault(letter, 0) + 1;
                }

                var valueSet = new HashSet<int>();
                foreach (var entry in letterDict)
                {
                    valueSet.Add(entry.Value);
                }

                twoTimes += valueSet.Contains(2) ? 1 : 0;
                threeTimes += valueSet.Contains(3) ? 1 : 0;
            }

            Console.WriteLine("Task 01: " + twoTimes * threeTimes);
        }

        private static void Task02(string[] data)
        {
            var found = false;
            var correctId = "";
            for (var outIdx = 0; outIdx < data.Length; outIdx++)
            {
                for (var inIdx = outIdx + 1; inIdx < data.Length; inIdx++)
                {
                    var first = data[outIdx];
                    var second = data[inIdx];
                    var different = first.Where((t, cIdx) => t != second[cIdx]).Count();
                    if (different != 1) continue;
                    correctId = first.Where((t, cIdx) => t == second[cIdx]).Aggregate(correctId, (current, t) => current + t);
                    found = true;
                    break;
                }

                if (found)
                {
                    break;
                }
            }
            Console.WriteLine("Task 02: " + correctId);
        }
    }
}