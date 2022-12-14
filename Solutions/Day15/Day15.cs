using Helpers;
using static Helpers.Reader;

namespace Day15;

internal static class Day15
{
    private static void Main()
    {
        var attackPower = 3;
        Console.WriteLine("Task 01: " + RunSimulation(attackPower, false));
        var res = -1;
        while (res == -1)
        {
            attackPower++;
            res = RunSimulation(attackPower, true);
        }

        Console.WriteLine("Task 02: " + res);
    }

    private static int RunSimulation(int attackPower, bool breakOnElvenDeath, bool printSteps = false)
    {
        var data = ReadFile("Data/Day15.txt");
        var map = ArrayHelper.GetNew2DArray(data.Length, data[0].Length, true);
        var units = new List<Unit>();

        for (var i = 0; i < data.Length; i++)
        {
            for (var j = 0; j < data[i].Length; j++)
            {
                map[i, j] = data[i][j] != '#';
                if (data[i][j] == 'G') units.Add(new Unit(i, j, 0, 3));
                if (data[i][j] == 'E') units.Add(new Unit(i, j, 1, attackPower));
            }
        }

        var end = false;
        var turns = 0;
        while (!end)
        {
            if (printSteps) PrintStatus(units, map);
            var actionOrder = ActionOrder(units);
            var dead = new List<Unit>();
            foreach (var curUnit in actionOrder.Where(idx => !dead.Contains(idx)))
            {
                if (!HasTarget(curUnit, units))
                {
                    end = true;
                    break;
                }

                var attacked = Attack(curUnit, ref units, ref dead);
                if (attacked) continue;
                Move(curUnit, units, map);
                Attack(curUnit, ref units, ref dead);
            }

            if (breakOnElvenDeath && dead.Any(unit => unit.Team == 1))
            {
                return -1;
            }

            if (end) break;
            turns++;
        }

        if (printSteps) PrintStatus(units, map);
        var totalHp = units.Sum(unit => unit.HitPoints);
        return turns * totalHp;
    }

    private static bool HasTarget(Unit unit, List<Unit> units)
    {
        return units.Any(other => other.Team != unit.Team);
    }

    private static bool Attack(Unit curUnit, ref List<Unit> units, ref List<Unit> dead)
    {
        var target = AttackTarget(curUnit, units);
        if (target == null) return false;
        target.HitPoints -= curUnit.AttackPower;
        if (target.HitPoints > 0) return true;
        dead.Add(target);
        units.Remove(target);
        return true;
    }

    private static bool Move(Unit curUnit, List<Unit> units, bool[,] map)
    {
        var moved = false;
        var toTry = curUnit.Neighbours();
        var inRange = GetInRange(curUnit, units, map);
        var dist = int.MaxValue;
        while (dist == int.MaxValue && inRange.Count > 0)
        {
            var closestList = GetClosest(curUnit.Position, ref inRange, units, map);
            var targets = closestList.Select(index => inRange[index]).ToList();
            targets.Sort();
            var idx = -1;
            var nextPos = new Point2D(0, 0);
            foreach (var target in targets)
            {
                foreach (var test in toTry)
                {
                    var distBuff = Dijkstras(test, target, units, map, false, dist);
                    if (dist <= distBuff) continue;
                    dist = distBuff;
                    idx = 1;
                    nextPos = test;
                }

                if (idx != -1) break;
            }

            if (idx != -1)
            {
                curUnit.Position = nextPos;
                moved = true;
            }
            else RemoveByIdx(ref inRange, closestList);
        }

        return moved;
    }

    private static Unit? AttackTarget(Unit attacker, List<Unit> units)
    {
        var posAttacks = attacker.Neighbours().ToArray();
        var foundHp = new[] { int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue };
        var foundOthers = new Unit?[] { null, null, null, null };
        foreach (var other in units.Where(other => other != attacker))
        {
            for (var i = 0; i < posAttacks.Length; i++)
            {
                if (other.Position != posAttacks[i] || other.Team == attacker.Team) continue;
                foundHp[i] = other.HitPoints;
                foundOthers[i] = other;
            }
        }

        var minHp = int.MaxValue;
        var idx = -1;
        for (var i = 0; i < foundHp.Length; i++)
        {
            if (foundHp[i] >= minHp) continue;
            minHp = foundHp[i];
            idx = i;
        }

        return idx == -1 ? null : foundOthers[idx];
    }

    private static void PrintStatus(List<Unit> units, bool[,] map)
    {
        var repr = ArrayHelper.GetNew2DArray(map.GetLength(0), map.GetLength(1), ' ');
        for (var i = 0; i < map.GetLength(0); i++)
        {
            for (var j = 0; j < map.GetLength(1); j++)
            {
                repr[i, j] = map[i, j] ? '.' : '#';
            }
        }

        foreach (var unit in units)
        {
            repr[unit.Position.X, unit.Position.Y] = unit.Team == 0 ? 'G' : 'E';
        }

        Console.WriteLine("");
        for (var i = 0; i < map.GetLength(0); i++)
        {
            var buff = ArrayHelper.GetRow(repr, i);
            Console.WriteLine(string.Join("", buff));
        }

        Console.WriteLine("");
    }

    private static List<Unit> ActionOrder(IReadOnlyList<Unit> units)
    {
        var positions = units.Select(unit => unit.Position).ToList();
        positions.Sort();
        var actionOrder = new List<Unit>();
        foreach (var position in positions)
        {
            for (var i = 0; i < units.Count; i++)
            {
                if (position == units[i].Position)
                {
                    actionOrder.Add(units[i]);
                }
            }
        }

        return actionOrder;
    }

    private static int Dijkstras(Point2D start, Point2D destination, IEnumerable<Unit> units, bool[,] map,
        bool ignoreStart, int max)
    {
        if (start == destination)
        {
            return 0;
        }

        if (map.Clone() is not bool[,] mapBuff) return int.MaxValue;
        foreach (var unit in units)
        {
            if (start == unit.Position && ignoreStart) continue;
            mapBuff[unit.Position.X, unit.Position.Y] = false;
        }

        if (!mapBuff[start.X, start.Y]) return int.MaxValue;

        var toVisit = new Dictionary<Point2D, int>
        {
            [start] = 0
        };
        var visited = new HashSet<Point2D>();

        while (toVisit.Count > 0)
        {
            var next = toVisit.MinBy(element => element.Value).Key;
            var curDist = toVisit[next];
            if (curDist > max) return int.MaxValue;
            if (next == destination) return curDist;
            foreach (var buffPoint in next.Neighbours())
            {
                if (!mapBuff[buffPoint.X, buffPoint.Y]) continue;
                var distance = toVisit.GetValueOrDefault(buffPoint, int.MaxValue);
                if (distance <= curDist + 1 || visited.Contains(buffPoint)) continue;
                toVisit[buffPoint] = curDist + 1;
                visited.Add(buffPoint);
            }

            toVisit.Remove(next);
        }

        return int.MaxValue;
    }


    private static List<int> GetClosest(Point2D start, ref List<Point2D> others, IReadOnlyCollection<Unit> units, bool[,] map)
    {
        var minDist = int.MaxValue;
        var closestList = new List<int>();
        for (var i = 0; i < others.Count; i++)
        {
            if (start.ManhattanDistance(others[i]) > minDist) continue;
            var distBuff = Dijkstras(start, others[i], units, map, true, minDist);
            if (distBuff < minDist)
            {
                minDist = distBuff;
                closestList.Clear();
                closestList.Add(i);
            }
            else if (distBuff == minDist)
            {
                closestList.Add(i);
            }
        }

        return closestList;
    }

    private static List<Point2D> GetInRange(Unit unit, IReadOnlyCollection<Unit> unitList, bool[,] map)
    {
        var output = new List<Point2D>();
        foreach (var other in unitList.Where(other => other != unit).Where(other => other.Team != unit.Team))
        {
            output.AddRange(other.Neighbours());
        }

        var toRemove = new List<int>();
        for (var i = 0; i < output.Count; i++)
        {
            var (x, y) = output[i].AsTuple();
            if (!map[x, y])
            {
                toRemove.Add(i);
                continue;
            }

            if (unitList.Any(other => other.IsInPosition(x, y)))
            {
                toRemove.Add(i);
            }
        }

        RemoveByIdx(ref output, toRemove);

        output.Sort();
        return output;
    }

    private static void RemoveByIdx<T>(ref List<T> target, IEnumerable<int> toRemove)
    {
        foreach (var idx in toRemove.OrderByDescending(v => v))
        {
            target.RemoveAt(idx);
        }
    }

    private class Unit : IComparable<Unit>
    {
        public Point2D Position { get; set; }
        public byte Team { get; }

        public int HitPoints { get; set; }

        public int AttackPower { get; }

        public Unit(int x, int y, byte team, int attackPower)
        {
            Position = new Point2D(x, y);
            Team = team;
            HitPoints = 200;
            AttackPower = attackPower;
        }

        public bool IsInPosition(int x, int y)
        {
            return Position.X == x && Position.Y == y;
        }

        public IEnumerable<Point2D> Neighbours()
        {
            return Position.Neighbours();
        }

        public int CompareTo(Unit? other)
        {
            if (other == null) throw new ArgumentNullException();
            return Position.CompareTo(other.Position);
        }
    }
}