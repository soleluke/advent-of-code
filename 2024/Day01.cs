using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Collections;
using System.Drawing;

public class Day01 : IDay
{



    public void Run(string input)
    {
        var rows = ParseRows(input);
        List<int> left = new List<int>();
        List<int> right = new List<int>();
        foreach (var row in rows)
        {
            left.Add(row.Item1);
            right.Add(row.Item2);
        }
        List<int> diffs = new List<int>();


        var calcSims = left.Select(l =>
        {
            return (long)l * (long)right.Count(r => r == l);
        });

        Console.WriteLine($"Similarities: {calcSims.Sum()}");

        while (left.Count > 0)
        {
            int leftMin = left.Min();
            int rightMin = right.Min();
            diffs.Add(Math.Abs(leftMin - rightMin));
            left.Remove(leftMin);
            right.Remove(rightMin);
        }
        Console.WriteLine(diffs.Sum());
    }

    public List<Tuple<int, int>> ParseRows(string input)
    {
        List<Tuple<int, int>> rows = new();
        using (StringReader sr = new StringReader(input))
        {
            string? line;
            while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
            {
                if (String.IsNullOrWhiteSpace(line.Trim()))
                {
                    continue;
                }
                Regex re = new Regex(@"(\d+)\s+(\d+)");
                var match = re.Match(line);
                int first = int.Parse(match.Groups[1].Value);
                int second = int.Parse(match.Groups[2].Value);
                rows.Add(new Tuple<int, int>(first, second));
            }

        }
        return rows;
    }
}
