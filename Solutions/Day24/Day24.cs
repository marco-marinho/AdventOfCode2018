using System.Collections;
using System.Text.RegularExpressions;
using static Helpers.Reader;

namespace Day24
{
    internal static class Day24
    {
        private static void Main()
        {

            Console.WriteLine("Task 01: " + Run()["Infection"].Aggregate(0, (total, next) => total + next.Units));

            var boost = 1;
            var result = Run(boost);
            while (result["Infection"].Count != 0)
            {
                result = Run(++boost);
            }
            Console.WriteLine("Task 02: " + result["Immune System"].Aggregate(0, (total, next) => total + next.Units));
            
        }

        private static Dictionary<string, List<Army>> Run(int boost = 0)
        {
            var armies = GetArmies(boost);
            var attacked = true;
            while (armies["Infection"].Count > 0 && armies["Immune System"].Count > 0 && attacked)
            {
                FindTarget(armies);
                attacked = Attack(armies);
                CleanUpDead(armies);
            }

            return armies;
        }

        private static Dictionary<string, List<Army>> GetArmies(int boost = 0)
        {
            var regEx = new Regex(
                @"^(?<units>[0-9]+).+?(?<hp>[0-9]+) hit points (\((?<first>.+?;)?(?<second>.+?)\))?.+?(?<dmf>[0-9]+)\s(?<type>.+?)\s.+?(?<initiative>[0-9]+)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var data = ReadFile("Data/Day24.txt");
            var armies = new Dictionary<string, List<Army>>();
            var curArmie = "";
            foreach (var line in data)
            {
                if (line.Length == 0) continue;
                if (line.Contains(':'))
                {
                    curArmie = line.Replace(":", "");
                    armies[curArmie] = new List<Army>();
                    continue;
                }

                var matches = regEx.Match(line);
                var groups = matches.Groups;
                var units = int.Parse(groups["units"].Value);
                var hp = int.Parse(groups["hp"].Value);
                var first = groups["first"].Value;
                var second = groups["second"].Value;
                var dmg = int.Parse(groups["dmf"].Value);
                var type = groups["type"].Value;
                var initiative = int.Parse(groups["initiative"].Value);
                var buffBoost = curArmie == "Immune System" ? boost : 0;
                armies[curArmie].Add(new Army(units, hp, dmg + buffBoost, initiative, type, first, second));
            }

            return armies;
        }

        private static void PrintStatus(Dictionary<string, List<Army>> armies)
        {
            Console.WriteLine("Immune System");
            foreach (var army in armies["Immune System"])
            {
                Console.WriteLine(army.Units);
            }

            Console.WriteLine("Infection");
            foreach (var army in armies["Infection"])
            {
                Console.WriteLine(army.Units);
            }
        }

        private static void FindTarget(Dictionary<string, List<Army>> armies)
        {
            armies["Immune System"].Sort(Army.SortTargetingOrder());
            foreach (var army in armies["Immune System"])
            {
                army.ChooseTarget(armies["Infection"]);
            }

            armies["Infection"].Sort(Army.SortTargetingOrder());
            foreach (var army in armies["Infection"])
            {
                army.ChooseTarget(armies["Immune System"]);
            }
        }

        private static bool Attack(Dictionary<string, List<Army>> armies)
        {
            var killed = false;
            var attackOrder = new List<Army>();
            attackOrder.AddRange(armies["Immune System"]);
            attackOrder.AddRange(armies["Infection"]);
            attackOrder.Sort(Army.SortAttackOrder());
            foreach (var army in attackOrder)
            {
                killed |= army.Attack();
            }

            return killed;
        }

        private static void CleanUpDead(Dictionary<string, List<Army>> armies)
        {
            var toRemove = armies["Immune System"].Where(army => army.Units <= 0).ToList();
            foreach (var dead in toRemove)
            {
                armies["Immune System"].Remove(dead);
            }

            toRemove = armies["Infection"].Where(army => army.Units <= 0).ToList();
            foreach (var dead in toRemove)
            {
                armies["Infection"].Remove(dead);
            }
        }

        private class Army
        {
            public int Units { get; private set; }
            private readonly int _hp;
            private readonly HashSet<string> _weakness;
            private readonly HashSet<string> _immunity;
            private readonly int _dmg;
            private Army? Target {get; set;}
            private readonly string _type;
            private readonly int _initiative;
            private int EffectivePow { get; set; }
            private bool IsTargeted { get; set; }

            public Army(int units, int hp, int dmg, int initiative, string type, string first, string second)
            {
                Units = units;
                _hp = hp;
                _dmg = dmg;
                _initiative = initiative;
                _type = type;
                _weakness = new HashSet<string>();
                _immunity = new HashSet<string>();
                EffectivePow = Units * _dmg;
                IsTargeted = false;
                Target = null;
                ProcessString(first);
                ProcessString(second);
            }

            private void UpdatePower()
            {
                EffectivePow = Units * _dmg;
            }

            public bool Attack()
            {
                if (Target == null) return false;
                var multiplies = Target._weakness.Contains(_type) ? 2 : 1;
                var killed = (multiplies * EffectivePow) / Target._hp;
                Target.Units -= killed;
                Target.UpdatePower();
                Target.IsTargeted = false;
                Target = null;
                return killed > 0;
            }

            private class SortTargetingOrderHelper : IComparer<Army>
            {
                public int Compare(Army? first, Army? second)
                {
                    if (first == null || second == null) return 0;
                    if (first.EffectivePow > second.EffectivePow) return -1;
                    if (first.EffectivePow < second.EffectivePow) return 1;
                    if (first._initiative > second._initiative) return -1;
                    return 1;
                }
            }

            public static IComparer<Army> SortTargetingOrder()
            {
                return new SortTargetingOrderHelper();
            }

            private class SortAttackOrderHelper : IComparer<Army>
            {
                public int Compare(Army? first, Army? second)
                {
                    if (first == null || second == null) return 0;
                    if (first._initiative > second._initiative) return -1;
                    return 1;
                }
            }

            public static IComparer<Army> SortAttackOrder()
            {
                return new SortAttackOrderHelper();
            }

            public void ChooseTarget(List<Army> enemies)
            {
                Target = null;
                var maxDamage = 1;
                var maxEffectivePow = 0;
                var maxInitiative = 0;
                foreach (var enemy in enemies)
                {
                    if (enemy.IsTargeted) continue;
                    if (enemy._immunity.Contains(_type)) continue;
                    var buffPow = EffectivePow;
                    if (enemy._weakness.Contains(_type)) buffPow *= 2;
                    if (buffPow > maxDamage)
                    {
                        maxDamage = buffPow;
                        maxEffectivePow = enemy.EffectivePow;
                        maxInitiative = enemy._initiative;
                        Target = enemy;
                    }
                    else if (buffPow == maxDamage && enemy.EffectivePow > maxEffectivePow)
                    {
                        maxEffectivePow = enemy.EffectivePow;
                        maxInitiative = enemy._initiative;
                        Target = enemy;
                    }
                    else if (buffPow == maxDamage && enemy.EffectivePow == maxEffectivePow &&
                             enemy._initiative > maxInitiative)
                    {
                        maxInitiative = enemy._initiative;
                        Target = enemy;
                    }
                }

                if (Target != null) Target.IsTargeted = true;
            }

            private void ProcessString(string input)
            {
                if (input.Contains("weak"))
                {
                    input = input.Replace("weak to ", "").Replace(",", "").Replace(";", "");
                    var buff = input.Split(" ");
                    foreach (var entry in buff)
                    {
                        _weakness.Add(entry);
                    }

                    return;
                }

                if (input.Contains("immune"))
                {
                    input = input.Replace("immune to ", "").Replace(",", "").Replace(";", "");
                    var buff = input.Split(" ");
                    foreach (var entry in buff)
                    {
                        _immunity.Add(entry);
                    }
                }
            }
        }
    }
}