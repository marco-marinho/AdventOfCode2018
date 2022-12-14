using static Helpers.Reader;

namespace Day07;

internal static class Day07
{
    private static void Main()
    {
        Task01();
        Task02();
    }

    private static void Task02()
    {
        const int timeStep = 60;
        const int numElfs = 5;
        var totalTime = 0;
        var (dependenceList, allSteps, ready) = GetData();
        var done = new HashSet<char>();
        var working = new Dictionary<char, int>();
        while (done.Count < allSteps.Count)
        {
            while (working.Count < numElfs && ready.Count > 0)
            {
                var next = ready.Min();
                var time = next - 'A' + 1 + timeStep;
                working[next] = time;
                ready.Remove(next);
            }

            var timeSpent = working.Values.Min();
            totalTime += timeSpent;
            foreach (var entry in working.Where(entry => entry.Value - timeSpent == 0))
            {
                done.Add(entry.Key);
                working.Remove(entry.Key);
            }

            foreach (var entry in working.Keys) working[entry] -= timeSpent;
            
            var toRemove = new List<char>();
            foreach (var entry in dependenceList.Where(entry => entry.Value.All(step => done.Contains(step))))
            {
                ready.Add(entry.Key);
                toRemove.Add(entry.Key);
            }

            foreach (var entry in toRemove)
            {
                dependenceList.Remove(entry);
            }
        }

        Console.WriteLine("Task 02: " + totalTime);
    }

    private static void Task01()
    {
        var (dependenceList, allSteps, ready) = GetData();
        var done = new HashSet<char>();
        var doneOrder = new List<char>();
        while (done.Count < allSteps.Count)
        {
            var next = ready.Min();
            done.Add(next);
            ready.Remove(next);
            doneOrder.Add(next);
            var toRemove = new List<char>();
            foreach (var entry in dependenceList)
            {
                if (!entry.Value.All(step => done.Contains(step))) continue;
                ready.Add(entry.Key);
                toRemove.Add(entry.Key);
            }

            foreach (var entry in toRemove)
            {
                dependenceList.Remove(entry);
            }
        }

        Console.WriteLine("Task 01: " + new string(doneOrder.ToArray()));
    }

    private static Tuple<Dictionary<char, HashSet<char>>, HashSet<char>, HashSet<char>> GetData()
    {
        var data = ReadFile("Data\\Day07.txt");
        var dependenceList = new Dictionary<char, HashSet<char>>();
        var allSteps = new HashSet<char>();
        foreach (var line in data)
        {
            var step = line[36];
            var dependence = line[5];
            allSteps.Add(step);
            allSteps.Add(dependence);
            if (!dependenceList.ContainsKey(step)) dependenceList[step] = new HashSet<char>();
            dependenceList[step].Add(dependence);
        }

        var ready = new HashSet<char>();
        foreach (var step in allSteps.Where(step => !dependenceList.ContainsKey(step)))
        {
            ready.Add(step);
        }

        return Tuple.Create(dependenceList, allSteps, ready);
    }
}