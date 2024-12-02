using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day02 : IDay
{

    public void PartOne(List<List<int>> reports)
    {
        var safeReports = new List<List<int>>();
        var unsafeReports = new List<List<int>>();
        foreach (var report in reports)
        {
            int direction = 0;
            bool safe = true;
            for (int i = 0; i < report.Count - 1; i++)
            {
                var cur = report[i];
                var next = report[i + 1];
                var diff = cur - next;
                if (Math.Abs(diff) < 1)
                {
                    unsafeReports.Add(report);
                    safe = false;
                    break;
                }
                var newDir = diff / Math.Abs(diff);
                if (direction != 0 && newDir != direction)
                {
                    unsafeReports.Add(report);
                    safe = false;
                    break;
                }
                direction = newDir;
                if (Math.Abs(diff) > 3)
                {
                    unsafeReports.Add(report);
                    safe = false;
                    break;
                }
            }
            if (safe)
                safeReports.Add(report);

        }
        Console.WriteLine(safeReports.Count);
        //PrintReports(safeReports);

    }
    public (bool, int) CheckReport(List<int> report)
    {
        int direction = 0;
        for (int i = 0; i < report.Count - 1; i++)
        {
            var cur = report[i];
            var next = report[i + 1];
            var diff = cur - next;
            if (diff == 0)
            {
                return (false, i);
            }
            var newDir = diff / Math.Abs(diff);
            if (direction != 0 && newDir != direction)
            {
                return (false, i);
            }
            direction = newDir;
            if (Math.Abs(diff) > 3)
            {
                return (false, i);
            }
        }
        return (true, -1);
    }

    public void Part2(List<List<int>> reports)
    {
        var safeReports = new List<List<int>>();
        var unsafeReports = new List<List<int>>();
        foreach (var report in reports)
        {
            var (safe, badIndex) = CheckReport(report);
            if (safe)
            {
                safeReports.Add(report);
            }
            else
            {
                if (badIndex > 0)
                {
                    var prev = new List<int>(report);
                    prev.RemoveAt(badIndex - 1);
                    var (s, bi) = CheckReport(prev);
                    if (s)
                    {
                        safeReports.Add(report);
                        continue;
                    }
                }
                var cur = new List<int>(report);
                cur.RemoveAt(badIndex);
                var (s3, bi3) = CheckReport(cur);
                if (s3)
                {
                    safeReports.Add(report);
                    continue;
                }
                if (badIndex < report.Count - 1)
                {
                    var next = new List<int>(report);
                    next.RemoveAt(badIndex + 1);
                    var (s2, bi2) = CheckReport(next);
                    if (s2)
                    {
                        safeReports.Add(report);
                        continue;
                    }
                    unsafeReports.Add(report);
                }
            }
        }
        Console.WriteLine(safeReports.Count);
        //PrintReports(safeReports);

    }

    public void Run(string input)
    {
        var reports = ParseRows(input);
        Part2(reports);
    }
    public void PrintReports(List<List<int>> reports)
    {
        foreach (var report in reports)
        {
            Console.WriteLine(String.Join(' ', report));
        }
    }

    public List<List<int>> ParseRows(string input)
    {
        var reports = new List<List<int>>();

        using (StringReader sr = new StringReader(input))
        {
            string? line;
            while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
            {
                reports.Add(line.Split(' ').Select(l => int.Parse(l)).ToList());
            }

        }
        return reports;
    }
}
