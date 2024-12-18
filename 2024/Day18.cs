using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day18 : IDay
{

    public void Run(string input)
    {
        var gridSize = 71;
        var grid = new bool[gridSize, gridSize];
        var bytes = ParseRows(input);
        var numBytes = 1024;
        if (numBytes > bytes.Count)
            throw new NotSupportedException();
        for (int i = 0; i < numBytes; i++)
        {
            var (y, x) = bytes[i];
            grid[x, y] = true;
        }
        var (score, paths) = Run((0, 0), (grid.GetLength(0) - 1, grid.GetLength(1) - 1), grid);

        Console.WriteLine(score);
        var b = numBytes;
        while (paths.Any())
        {
            var (y, x) = bytes[b];
            grid[x, y] = true;
            (score, paths) = Run((0, 0), (gridSize - 1, gridSize - 1), grid);
            if (paths.Count() == 0)
                break;
            b++;
        }
        Console.WriteLine(bytes[b]);
    }
    static (int, int) UP = (-1, 0);
    static (int, int) DOWN = (1, 0);
    static (int, int) LEFT = (0, -1);
    static (int, int) RIGHT = (0, 1);
    public class Node
    {
        public (int, int) P { get; set; }
        public long S { get; set; }
        public HashSet<(int, int)> Path { get; set; }
        public Node()
        {
            Path = new HashSet<(int, int)>();
        }
        public Node Clone((int, int) dir)
        {
            var n = new Node()
            {
                P = GoDir(P, dir),
                S = S + 1,
                Path = Path.ToHashSet()
            };
            Path.Add(n.P);
            return n;
        }
        public Node Up()
        {
            return Clone(UP);
        }
        public Node Down()
        {
            return Clone(DOWN);
        }
        public Node Left()
        {
            return Clone(LEFT);
        }
        public Node Right() { return Clone(RIGHT); }
    }
    public (long, List<HashSet<(int, int)>>) Run((int, int) start, (int, int) e, bool[,] m)
    {
        var s = new Node() { P = start };
        s.Path.Add(start);
        var q = new Queue<Node>([s]);
        var scores = new Dictionary<(int, int), long>();

        var bestScore = long.MaxValue;
        var paths = new List<HashSet<(int, int)>>();
        while (q.Count != 0)
        {
            var n = q.Dequeue();
            if (n.S > bestScore)
                continue;
            var (r, c) = n.P;

            if (n.P == e)
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
            if (CheckDir(n.P, UP, m))
                QIfBetter(n.Up());
            if (CheckDir(n.P, DOWN, m))
                QIfBetter(n.Down());
            if (CheckDir(n.P, LEFT, m))
                QIfBetter(n.Left());
            if (CheckDir(n.P, RIGHT, m))
                QIfBetter(n.Right());

        }
        return (bestScore, paths);
        void QIfBetter(Node test)
        {
            if (!scores.ContainsKey(test.P))
            {
                scores[test.P] = test.S;
                q.Enqueue(test);
            }
        }
    }
    public static (int, int) GoDir((int, int) cur, (int, int) dir)
    {

        return (cur.Item1 + dir.Item1, cur.Item2 + dir.Item2);
    }
    public bool CheckDir((int, int) cur, (int, int) dir, bool[,] map)
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
        return !map[r, c];
    }
    public void PrintMap(bool[,] m)
    {
        for (int r = 0; r < m.GetLength(0); r++)
        {
            for (int c = 0; c < m.GetLength(1); c++)
            {
                Console.Write(m[r, c] ? '#' : '.');
            }
            Console.WriteLine();
        }
    }

    public List<(int, int)> ParseRows(string input)
    {
        var bytes = new List<(int, int)>();
        using (StringReader sr = new StringReader(input))
        {
            string? line;
            while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
            {
                var b = line.Split(',').Select(s => int.Parse(s)).ToArray();
                bytes.Add((b[0], b[1]));
            }

        }
        return bytes;
    }
}
