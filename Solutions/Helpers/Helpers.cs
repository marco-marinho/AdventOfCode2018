namespace Helpers
{
    public static class Reader
    {
        public static string[] ReadFile(string path)
        {
            var text = File.ReadAllText(path);
            var lines = text.Split("\n");
            var output = lines.Select(entry => entry.Replace("\r", "")).ToArray();
            return output;
        }
    }

    public class Point2D : IComparable<Point2D>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int ManhattanDistance(Point2D other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        public int CompareTo(Point2D? other)
        {
            if (other == null) throw new ArgumentNullException();
            if (X < other.X) return -1;
            if (X > other.X) return 1;
            if (Y < other.Y) return -1;
            if (Y > other.Y) return 1;
            return 0;
        }

        public (int, int) AsTuple()
        {
            return (X, Y);
        }

        public IEnumerable<Point2D> Neighbours()
        {
            yield return new Point2D(X - 1, Y);
            yield return new Point2D(X, Y - 1);
            yield return new Point2D(X, Y + 1);
            yield return new Point2D(X + 1, Y);
        }

        public override bool Equals(object? obj)
        {
            return obj is Point2D point2D && this == point2D;
        }

        public override int GetHashCode() => (X, Y).GetHashCode();

        public static bool operator ==(Point2D? lhs, Point2D? rhs)
        {
            if (lhs is null || rhs is null) return false;
            return rhs.X == lhs.X && rhs.Y == lhs.Y;
        }

        public static bool operator !=(Point2D lhs, Point2D rhs) => !(lhs == rhs);
    }

    public static class ArrayHelper
    {
        public static T[,] GetNew2DArray<T>(int x, int y, T initialValue)
        {
            T[,] nums = new T[x, y];
            for (int i = 0; i < x * y; i++) nums[i % x, i / x] = initialValue;
            return nums;
        }

        public static T[] GetColumn<T>(T[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                .Select(x => matrix[x, columnNumber])
                .ToArray();
        }

        public static T[] GetRow<T>(T[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                .Select(x => matrix[rowNumber, x])
                .ToArray();
        }
    }

    public class Cpu
    {
        public readonly Dictionary<int, int> Registers;

        public delegate void InstructionDelegate(int a, int b, int c);

        private int _ipRegister;

        private readonly Dictionary<string, InstructionDelegate> _instructionTable;

        private List<(string, int, int, int)> _rom;

        private int _breakPoint = -1;

        private bool _halted;

        public Cpu(int reg0 = 0, int reg1 = 0, int reg2 = 0, int reg3 = 0, int reg4 = 0, int reg5 = 0)
        {
            Registers = new Dictionary<int, int>
            {
                [0] = reg0,
                [1] = reg1,
                [2] = reg2,
                [3] = reg3,
                [4] = reg4,
                [5] = reg5,
            };
            _instructionTable = InstructionDictEx();
            _rom = new List<(string, int, int, int)>();
        }

        public void SetBreakPoint(int breakPoint)
        {
            _breakPoint = breakPoint;
        }

        public void BindIp(int reg)
        {
            _ipRegister = reg;
        }

        public void SetRom(List<(string, int, int, int)> rom)
        {
            _rom = rom;
        }

        public int ExecuteRom()
        {
            while (Registers[_ipRegister] < _rom.Count)
            {
                if (Registers[_ipRegister] == _breakPoint)
                {
                    if (!_halted)
                    {
                        _halted = true;
                        return Registers[4];
                    }

                    _halted = false;
                }
                var (inst, a, b, c) = _rom[Registers[_ipRegister]];
                _instructionTable[inst](a, b, c);
                Registers[_ipRegister] += 1;
            }
            return Registers[0];
        }

        public (int, int, int, int, int, int) GetRegsEx()
        {
            return (Registers[0], Registers[1], Registers[2], Registers[3], Registers[4], Registers[5]);
        }

        public (int, int, int, int) GetRegs()
        {
            return (Registers[0], Registers[1], Registers[2], Registers[3]);
        }

        public void SetRegisters((int, int, int, int) input)
        {
            Registers[0] = input.Item1;
            Registers[1] = input.Item2;
            Registers[2] = input.Item3;
            Registers[3] = input.Item4;
        }

        public List<InstructionDelegate> Instructions()
        {
            var output = new List<InstructionDelegate>
            {
                Addr, Addi, Mulr, Muli, Banr, Bani, Borr, Bori,
                Setr, Seti, Gtir, Gtri, Gtrr, Eqir, Eqri, Eqrr,
            };
            return output;
        }

        public Dictionary<string, InstructionDelegate> InstructionDict()
        {
            var buffer = Instructions();
            var output = new Dictionary<string, InstructionDelegate>();
            foreach (var entry in buffer)
            {
                output[entry.Method.Name] = entry;
            }

            return output;
        }
        public List<InstructionDelegate> InstructionsEx()
        {
            var output = new List<InstructionDelegate>
            {
                Addr, Addi, Mulr, Muli, Banr, Bani, Borr, Bori,
                Setr, Seti, Gtir, Gtri, Gtrr, Eqir, Eqri, Eqrr,
                Espc
            };
            return output;
        }

        public Dictionary<string, InstructionDelegate> InstructionDictEx()
        {
            var buffer = InstructionsEx();
            var output = new Dictionary<string, InstructionDelegate>();
            foreach (var entry in buffer)
            {
                output[entry.Method.Name] = entry;
            }

            return output;
        }

        private void Addr(int a, int b, int c)
        {
            Registers[c] = Registers[a] + Registers[b];
        }

        private void Addi(int a, int b, int c)
        {
            Registers[c] = Registers[a] + b;
        }

        private void Mulr(int a, int b, int c)
        {
            Registers[c] = Registers[a] * Registers[b];
        }

        private void Muli(int a, int b, int c)
        {
            Registers[c] = Registers[a] * b;
        }

        private void Banr(int a, int b, int c)
        {
            Registers[c] = Registers[a] & Registers[b];
        }

        private void Bani(int a, int b, int c)
        {
            Registers[c] = Registers[a] & b;
        }

        private void Borr(int a, int b, int c)
        {
            Registers[c] = Registers[a] | Registers[b];
        }

        private void Bori(int a, int b, int c)
        {
            Registers[c] = Registers[a] | b;
        }

        private void Setr(int a, int _, int c)
        {
            Registers[c] = Registers[a];
        }

        private void Seti(int a, int _, int c)
        {
            Registers[c] = a;
        }

        private void Gtir(int a, int b, int c)
        {
            Registers[c] = a > Registers[b] ? 1 : 0;
        }

        private void Gtri(int a, int b, int c)
        {
            Registers[c] = Registers[a] > b ? 1 : 0;
        }

        private void Gtrr(int a, int b, int c)
        {
            Registers[c] = Registers[a] > Registers[b] ? 1 : 0;
        }

        private void Eqir(int a, int b, int c)
        {
            Registers[c] = a == Registers[b] ? 1 : 0;
        }

        private void Eqri(int a, int b, int c)
        {
            Registers[c] = Registers[a] == b ? 1 : 0;
        }

        private void Eqrr(int a, int b, int c)
        {
            Registers[c] = Registers[a] == Registers[b] ? 1 : 0;
        }

        private void Espc(int a, int b, int c)
        {
                if (Registers[3] % Registers[5] == 0)
                {
                    Registers[0] += Registers[5];
                }

                Registers[2] = 0;
                Registers[1] += 1;
                Registers[_ipRegister] = 11;
        }
    }
}