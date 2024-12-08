using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day08 : IDay
{
    public class Frequency
    {
        public char I { get; set; }
        public List<(int, int)> A { get; set; }
        public Frequency()
        {
            A = new();
        }
        public Frequency(char I)
        {
            A = new();
            this.I = I;
        }
        public override string ToString()
        {
            return $"{I}: {string.Join(',', A)}";
        }
    }
    public void Run(string input)
    {
        var map = ParseRows(input);
        //PrintMap(map);
        var freqs = new Dictionary<char, Frequency>();
        var rows = map.GetLength(0);
        var cols = map.GetLength(1);
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var cur = map[r, c];
                if (cur != '.')
                {
                    if (freqs.ContainsKey(cur))
                        freqs[cur].A.Add((r, c));
                    else
                        freqs[cur] = new Frequency(cur)
                        {
                            A = new() { (r, c) }
                        };
                }
            }
        }
        var an = freqs.Values.SelectMany(f => FindAntiNodes(f, (rows, cols)));
        Console.WriteLine(an.Distinct().Count());
    }
    public List<(int, int)> FindAntiNodes(Frequency f, (int, int) bounds)
    {
        var (rows, cols) = bounds;
        var an = new List<(int, int)>();
        var combos = GetCombinations(f.A);
        foreach (var c in combos)
        {
            an.AddRange(FindAntiNodes(c, bounds));

        }
        //Console.WriteLine(string.Join(',', an));
        return an;
    }
    public List<(int, int)> FindAntiNodes(((int, int), (int, int)) c, (int, int) bounds)
    {
        var (a, b) = c;
        var an = new List<(int, int)>();
        var (ar, ac) = a;
        var (br, bc) = b;
        var rdiff = br - ar;
        var cdiff = bc - ac;
        bool inBounds = true;
        (int, int) test = a;
        while (inBounds)
        {
            var (tr, tc) = test;
            test = (tr + rdiff, tc + cdiff);
            if (InBounds(bounds, test))
            {
                an.Add(test);
            }
            else
            {
                inBounds = false;
            }
        }
        inBounds = true;
        test = b;
        while (inBounds)
        {
            var (tr, tc) = test;
            test = (tr - rdiff, tc - cdiff);
            if (InBounds(bounds, test))
            {
                an.Add(test);
            }
            else
            {
                inBounds = false;
            }
        }
        //Console.WriteLine($"a: {a} b: {b} diff: ({rdiff},{cdiff}) first:{first} {fib} second:{second} {sib}");
        //Console.WriteLine(string.Join(',', an));
        return an;
    }
    public bool InBounds((int, int) bounds, (int, int) p)
    {
        var (r, c) = p;
        var (rows, cols) = bounds;
        if (r >= rows)
            return false;
        if (c >= cols)
            return false;
        if (r < 0)
            return false;
        if (c < 0)
            return false;
        return true;
    }
    public List<((int, int), (int, int))> GetCombinations(List<(int, int)> l)
    {
        var c = new List<((int, int), (int, int))>();
        for (int i = 0; i < l.Count - 1; i++)
        {
            for (int j = i + 1; j < l.Count; j++)
            {
                c.Add((l[i], l[j]));
            }
        }
        return c;

    }
    public void PrintMap(char[,] m)
    {
        var rows = m.GetLength(0);
        var cols = m.GetLength(1);
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Console.Write(m[r, c]);
            }
            Console.WriteLine();
        }
    }


    public char[,] ParseRows(string input)
    {
        List<char[]> map = new List<char[]>();
        using (StringReader sr = new StringReader(input))
        {
            string? line;
            while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
            {
                map.Add(line.ToCharArray());
            }
        }
        var rows = map.Count;
        var cols = map[0].Length;
        var m = new char[rows, cols];
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                m[r, c] = map[r][c];
            }
        }
        return m;
    }
}
