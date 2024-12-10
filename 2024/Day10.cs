using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day10 : IDay
{

    public void Run(string input)
    {
        var map = ParseRows(input);

        var th = FindTrailHeads(map).Distinct();
        var peaks = th.Select(t => CheckTrail(t, map));
        var peakCount = peaks.Select(p => p.Count);
        Console.WriteLine(peakCount.Sum());
        var ratings = th.Select(t => RateTrail(t, map));
        Console.WriteLine(ratings.Sum());
    }
    static (int, int) UP = (-1, 0);
    static (int, int) DOWN = (1, 0);
    static (int, int) LEFT = (0, -1);
    static (int, int) RIGHT = (0, 1);
    public int RateTrail((int, int) th, int[,] map)
    {
        var (r, c) = th;
        if (map[r, c] == 9)
            return 1;
        int score = 0;
        if (CheckDir(th, UP, map))
            score += RateTrail(GoDir(th, UP), map);
        if (CheckDir(th, DOWN, map))
            score += RateTrail(GoDir(th, DOWN), map);
        if (CheckDir(th, LEFT, map))
            score += RateTrail(GoDir(th, LEFT), map);
        if (CheckDir(th, RIGHT, map))
            score += RateTrail(GoDir(th, RIGHT), map);
        return score;

    }
    public List<(int, int)> CheckTrail((int, int) th, int[,] map)
    {
        var (r, c) = th;
        if (map[r, c] == 9)
            return new List<(int, int)>() { (r, c) };
        List<(int, int)> peaks = new();
        if (CheckDir(th, UP, map))
            peaks.AddRange(CheckTrail(GoDir(th, UP), map));
        if (CheckDir(th, DOWN, map))
            peaks.AddRange(CheckTrail(GoDir(th, DOWN), map));
        if (CheckDir(th, LEFT, map))
            peaks.AddRange(CheckTrail(GoDir(th, LEFT), map));
        if (CheckDir(th, RIGHT, map))
            peaks.AddRange(CheckTrail(GoDir(th, RIGHT), map));
        return peaks.Distinct().ToList();
    }
    public (int, int) GoDir((int, int) cur, (int, int) dir)
    {

        return (cur.Item1 + dir.Item1, cur.Item2 + dir.Item2);
    }
    public bool CheckDir((int, int) cur, (int, int) dir, int[,] map)
    {
        var (r, c) = GoDir(cur, dir);
        if (r < 0)
            return false;
        if (c < 0)
            return false;
        if (r >= map.GetLength(0))
            return false;
        if (c >= map.GetLength(1))
            return false;
        var (cr, cc) = cur;
        var curH = map[cr, cc];
        var test = map[r, c];
        return test == curH + 1;
    }
    public List<(int, int)> FindTrailHeads(int[,] map)
    {
        List<(int, int)> th = new();
        for (int r = 0; r < map.GetLength(0); r++)
        {
            for (int c = 0; c < map.GetLength(1); c++)
            {
                if (map[r, c] == 0)
                    th.Add((r, c));
            }
        }
        return th;
    }
    public void PrintMap(int[,] m)
    {
        for (int r = 0; r < m.GetLength(0); r++)
        {
            for (int c = 0; c < m.GetLength(1); c++)
            {
                Console.Write(m[r, c]);
            }
            Console.WriteLine();
        }
    }

    public int[,] ParseRows(string input)
    {
        List<int[]> map = new();

        using (StringReader sr = new StringReader(input))
        {
            string? line;
            while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
            {
                map.Add(line.ToCharArray().Select(c => c - '0').ToArray());
            }
        }
        int[,] m = new int[map.Count, map[0].Length];
        for (int r = 0; r < m.GetLength(0); r++)
        {
            for (int c = 0; c < m.GetLength(1); c++)
            {
                m[r, c] = map[r][c];
            }
        }
        return m;
    }
}
