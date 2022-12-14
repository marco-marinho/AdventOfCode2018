using static Helpers.Reader;
using Helpers;

namespace Day16;

internal static class Day16
{
    private static void Main()
    {
        var data = ReadFile("Data/Day16_1.txt");
        var output = new List<(int, HashSet<string>)>();
        var cpu = new Cpu();
        for (var i = 0; i < data.Length; i += 4)
        {
            var initialState = (data[i][9] - '0', data[i][12] - '0', data[i][15] - '0', data[i][18] - '0');
            var endState = (data[i + 2][9] - '0', data[i + 2][12] - '0', data[i + 2][15] - '0', data[i + 2][18] - '0');
            var input = Array.ConvertAll(data[i + 1].Split(" "), int.Parse);
            var matchingInstructions = TryAllInstruction(initialState, endState, input);
            output.Add(matchingInstructions);
        }

        Console.WriteLine("Task 01: " + output.Count(entry => entry.Item2.Count >= 3));

        var opcodeTable = new Dictionary<int, Cpu.InstructionDelegate>();
        var instructionDict = cpu.InstructionDict();
        var found = new HashSet<string>();
        while (opcodeTable.Count < instructionDict.Count)
        {
            foreach (var entry in output)
            {
                if (entry.Item2.Except(found).Count() != 1 || opcodeTable.ContainsKey(entry.Item1)) continue;
                var instruction = entry.Item2.Except(found).First();
                opcodeTable[entry.Item1] = instructionDict[instruction];
                found.Add(instruction);
                break;
            }
        }

        var instructions = ReadFile("Data/Day16_2.txt");
        foreach (var instruction in instructions)
        {
            var buff = Array.ConvertAll(instruction.Split(" "), int.Parse);
            opcodeTable[buff[0]](buff[1], buff[2], buff[3]);
        }

        Console.WriteLine("Task 02: " + cpu.Registers[0]);
    }

    private static (int, HashSet<string>) TryAllInstruction((int, int, int, int) initialState,
        (int, int, int, int) endState, IReadOnlyList<int> input)
    {
        var cpu = new Cpu();
        var ops = new HashSet<string>();
        var instructions = cpu.Instructions();
        foreach (var instruction in instructions)
        {
            cpu.SetRegisters(initialState);
            instruction(input[1], input[2], input[3]);
            if (cpu.GetRegs() == endState)
            {
                ops.Add(instruction.Method.Name);
            }
        }

        return (input[0], ops);
    }
}
