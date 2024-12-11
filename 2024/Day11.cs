using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;
using System.Threading;

public class Day11 : IDay
{
    public void Run(string input)
    {
        var arr = ParseRows(input);
        //        Console.WriteLine(string.Join(' ', arr));
        var times = 75;
        long sum = 0;
        var cache = new Dictionary<(long, int), long>();
        foreach (var l in arr)
        {
            sum += Blink(l, times, cache);
        }
        //Console.WriteLine(string.Join(' ', arr));
        Console.WriteLine(sum);

    }
    public long Blink(long a, int times, Dictionary<(long, int), long> cache)
    {

        if (cache.ContainsKey((a, times)))
        {
            return cache[(a, times)];
        }
        if (times == 1)
        {
            return Blink(a).Count();
        }
        var b = Blink(a);
        var res = b.Select(l => Blink(l, times - 1, cache));
        var sum = res.Aggregate<long>((r, s) => s + r);
        cache[(a, times)] = sum;
        return sum;
    }
    public long[] Blink(long[] a)
    {
        var r = new List<long>();
        foreach (var l in a)
        {
            var b = Blink(l);
            r.AddRange(b);
        }
        return r.ToArray();
    }
    public long[] Blink(long s)
    {
        if (s == 0)
            return new long[] { 1 };
        var str = s.ToString();
        if (str.Length % 2 == 0)
        {
            var first = str.Substring(0, str.Length / 2);
            var sec = str.Substring(str.Length / 2, str.Length / 2);
            return new long[] { long.Parse(first), long.Parse(sec) };
        }
        return new long[] { s * 2024 };
    }

    public long[] ParseRows(string input)
    {

        using (StringReader sr = new StringReader(input))
        {
            string? line = sr.ReadLine();
            return line.Split(' ').Select(s => long.Parse(s)).ToArray();
        }
    }
}
