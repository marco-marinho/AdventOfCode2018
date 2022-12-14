using static Helpers.Reader;

namespace Day08;

internal static class Day08{

    private static void Main()
    {
        var data = ReadFile("Data/Day08.txt")[0];

        var buffer = data.Split(" ");
        var (root, _) = Node.GenTree(buffer);
        Console.WriteLine("Task 01: " + root.AddMetadataTree());
        Console.WriteLine("Task 02: "+ root.GetValue());
    }

}

internal class Node
{
    private readonly List<Node> _children;
    private readonly int[] _metadata;

    private Node(int[] metadata, List<Node> children)
    {
        _metadata = metadata;
        _children = children;
    }

    public int GetValue()
    {
        return _children.Count == 0 ? AddMetadata() : _metadata.Where(idx => idx <= _children.Count).Sum(idx => _children[idx - 1].GetValue());
    }

    private int AddMetadata()
    {
        return _metadata.Sum();
    }

    public int AddMetadataTree()
    {
        return _metadata.Sum() + _children.Sum(node => node.AddMetadataTree());
    }

    public static (Node, string[]) GenTree(string[] input)
    {
        var numChildren = int.Parse(input[0]);
        var numMetadata = int.Parse(input[1]);
        var children = new List<Node>();
        input = input.Skip(2).Take(input.Length - 2).ToArray();
        for (var i = 0; i < numChildren; i++)
        {
            (var buffNode, input) = GenTree(input);
            children.Add(buffNode);
        }

        var metadata = Array.ConvertAll(input.Take(numMetadata).ToArray(), int.Parse);
        input = input.Skip(numMetadata).ToArray();
        return (new Node(metadata, children), input);
    }
    
}

