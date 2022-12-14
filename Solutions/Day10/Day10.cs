using Helpers;
using static Helpers.Reader;

namespace Day10;

internal static class Day10
{
    private static void Main()
    {
        var data = ReadFile("Data\\Day10.txt");
        var pixels = new List<Pixel>();
        foreach (var line in data)
        {
            var buffer = line.Replace("position=<", "").Replace("> velocity=<", ",").Replace(">", "").Split(",");
            var numBuffer = Array.ConvertAll(buffer, int.Parse);
            pixels.Add(new Pixel(numBuffer[0], numBuffer[1], numBuffer[2], numBuffer[3]));
        }

        var seconds = 0;
        var (_, _, yMin, yMax) = GetBounds(pixels);
        while (yMax - yMin != 9)
        {
            foreach (var pixel in pixels)
            {
                pixel.Walk();
            }
            (_, _, yMin, yMax) = GetBounds(pixels);
            seconds++;
        }
        Console.WriteLine("Task 01: ");
        PrintPixels(pixels);
        Console.WriteLine("Task 02: "+ seconds);
    }

    private static (int, int, int, int) GetBounds(List<Pixel> pixels)
    {
        var xMin = int.MaxValue;
        var yMin = int.MaxValue;
        var yMax = 0;
        var xMax = 0;
        foreach (var pixel in pixels)
        {
            if (pixel.X < xMin) xMin = pixel.X;
            if (pixel.Y < yMin) yMin = pixel.Y;
            if (pixel.X > xMax) xMax = pixel.X;
            if (pixel.Y > yMax) yMax = pixel.Y;
        }

        return (xMin, xMax, yMin, yMax);
    }

    private static void PrintPixels(List<Pixel> pixels)
    {
        var (xMin, xMax, yMin, yMax) = GetBounds(pixels);
        var toPrint = ArrayHelper.GetNew2DArray(yMax - yMin + 1, xMax - xMin + 1, ".");
        foreach (var pixel in pixels)
        {
            toPrint[pixel.Y - yMin, pixel.X - xMin] = "#";
        }

        for (var i = 0; i < toPrint.GetLength(0); i++)
        {
            Console.WriteLine(string.Join("", ArrayHelper.GetRow(toPrint, i)));
        }
    }

    private class Pixel : Point2D
    {
        private readonly int _xVel;
        private readonly int _yVel;

        public Pixel(int x, int y, int xVel, int yVel) : base(x, y)
        {
            _xVel = xVel;
            _yVel = yVel;
        }

        public void Walk()
        {
            X += _xVel;
            Y += _yVel;
        }
    }
}