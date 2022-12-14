using Helpers;

namespace Day13;

internal static class Day13
{
    private static void Main()
    {
        var data = Reader.ReadFile("Data/Day13.txt");
        var map = ArrayHelper.GetNew2DArray(data.Length, data[0].Length, ' ');
        var cars = new List<Car>();
        for (var i = 0; i < data.Length; i++)
        {
            var buff = data[i].ToCharArray();
            for (var j = 0; j < buff.Length; j++)
            {
                switch (buff[j])
                {
                    case '<':
                        map[i, j] = '-';
                        cars.Add(new Car(new Point2D(i, j), 'l'));
                        break;
                    case '>':
                        map[i, j] = '-';
                        cars.Add(new Car(new Point2D(i, j), 'r'));
                        break;
                    case 'v':
                        map[i, j] = '|';
                        cars.Add(new Car(new Point2D(i, j), 'd'));
                        break;
                    case '^':
                        map[i, j] = '|';
                        cars.Add(new Car(new Point2D(i, j), 'u'));
                        break;
                    default:
                        map[i, j] = buff[j];
                        break;
                }
            }
        }

        var firstCollision = true;
        var (xCollision, yCollision) = (0, 0);
        while (cars.Count > 1)
        {
            cars.Sort();
            var toRemove = new List<Car>();
            foreach (var car in cars.Where(car => !toRemove.Contains(car)))
            {
                car.Move();
                var (collided, i, j) = car.CheckCollision(ref cars);
                var (x, y) = car.GetCoordinates();
                if (collided)
                {
                    if (firstCollision)
                    {
                        (xCollision, yCollision) = (x, y);
                        firstCollision = false;
                    }

                    toRemove.Add(cars[i]);
                    toRemove.Add(cars[j]);
                }

                car.CheckCurrent(map[x, y]);
            }

            foreach (var car in toRemove)
            {
                cars.Remove(car);
            }
        }

        var (xFinal, yFinal) = cars[0].GetCoordinates();
        Console.WriteLine("Task 01: " + yCollision + "," + xCollision);
        Console.WriteLine("Task 02: " + yFinal + "," + xFinal);
    }

    private static void PrintStatus(char[,] map, List<Car> cars)
    {
        var buffer = (char[,])map.Clone();
        foreach (var car in cars)
        {
            var (x, y) = car.GetCoordinates();
            buffer[x, y] = car.GetChar();
        }

        for (var i = 0; i < buffer.GetLength(0); i++)
        {
            var line = ArrayHelper.GetRow(buffer, i);
            Console.WriteLine(string.Join("", line));
        }
    }

    private class Car : IComparable<Car>
    {
        private readonly Point2D _position;
        private int _turnIdx;
        private readonly char[] _directions = { 'l', 'u', 'r', 'd' };
        private char _direction;

        public Car(Point2D coordinates, char direction)
        {
            _position = coordinates;
            _turnIdx = 0;
            _direction = direction;
        }

        public (int, int) GetCoordinates()
        {
            return (_position.X, _position.Y);
        }

        public char GetChar()
        {
            return _direction switch
            {
                'u' => '^',
                'd' => 'v',
                'l' => '<',
                'r' => '>',
                _ => 'X'
            };
        }

        public (bool, int, int) CheckCollision(ref List<Car> cars)
        {
            foreach (var car in cars.Where(car => car != this))
            {
                var (x, y) = car.GetCoordinates();
                if (x == _position.X && y == _position.Y)
                {
                    return (true, cars.IndexOf(this), cars.IndexOf(car));
                }
            }

            return (false, -1, -1);
        }

        private void TurnRight()
        {
            var newIdx = (Array.IndexOf(_directions, _direction) + 1) % 4;
            _direction = _directions[newIdx];
        }


        private void TurnLeft()
        {
            var newIdx = Array.IndexOf(_directions, _direction) - 1;
            newIdx = newIdx >= 0 ? newIdx : _directions.Length - 1;
            _direction = _directions[newIdx];
        }

        private void Turn()
        {
            switch (_turnIdx)
            {
                case 0:
                    _turnIdx += 1;
                    TurnLeft();
                    break;
                case 1:
                    _turnIdx += 1;
                    break;
                case 2:
                    _turnIdx = 0;
                    TurnRight();
                    break;
            }
        }

        public void Move()
        {
            switch (_direction)
            {
                case 'u':
                    _position.X -= 1;
                    break;
                case 'd':
                    _position.X += 1;
                    break;
                case 'r':
                    _position.Y += 1;
                    break;
                case 'l':
                    _position.Y -= 1;
                    break;
            }
        }

        public void CheckCurrent(char current)
        {
            if (current == '+') Turn();
            _direction = current switch
            {
                '/' => _direction switch
                {
                    'd' => 'l',
                    'u' => 'r',
                    'l' => 'd',
                    'r' => 'u',
                    _ => _direction
                },
                '\\' => _direction switch
                {
                    'd' => 'r',
                    'u' => 'l',
                    'l' => 'u',
                    'r' => 'd',
                    _ => _direction
                },
                _ => _direction
            };
        }

        public int CompareTo(Car? other)
        {
            if (other == null) throw new ArgumentNullException();
            var (otherX, otherY) = other.GetCoordinates();
            if (_position.X < otherX) return -1;
            if (_position.X > otherX) return 1;
            if (_position.Y < otherY) return -1;
            if (_position.Y > otherY) return 1;
            return 0;
        }
    }
}