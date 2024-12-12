using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day12 : IDay
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
    public void Run(string input)
    {
        var map = ParseRows(input);
        var regions = new List<Region>();
        for (int r = 0; r < map.GetLength(0); r++)
        {
            for (int c = 0; c < map.GetLength(1); c++)
            {
                if (!regions.Any(re => re.Contains((r, c))))
                {
                    regions.Add(FindRegion((r, c), map));
                }
            }
        }
        long price = 0;
        foreach (var r in regions)
        {
            //Console.WriteLine(r.ToString());
            price += r.Price();
        }
        Console.WriteLine(price);

    }
    static (int, int) UP = (-1, 0);
    static (int, int) DOWN = (1, 0);
    static (int, int) LEFT = (0, -1);
    static (int, int) RIGHT = (0, 1);

    public static (int, int) GoDir((int, int) p, (int, int) d)
    {

        var (r, c) = (p.Item1 + d.Item1, p.Item2 + d.Item2);
        return (r, c);
    }
    public (char, (int, int))? Try((int, int) p, (int, int) d, char[,] map)
    {
        var (r, c) = GoDir(p, d);
        if (r < 0)
            return null;
        if (c < 0)
            return null;
        if (r >= map.GetLength(0))
            return null;
        if (c >= map.GetLength(1))
            return null;
        return (map[r, c], (r, c));
    }
    public List<(int, int)> FindPoints(char p, (int, int) start, char[,] map, bool[,] visited)
    {
        var (r, c) = start;
        if (visited[r, c])
            return [];
        visited[r, c] = true;

        var path = new List<(int, int)>() { start };
        var u = Try(start, UP, map);
        if (u.HasValue)
        {
            var (nr, nc) = u.Value.Item2;
            if (u.Value.Item1 == p && !visited[nr, nc])
            {
                path.AddRange(FindPoints(p, (nr, nc), map, visited));
            }
        }
        u = Try(start, DOWN, map);
        if (u.HasValue)
        {
            var (nr, nc) = u.Value.Item2;
            if (u.Value.Item1 == p && !visited[nr, nc])
            {
                path.AddRange(FindPoints(p, (nr, nc), map, visited));
            }
        }
        u = Try(start, LEFT, map);
        if (u.HasValue)
        {
            var (nr, nc) = u.Value.Item2;
            if (u.Value.Item1 == p && !visited[nr, nc])
            {
                path.AddRange(FindPoints(p, (nr, nc), map, visited));
            }
        }
        u = Try(start, RIGHT, map);
        if (u.HasValue)
        {
            var (nr, nc) = u.Value.Item2;
            if (u.Value.Item1 == p && !visited[nr, nc])
            {
                path.AddRange(FindPoints(p, (nr, nc), map, visited));
            }
        }
        return path;
    }
    public Region FindRegion((int, int) start, char[,] map)
    {
        var path = new List<(int, int)>();
        var visited = new bool[map.GetLength(0), map.GetLength(1)];
        var (r, c) = start;
        char p = map[r, c];
        return new Region()
        {
            P = p,
            Points = FindPoints(p, start, map, visited)
        };

    }
    public class Region
    {
        public char P { get; set; }
        public List<(int, int)> Points { get; set; }
        public Region()
        {
            Points = new List<(int, int)>();
            perimeter = 0;
            sides = 0;
        }
        public bool Contains((int, int) p)
        {
            return Points.Contains(p);
        }
        public override string ToString()
        {
            return $"{P} ({Perimeter()},{A},{NumSides()}): {string.Join(',', Points)}";
        }
        public int A => Points.Count;
        private long perimeter;
        public long Perimeter()
        {
            if (perimeter > 0)
                return perimeter;
            perimeter = 0;
            foreach (var p in Points)
            {
                int peri = 4;
                if (Points.Contains(GoDir(p, UP)))
                    peri--;
                if (Points.Contains(GoDir(p, DOWN)))
                    peri--;
                if (Points.Contains(GoDir(p, LEFT)))
                    peri--;
                if (Points.Contains(GoDir(p, RIGHT)))
                    peri--;
                perimeter += peri;
            }
            return perimeter;
        }
        public long Price()
        {
            return A * NumSides();
        }
        private long sides;
        public long NumSides()
        {
            if (sides > 0)
                return sides;
            sides = 0;
            foreach (var p in Points)
            {
                var u = Points.Contains(GoDir(p, UP));
                var d = Points.Contains(GoDir(p, DOWN));
                var r = Points.Contains(GoDir(p, RIGHT));
                var l = Points.Contains(GoDir(p, LEFT));

                var ul = Points.Contains(GoDir(GoDir(p, UP), LEFT));
                var ur = Points.Contains(GoDir(GoDir(p, UP), RIGHT));
                var dl = Points.Contains(GoDir(GoDir(p, DOWN), LEFT));
                var dr = Points.Contains(GoDir(GoDir(p, DOWN), RIGHT));

                if (u && l && !ul) // inside upleft corner
                    sides++;
                if (!u && !l) // outside upleft corner
                    sides++;
                if (u && r && !ur) // inside upright corner
                    sides++;
                if (!u && !r) // outside upright corner
                    sides++;
                if (d && l && !dl) //inside downleft corner
                    sides++;
                if (!d && !l) //outside downleft corner
                    sides++;
                if (d && r && !dr) //inside downright corner
                    sides++;
                if (!d && !r) //outside downright corner
                    sides++;


            }
            return sides;
        }
    }
    public static int Angle((int, int) dir)
    {
        if (dir == UP)
            return 90;
        if (dir == LEFT)
            return 180;
        if (dir == DOWN)
            return 270;
        if (dir == RIGHT)
            return 360;
        throw new NotSupportedException();
    }
    public static (int, int) Dir((int, int) a, (int, int) b)
    {
        return (b.Item1 - a.Item1, b.Item2 - a.Item2);
    }
    public static int Dist((int, int) a, (int, int) b)
    {
        return Math.Abs(a.Item1 - b.Item1) + Math.Abs(a.Item2 - b.Item2);
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
