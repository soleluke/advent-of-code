using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day07 : IDay
{

    public void Run(string input)
    {
        var eqs = ParseRows(input);
        var solvable = eqs.Where(e => Solvable(e));
        foreach (var s in solvable)
        {
            Console.WriteLine(s.ToString());
        }
        Console.WriteLine(solvable.Select(s => s.Test).Sum());
    }
    public bool Solvable(Equation eq)
    {
        //Console.WriteLine(eq.ToString());
        if (eq.Values.Count == 2)
        {
            var product = eq.Values[0] * eq.Values[1];
            if (product == eq.Test)
                return true;
            var sum = eq.Values[0] + eq.Values[1];
            if (sum == eq.Test)
                return true;
            var c = Concat(eq.Values[0], eq.Values[1]);
            if (c == eq.Test)
                return true;
            return false;
        }
        var first = eq.Values[0];
        var second = eq.Values[1];
        var np = first * second;
        var neq = eq.Clone();
        neq.Values.RemoveAt(0);
        neq.Values.RemoveAt(0);
        neq.Values.Insert(0, np);
        if (Solvable(neq))
            return true;
        var ns = first + second;
        neq.Values[0] = ns;
        if (Solvable(neq))
            return true;
        var nc = Concat(first, second);
        neq.Values[0] = nc;
        if (Solvable(neq))
            return true;
        return false;
    }
    public long Concat(long i, long j)
    {
        string iStr = i.ToString();
        string jStr = j.ToString();
        var c = iStr + jStr;
        return long.Parse(c);
    }
    public class Equation
    {
        public long Test { get; set; }
        public List<long> Values { get; set; }
        public Equation()
        {
            Values = new List<long>();
        }
        public override string ToString()
        {
            return $"{Test}: {string.Join(',', Values)}";
        }
        public Equation Clone()
        {
            var eq = new Equation()
            {
                Test = Test,
                Values = new List<long>(Values)
            };
            return eq;
        }
    }
    public List<Equation> ParseRows(string input)
    {
        var eqs = new List<Equation>();
        using (StringReader sr = new StringReader(input))
        {
            string? line;
            while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
            {
                Equation eq = new Equation();
                Regex re = new Regex(@"(\d+)\:\s+((\d+\s*)+)");
                var m = re.Match(line);
                var testStr = m.Groups[1].Value;
                var valStr = m.Groups[2].Value.Split(" ");
                eq.Test = long.Parse(testStr);
                eq.Values = valStr.Select(v => long.Parse(v)).ToList();
                eqs.Add(eq);
            }
        }
        return eqs;
    }
}
