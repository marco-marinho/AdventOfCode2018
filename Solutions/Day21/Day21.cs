using Helpers;
using static Helpers.Reader;

namespace Day21;

internal static class Day21
{
    private static void Main()
    {
        var data = ReadFile("Data/Day21.txt");
        var ipBind = data[0][4] - '0';
        var ROM = new List<(string, int, int, int)>();
        foreach (var line in data.Skip(1))
        {
            var buff = line.Split(" ");
            ROM.Add((char.ToUpper(buff[0][0]) + buff[0][1..], int.Parse(buff[1]), int.Parse(buff[2]),
                int.Parse(buff[3])));
        }

        var cpu = new Cpu();
        cpu.BindIp(ipBind);
        cpu.SetRom(ROM);
        cpu.SetBreakPoint(28);
        var options = new HashSet<int>();
        var first = cpu.ExecuteRom();
        options.Add(first);
        Console.WriteLine("Task 01: " + first);
        first = cpu.ExecuteRom();
        var last = first;
        while (!options.Contains(first))
        {
            options.Add(first);
            last = first;
            first = cpu.ExecuteRom();
        }

        Console.WriteLine("Task 02: " + last);
    }
}