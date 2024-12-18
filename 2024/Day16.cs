using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day16 : IDay
{

    public void Run(string input)
    {
        var map = ParseRows(input);
        (int, int) start = (-1, -1);
        (int, int) end = (-1, -1);
        for (int r = 0; r < map.GetLength(0); r++)
        {
            for (int c = 0; c < map.GetLength(1); c++)
            {
                if (map[r, c] == 'S')
                    start = (r, c);
                if (map[r, c] == 'E')
                    end = (r, c);
            }
        }
        if (start == (-1, -1) || end == (-1, -1))
            throw new NotSupportedException();

        var (score, paths) = Run(start, end, map);

        Console.WriteLine(score);
        Console.WriteLine(paths.SelectMany(p => p.Select(pn => pn.Item1)).Distinct().Count());

    }
    public class Node
    {
        public (int, int) P { get; set; }
        public (int, int) D { get; set; }
        public HashSet<((int, int), (int, int))> Path { get; set; }
        public long S { get; set; }
        public Node()
        {
            Path = new HashSet<((int, int), (int, int))>();
        }
        public Node F()
        {
            var n = new Node()
            {
                P = GoDir(P, D),
                D = D,
                S = S + 1,
                Path = Path.ToHashSet()
            };
            n.Path.Add((n.P, n.D));
            return n;
        }
        public Node RCW()
        {
            var n = new Node()
            {
                P = P,
                D = CW(D),
                S = S + 1000,
                Path = Path.ToHashSet()
            };
            n.Path.Add((n.P, n.D));
            return n;
        }
        public Node RCCW()
        {
            var n = new Node()
            {
                P = P,
                D = CCW(D),
                S = S + 1000,
                Path = Path.ToHashSet()
            };
            n.Path.Add((n.P, n.D));
            return n;
        }
    }
    public (long, List<HashSet<((int, int), (int, int))>>) Run((int, int) start, (int, int) e, char[,] m)
    {
        var s = new Node() { P = start, D = RIGHT };
        s.Path.Add((start, RIGHT));
        var q = new Queue<Node>([s]);
        var scores = new Dictionary<((int, int), (int, int)), long>();

        var bestScore = long.MaxValue;
        var paths = new List<HashSet<((int, int), (int, int))>>();
        while (q.Count != 0)
        {
            var n = q.Dequeue();
            if (n.S > bestScore)
                continue;
            var (r, c) = n.P;
            if (m[r, c] == 'E')
            {
                if (n.S == bestScore)
                {
                    paths.Add(n.Path);
                }
                else if (n.S < bestScore)
                {
                    bestScore = n.S;
                    paths = [n.Path];
                }
                continue;
            }
            if (CheckDir(n.P, n.D, m))
                QIfBetter(n.F());
            QIfBetter(n.RCCW());
            QIfBetter(n.RCW());

        }
        return (bestScore, paths);
        void QIfBetter(Node test)
        {
            if (!scores.ContainsKey((test.P, test.D)) || scores[(test.P, test.D)] >= test.S)
            {
                scores[(test.P, test.D)] = test.S;
                q.Enqueue(test);
            }
        }
    }
    public static Dictionary<((int, int), (int, int)), long> Scores = new Dictionary<((int, int), (int, int)), long>();
    public long Go((int, int) cur, (int, int) dir, (int, int) e, char[,] m, HashSet<((int, int), (int, int))> visited)
    {
        if (Scores.ContainsKey((cur, dir)))
            return Scores[(cur, dir)];
        if (visited.Contains((cur, dir)))
        {
            Scores[(cur, dir)] = long.MaxValue;
            return long.MaxValue;
        }
        else
            visited.Add((cur, dir));

        if (cur == e)
            return 0;
        long f = long.MaxValue;
        if (CheckDir(cur, dir, m))
        {
            f = Go(GoDir(cur, dir), dir, e, m, visited.ToHashSet());
        }
        long cw = Go(cur, CW(dir), e, m, visited.ToHashSet());
        long ccw = Go(cur, CCW(dir), e, m, visited.ToHashSet());
        if (cw != long.MaxValue)
            cw += 1000;
        if (ccw != long.MaxValue)
            ccw += 1000;
        if (f != long.MaxValue)
            f += 1;

        Console.WriteLine($"cur: {cur} dir: {dir} v: {visited.Count} f:{f} cw:{cw} ccw:{ccw}");
        if (f == cw && cw == ccw)
        {
            Scores[(cur, dir)] = f;
            return f;
        }
        if (f < cw && f < ccw)
        {
            Scores[(cur, dir)] = f;
            return f;
        }
        if (cw < ccw)
        {
            Scores[(cur, dir)] = cw;
            return cw;
        }
        Scores[(cur, dir)] = ccw;
        return ccw;

    }
    public static (int, int) CW((int, int) dir)
    {
        if (dir == RIGHT)
            return DOWN;
        if (dir == DOWN)
            return LEFT;
        if (dir == LEFT)
            return UP;
        if (dir == UP)
            return RIGHT;
        throw new NotSupportedException();
    }
    public static (int, int) CCW((int, int) dir)
    {
        if (dir == RIGHT)
            return UP;
        if (dir == DOWN)
            return RIGHT;
        if (dir == LEFT)
            return DOWN;
        if (dir == UP)
            return LEFT;
        throw new NotSupportedException();
    }
    static (int, int) UP = (-1, 0);
    static (int, int) DOWN = (1, 0);
    static (int, int) LEFT = (0, -1);
    static (int, int) RIGHT = (0, 1);
    public static (int, int) GoDir((int, int) cur, (int, int) dir)
    {

        return (cur.Item1 + dir.Item1, cur.Item2 + dir.Item2);
    }
    public bool CheckDir((int, int) cur, (int, int) dir, char[,] map)
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
        return map[r, c] != '#';
    }
    public void PrintMap(char[,] m)
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
    public char[,] ParseRows(string input)
    {
        List<char[]> map = new();

        using (StringReader sr = new StringReader(input))
        {
            string? line;
            while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
            {
                map.Add(line.ToCharArray());
            }
        }
        char[,] m = new char[map.Count, map[0].Length];
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
