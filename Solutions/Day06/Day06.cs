using static Helpers.Reader;
using Helpers;

namespace Day06;

internal static class Day06
{
    private static void Main()
    {
        var data = ReadFile("Data\\Day06.txt");
        var points = new List<Point2D>();
        var limX = new[] { int.MaxValue, 0 };
        var limY = new[] { int.MaxValue, 0 };
        foreach (var line in data)
        {
            var buff = line.Split(", ");
            var x = int.Parse(buff[0]);
            if (x < limX[0]) limX[0] = x;
            if (x > limX[1]) limX[1] = x;
            var y = int.Parse(buff[1]);
            if (y < limY[0]) limY[0] = y;
            if (y > limY[1]) limY[1] = y;
            points.Add(new Point2D(x, y));
        }

        var boundingBox = GetBoundingBox(limX[0] - 1, limX[1] + 1, limY[0] - 1, limY[1] + 1);
        var unboundedPoints = new HashSet<Point2D>();
        foreach (var bound in boundingBox)
        {
            var idx = -1;
            var min = int.MaxValue;
            for (var i = 0; i < points.Count; i++)
            {
                var dist = points[i].ManhattanDistance(new Point2D(bound[0], bound[1]));
                if (dist > min) continue;
                min = dist;
                idx = i;
            }

            unboundedPoints.Add(points[idx]);
        }

        var closest = new Dictionary<Point2D, int>();
        for (var x = limX[0]; x <= limX[1]; x++)
        {
            for (var y = limY[0]; y <= limY[1]; y++)
            {
                var dist = int.MaxValue;
                var tmpPoint = new Point2D(x, y);
                var closestList = new List<int>();
                for (var i = 0; i < points.Count; i++)
                {
                    if (tmpPoint.ManhattanDistance(points[i]) < dist)
                    {
                        closestList.Clear();
                        dist = tmpPoint.ManhattanDistance(points[i]);
                        closestList.Add(i);
                    }
                    else if (tmpPoint.ManhattanDistance(points[i]) == dist)
                    {
                        closestList.Add(i);
                    }
                }

                if (closestList.Count == 1)
                {
                    closest[points[closestList[0]]] = closest.GetValueOrDefault(points[closestList[0]], 0) + 1;
                }
            }
        }

        var max = 0;
        foreach (var entry in closest)
        {
            if (unboundedPoints.Contains(entry.Key)) continue;
            if (entry.Value > max) max = entry.Value;
        }

        Console.WriteLine("Task 01: " + max);

        var numPoints = 0;
        for (var x = limX[0]; x <= limX[1]; x++)
        {
            for (var y = limY[0]; y <= limY[1]; y++)
            {
                var curPoint = new Point2D(x, y);
                var totalDist = points.Sum(point => curPoint.ManhattanDistance(point));
                if (totalDist < 10000) numPoints += 1;
            }
        }

        Console.WriteLine("Task 02: " + numPoints);
    }

    private static List<int[]> GetBoundingBox(int minX, int maxX, int minY, int maxY)
    {
        var output = new List<int[]>();
        for (var x = minX; x <= maxX; x++)
        {
            output.Add(new[] { x, minY });
            output.Add(new[] { x, maxY });
        }

        for (var y = minY; y <= maxY; y++)
        {
            output.Add(new[] { minX, y });
            output.Add(new[] { maxX, y });
        }

        return output;
    }
}