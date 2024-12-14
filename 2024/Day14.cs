using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day14 : IDay
{
    public void PrintMap(int[,] m)
    {
        for (int r = 0; r < m.GetLength(0); r++)
        {
            for (int c = 0; c < m.GetLength(1); c++)
            {
                Console.Write(m[r, c] > 0 ? 'X' : ' ');
            }
            Console.WriteLine();
        }
    }
    public void Run(string input)
    {
        var room = new int[101, 103];

        var robots = ParseRows(input);
        for (int i = 0; i < 1000000; i++)
        {
            foreach (var r in robots)
            {
                r.Move(room);
            }
            foreach (var r in robots)
            {
                var (x, y) = r.Pos;
                room[x, y]++;
            }
            if (CheckRoom(room))
            {
                PrintMap(room);
                Console.WriteLine(i);
                Console.ReadLine();
            }
            room = new int[101, 103];
        }


    }
    public bool CheckRoom(int[,] room)
    {
        foreach (var r in room)
        {
            if (r > 1)
                return false;
        }
        return true;
    }
    public class Robot
    {
        public (int, int) Pos { get; set; }
        public (int, int) Vel { get; set; }
        public override string ToString()
        {
            return $"P:{Pos} V:{Vel}";
        }
        public void Move(int[,] map)
        {
            var (r, c) = GoDir(Pos, Vel);
            if (r < 0)
                r += map.GetLength(0);
            if (c < 0)
                c += map.GetLength(1);
            if (r >= map.GetLength(0))
                r -= map.GetLength(0);
            if (c >= map.GetLength(1))
                c -= map.GetLength(1);
            Pos = (r, c);

        }
    }
    public static (int, int) GoDir((int, int) cur, (int, int) dir)
    {

        return (cur.Item1 + dir.Item1, cur.Item2 + dir.Item2);
    }
    public List<Robot> ParseRows(string input)
    {
        var robots = new List<Robot>();

        using (StringReader sr = new StringReader(input))
        {
            string? line;
            while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
            {
                Regex reg = new Regex(@"p\=(\-{0,1}\d+)\,(\-{0,1}\d+)\s+v\=(\-{0,1}\d+)\,(\-{0,1}\d+)");
                var res = reg.Match(line);
                var x = int.Parse(res.Groups[1].Value);
                var y = int.Parse(res.Groups[2].Value);
                var vx = int.Parse(res.Groups[3].Value);
                var vy = int.Parse(res.Groups[4].Value);
                var rob = new Robot()
                {
                    Pos = (x, y),
                    Vel = (vx, vy)
                };
                robots.Add(rob);
            }

        }
        return robots;
    }
}

