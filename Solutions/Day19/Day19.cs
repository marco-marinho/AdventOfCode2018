using static Helpers.Reader;
using Helpers;
namespace Day19;

internal static class Day16
{
    private static void Main()
    {
        var data = ReadFile("Data/Day19.txt");
        var ipBind = data[0][4] - '0';
        var ROM = new List<(string, int, int, int)>();
        foreach (var line in data.Skip(1))
        {
            var buff = line.Split(" ");
            ROM.Add((char.ToUpper(buff[0][0]) + buff[0][1..], int.Parse(buff[1]), int.Parse(buff[2]), int.Parse(buff[3])));
        }
        
        var cpu = new Cpu();
        cpu.BindIp(ipBind);
        cpu.SetRom(ROM);
        Console.WriteLine("Task 01: " + cpu.ExecuteRom());
        
        cpu = new Cpu
        {
            Registers =
            {
                [0] = 1
            }
        };
        cpu.BindIp(ipBind);
        cpu.SetRom(ROM);
        Console.WriteLine("Task 02: " + cpu.ExecuteRom());
    }
}

