using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day13 : IDay
{

    public void Run(string input)
    {
        var machines = ParseRows(input);
        decimal sum = 0;
        foreach (var m in machines)
        {
            var t = TokensIntersect(m);
            if (t < decimal.MaxValue)
            {
                sum += t;
            }
        }
        Console.WriteLine(sum);
    }
    public decimal TokensIntersect(Machine m)
    {

        decimal delta = m.A.Item1 * m.B.Item2 - m.A.Item2 * m.B.Item1;
        if (delta == 0)
            return long.MaxValue;
        decimal a = (m.B.Item2 * m.Prize.Item1 - m.B.Item1 * m.Prize.Item2) / delta;
        decimal b = (m.A.Item1 * m.Prize.Item2 - m.A.Item2 * m.Prize.Item1) / delta;
        if (a % 1 != 0)
            return decimal.MaxValue;
        if (b % 1 != 0)
            return decimal.MaxValue;
        return (3 * a) + b;
    }



    public class Machine
    {
        public (long, long) A { get; set; }
        public (long, long) B { get; set; }
        public (decimal, decimal) Prize { get; set; }
        public override string ToString()
        {
            return $"A: {A} B: {B} Prize: {Prize}";
        }
    }
    public List<Machine> ParseRows(string input)
    {
        var machines = new List<Machine>();
        using (StringReader sr = new StringReader(input))
        {
            string? line;
            while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
            {
                var m = new Machine();
                var btnARegex = new Regex(@"Button A\: X\+(\d+)\, Y\+(\d+)");
                var btnBRegex = new Regex(@"Button B\: X\+(\d+)\, Y\+(\d+)");
                var prizeRegex = new Regex(@"Prize\: X\=(\d+)\, Y\=(\d+)");
                var aMatch = btnARegex.Match(line);
                long ax = long.Parse(aMatch.Groups[1].Value);
                long ay = long.Parse(aMatch.Groups[2].Value);
                m.A = (ax, ay);
                line = sr.ReadLine();
                if (line == null)
                    throw new Exception("bad input");
                var bMatch = btnBRegex.Match(line);
                long bx = long.Parse(bMatch.Groups[1].Value);
                long by = long.Parse(bMatch.Groups[2].Value);
                m.B = (bx, by);
                line = sr.ReadLine();
                if (line == null)
                    throw new Exception("bad input");
                var prizeMatch = prizeRegex.Match(line);
                decimal offset = 10000000000000;
                decimal px = decimal.Parse(prizeMatch.Groups[1].Value);
                px += offset;
                decimal py = decimal.Parse(prizeMatch.Groups[2].Value);
                py += offset;
                m.Prize = (px, py);
                machines.Add(m);
                line = sr.ReadLine();
            }

        }
        return machines;
    }
}
