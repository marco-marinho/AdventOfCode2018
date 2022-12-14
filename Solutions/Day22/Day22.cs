using Helpers;

namespace Day22;

internal static class Day22
{
    private static void Main()
    {
        const int depth = 3066;
        var (xTarget, yTarget) = (13, 726);
        var (xLimit, yLimit) = (800, 800);
        var grid = ArrayHelper.GetNew2DArray(xLimit, yLimit, 0);
        var type = ArrayHelper.GetNew2DArray(xLimit, yLimit, '.');
        for (var col = 1; col < yLimit; col++)
        {
            for (var x = 0; x < col; x++)
            {
                grid[x, col] = (CalcErosion(grid, x, col, (xTarget, yTarget)) + depth) % 20183;
                FillType(ref type, grid[x, col], col, x);
            }

            for (var y = 0; y <= col; y++)
            {
                grid[col, y] = (CalcErosion(grid, col, y, (xTarget, yTarget)) + depth) % 20183;
                FillType(ref type, grid[col, y], y, col);
            }
        }

        type[yTarget, xTarget] = 'T';
        Console.WriteLine("Task 01: " + CalcRisk(type, xTarget, yTarget));
        Console.WriteLine("Task 02: " + Djkistras(type, xTarget, yTarget));
    }

    private static int Djkistras(char[,] status, int y, int x)
    {
        var distances = new Dictionary<((int, int), char), int>();
        var handler = new MoveHandler();
        var toVisit = new Dictionary<((int, int), char), int> { [((0, 0), 't')] = 0 };
        var toVisitKey = new PriorityQueue<((int, int), char), int>(Comparer<int>.Create((a, b) => a - b));
        toVisitKey.Enqueue(((0, 0), 't'), 0);

        var changes = new[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
        while (toVisit.Count > 0)
        {
            var (curPos, curGear) = toVisitKey.Dequeue();
            if (!toVisit.ContainsKey(((curPos), curGear))) continue;
            var curDist = toVisit[(curPos, curGear)];
            if (curPos.Item1 == x && curPos.Item2 == y) return curDist;
            distances[(curPos, curGear)] = curDist;
            toVisit.Remove((curPos, curGear));
            foreach (var change in changes)
            {
                var (xNext, yNext) = (curPos.Item1 + change.Item1, curPos.Item2 + change.Item2);
                if (xNext < 0 || yNext < 0 || xNext >= status.GetLength(0) || yNext >= status.GetLength(1)) continue;
                var moves = handler.GetMoves(status[curPos.Item1, curPos.Item2], status[xNext, yNext], curGear);
                foreach (var move in moves)
                {
                    if (distances.ContainsKey(((xNext, yNext), move.Item2))) continue;
                    var currEntry = toVisit.GetValueOrDefault(((xNext, yNext), move.Item2), int.MaxValue);
                    var tentativeDist = move.Item1 + curDist;
                    if (currEntry <= tentativeDist) continue;
                    toVisit[((xNext, yNext), move.Item2)] = tentativeDist;
                    toVisitKey.Enqueue(((xNext, yNext), move.Item2), tentativeDist);
                }
            }
        }

        return int.MaxValue;
    }

    private class MoveHandler
    {
        private readonly Dictionary<(char, char, char), List<(int, char)>> _movesMem;
        private static readonly char[] PossibleItems = { 't', 'n', 'g' };

        public MoveHandler()
        {
            _movesMem = new Dictionary<(char, char, char), List<(int, char)>>();
        }

        public List<(int, char)> GetMoves(char current, char next, char equip)
        {
            if (_movesMem.ContainsKey((current, next, equip)))
            {
                return _movesMem[(current, next, equip)];
            }

            var buff = (from item in PossibleItems
                where CanMove(current, item) && CanMove(next, item)
                select item == equip ? (1, item) : (8, item)).ToList();
            _movesMem[(current, next, equip)] = buff;
            return buff;
        }

        private static bool CanMove(char next, char equip)
        {
            var canMove = next switch
            {
                '.' => equip is 't' or 'g',
                '=' => equip is 'g' or 'n',
                '|' => equip is 't' or 'n',
                'T' => equip is 't',
                _ => false
            };
            return canMove;
        }
    }

    private static int CalcRisk(char[,] status, int y, int x)
    {
        var sum = 0;
        for (var j = 0; j <= x; j++)
        {
            for (var i = 0; i <= y; i++)
            {
                switch (status[j, i])
                {
                    case '=':
                        sum += 1;
                        break;
                    case '|':
                        sum += 2;
                        break;
                }
            }
        }

        return sum;
    }

    private static int CalcErosion(int[,] grid, int x, int y, (int, int) target)
    {
        if ((x == 0 && y == 0) || (x == target.Item1 && y == target.Item2)) return 0;
        if (x == 0) return y * 48271;
        if (y == 0) return x * 16807;
        return grid[x - 1, y] * grid[x, y - 1];
    }

    private static void FillType(ref char[,] types, int erosion, int x, int y)
    {
        switch (erosion % 3)
        {
            case 0:
                types[x, y] = '.';
                break;
            case 1:
                types[x, y] = '=';
                break;
            case 2:
                types[x, y] = '|';
                break;
        }
    }
}