using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day06 : IDay
{
    public void PrintMap(char[,] map)
    {
        for (int r = 0; r < map.GetLength(0); r++)
        {
            for (int c = 0; c < map.GetLength(1); c++)
            {
                Console.Write(map[r, c]);
            }
            Console.WriteLine();
        }
    }
    public (int, int) GetStart(char[,] map)
    {
        for (int r = 0; r < map.GetLength(0); r++)
        {
            for (int c = 0; c < map.GetLength(1); c++)
            {
                if (map[r, c] == '^')
                    return (r, c);
            }
        }
        throw new NotImplementedException("no start found");
    }
    public (int, int) Turn((int, int) dir)
    {
        if (dir == (0, 1))
            return (1, 0);
        if (dir == (1, 0))
            return (0, -1);
        if (dir == (0, -1))
            return (-1, 0);
        if (dir == (-1, 0))
            return (0, 1);
        throw new NotImplementedException("bad direction");
    }
    public void Run(string input)
    {
        var map = ParseRows(input);
        //PrintMap(map);
        var start = GetStart(map);
        var direction = (-1, 0);
        var visits = Steps(start, direction, map);
        Console.WriteLine(visits);
        var loops = FindLoops(start, direction, new Dictionary<((int, int), (int, int)), bool>(), new List<(int, int)>(), null, map);
        Console.WriteLine(loops.Distinct().Count());

    }
    public List<(int, int)> FindLoops((int, int) start, (int, int) direction, Dictionary<((int, int), (int, int)), bool> looped, List<(int, int)> visited, (int, int)? added, char[,] map)
    {
        var (r, c) = start;
        if (!visited.Contains(start))
        {
            visited.Add(start);
        }
        var (dr, dc) = direction;
        var next = CheckNext(start, direction, map);
        //Console.WriteLine($"({r},{c}) dir ({dr},{dc}) next {next}");
        List<(int, int)> adds = new();
        switch (next)
        {
            case Status.Off:
                break;
            case Status.Empty:
                if (!added.HasValue)
                {
                    var newDir = Turn(direction);
                    var newMap = (char[,])map.Clone();
                    var tryAdd = (r + dr, c + dc);
                    if (!visited.Contains(tryAdd))
                    {
                        newMap[r + dr, c + dc] = '#';
                        var newLooped = new Dictionary<((int, int), (int, int)), bool>(looped);
                        newLooped.Add((start, direction), false);
                        //Console.WriteLine($"adding {tryAdd}");
                        var newVisited = new List<(int, int)>(visited);
                        var newAdds = FindLoops(start, newDir, newLooped, newVisited, tryAdd, newMap);
                        adds.AddRange(newAdds);
                    }
                }
                var oldAdds = FindLoops((r + dr, c + dc), direction, looped, visited, added, map);
                adds.AddRange(oldAdds);
                break;
            case Status.Blocked:
                if (looped.ContainsKey((start, direction)))
                {
                    //Console.WriteLine($"looped has ({start}) ({direction})");
                    if (!looped[(start, direction)])
                    {
                        looped[(start, direction)] = true;
                    }
                    else
                    {
                        if (added.HasValue)
                        {
                            adds.Add(added.Value);
                        }

                        return adds;
                    }
                }
                else
                {
                    looped[(start, direction)] = false;
                }
                var newd = Turn(direction);
                var adds2 = FindLoops(start, newd, looped, visited, added, map);
                adds.AddRange(adds2);
                break;
        }
        return adds;
    }
    public enum Status
    {
        Blocked,
        Empty,
        Off
    }
    public Status CheckNext((int, int) cur, (int, int) dir, char[,] map)
    {
        var (cr, cc) = cur;
        var (cdr, cdc) = dir;
        var (r, c) = (cr + cdr, cc + cdc);
        if (r >= map.GetLength(0) || c >= map.GetLength(1))
            return Status.Off;
        if (r < 0 || c < 0)
            return Status.Off;
        switch (map[r, c])
        {
            case '#':
                return Status.Blocked;
            default:
                return Status.Empty;
        }
    }
    public int Steps((int, int) start, (int, int) dir, char[,] map)
    {
        var visited = new bool[map.GetLength(0), map.GetLength(1)];
        bool free = false;
        var (r, c) = start;
        var (dr, dc) = dir;
        while (!free)
        {
            var s = CheckNext((r, c), (dr, dc), map);

            visited[r, c] = true;
            switch (s)
            {
                case Status.Empty:
                    //continue straight
                    (r, c) = (r + dr, c + dc);
                    break;
                case Status.Blocked:
                    //turn
                    (dr, dc) = Turn((dr, dc));
                    break;
                case Status.Off:
                    free = true;
                    break;

            }
        }
        int visits = 0;
        foreach (var v in visited)
        {
            if (v)
                visits++;
        }
        return visits;
    }

    public char[,] ParseRows(string input)
    {
        List<char[]> rows = new();
        using (StringReader sr = new StringReader(input))
        {
            string? line;
            while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
            {
                rows.Add(line.Trim().ToCharArray());
            }

        }
        int width = rows[0].Count();
        int height = rows.Count;
        char[,] map = new char[height, width];
        for (int r = 0; r < rows.Count; r++)
        {
            for (int c = 0; c < rows[0].Count(); c++)
            {
                map[r, c] = rows[r][c];
            }
        }
        return map;

    }
}
