using static Helpers.Reader;

namespace Day03;

internal static class Day03
{
    private static void Main()
    {
        var data = ReadFile("Data/Day03.txt");
        var squares = new List<Square>();
        foreach (var line in data)
        {
            var buff = line.Replace("#", "").Replace(" @ ", "|").Replace(": ", "|").Replace("x", ",");
            var pieces = buff.Split("|");
            var id = int.Parse(pieces[0]);
            var start = Array.ConvertAll(pieces[1].Split(","), int.Parse);
            var len = Array.ConvertAll(pieces[2].Split(","), int.Parse);
            squares.Add(new Square(id, start[0], start[1], len[0], len[1]));
        }

        var intersection = new HashSet<string>();
        var overlapping = new HashSet<int>();
        for (var outer = 0; outer < squares.Count - 1; outer++)
        {
            for (var inner = outer + 1; inner < squares.Count; inner++)
            {
                var buffIntersection = squares[outer].GetIntersection(squares[inner]);
                if (buffIntersection.Count > 0)
                {
                    overlapping.Add(squares[outer].Id);
                    overlapping.Add(squares[inner].Id);
                }

                foreach (var point in buffIntersection)
                {
                    intersection.Add(point);
                }
            }
        }

        var nonOverlapping = -1;
        for (var idx = 1; idx < data.Length+1; idx++)
        {
            if (overlapping.Contains(idx)) continue;
            nonOverlapping = idx;
        }

        Console.WriteLine("Task 01: "+intersection.Count);
        Console.WriteLine("Task 02: "+nonOverlapping);
    }
}

internal class Square
{
    private readonly int _xStart;
    private readonly int _xEnd;
    private readonly int _yStart;
    private readonly int _yEnd;
    public readonly int Id;

    public Square(int id, int startX, int startY, int lenX, int lenY)
    {
        Id = id;
        _xStart = startX;
        _yStart = startY;
        _xEnd = startX + lenX;
        _yEnd = startY + lenY;
    }

    public List<string> GetIntersection(Square other)
    {
        var xStartTemp = Math.Max(other._xStart, _xStart);
        var xEndTemp = Math.Min(other._xEnd, _xEnd);
        var yStartTemp = Math.Max(other._yStart, _yStart);
        var yEndTemp = Math.Min(other._yEnd, _yEnd);
        var output = new List<string>();
        if (xStartTemp >= xEndTemp || yStartTemp >= yEndTemp) return output;
        for (var tmpX = xStartTemp; tmpX < xEndTemp; tmpX++)
        {
            for (var tmpY = yStartTemp; tmpY < yEndTemp; tmpY++)
            {
                output.Add(tmpX + "," + tmpY);
            }
        }

        return output;
    }
}