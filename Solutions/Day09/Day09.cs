namespace Day09;

internal static class Day09
{
    private static void Main()
    {
        Console.WriteLine("Task 01: " + GetHighScore(411, 71170));
        Console.WriteLine("Task 02: " + GetHighScore(411, 7117000));
    }

    private static long GetHighScore(int numPlayers, int lastMarble)
    {
        var circle = new Circle();
        var curMarble = 1;
        var scores = new long[numPlayers];
        while (curMarble <= lastMarble)
        {
            scores[(curMarble - 1) % scores.Length] += circle.Insert(curMarble);
            curMarble++;
        }
        return scores.Max();
    }

    private class Circle
    {
        private readonly LinkedList<int> _marbles;
        private LinkedListNode<int>? _current;

        public Circle()
        {
            _marbles = new LinkedList<int>();
            _marbles.AddFirst(0);
            _current = _marbles.First!;
        }

        private void RotateClockWise()
        {
            _current = _current!.Next ?? _marbles.First;
        }

        private void RotateAntiClockWise()
        {
            _current = _current!.Previous ?? _marbles.Last;
        }

        public int Insert(int value)
        {
            if (value % 23 == 0)
            {
                for (var i = 0; i < 7; i++)
                {
                    RotateAntiClockWise();
                }

                var newNext = _current!.Next ?? _marbles.First;
                var score = value + _current.Value;
                _marbles.Remove(_current);
                _current = newNext;
                return score;
            }

            RotateClockWise();
            _marbles.AddAfter(_current!, value);
            _current = _current!.Next;
            return 0;
        }

        public void Print()
        {
            var buff = _marbles.First;
            while (buff != null)
            {
                Console.Write(buff.Value + " ");
                buff = buff.Next;
            }
            Console.Write("\n");
        }
    }
}