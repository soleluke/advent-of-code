using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day04 : IDay
{

    public void Run(string input)
    {
        var search = ParseRows(input);
        var x = Starts('X', search);
        foreach (var xc in x)
        {
            //Console.WriteLine($"{xc.Item1} {xc.Item2}");
        }
        var sums = x.Select(c => (c, Xmas(c, search))).ToList();


        Console.WriteLine($"Part 1: {sums.Select(s => s.Item2).Sum()}");

        var m = Starts('A', search);


        Console.WriteLine(m.Count);
        var mas = m.Select(ma => Mas(ma, search));
        Console.WriteLine($"Part 2: {mas.Count(ma => ma)}");

    }
    public bool Mas((int, int) x, char[,] grid)
    {
        var DLS = CheckChar('S', x, (1, -1), grid); //down left
        var DLM = CheckChar('M', x, (1, -1), grid); //down left
        var URS = CheckChar('S', x, (-1, 1), grid); // up right
        var URM = CheckChar('M', x, (-1, 1), grid); // up right
        var DRS = CheckChar('S', x, (1, 1), grid); //down right
        var DRM = CheckChar('M', x, (1, 1), grid); //down right
        var ULS = CheckChar('S', x, (-1, -1), grid); //UP LEFT
        var ULM = CheckChar('M', x, (-1, -1), grid); //UP LEFT

        if (DLS && URM)
        {
            if (DRS && ULM)
                return true;
            if (DRM && ULS)
                return true;
        }
        if (DLM && URS)
        {
            if (DRS && ULM)
                return true;
            if (DRM && ULS)
                return true;

        }
        return false;
    }

    public bool CheckChar(char check, (int, int) x, (int, int) dirs, char[,] grid)
    {
        var newX = (x.Item1 + dirs.Item1, x.Item2 + dirs.Item2);
        if (newX.Item1 < 0)
            return false;
        if (newX.Item1 >= grid.GetLength(0))
            return false;
        if (newX.Item2 < 0)
            return false;
        if (newX.Item2 >= grid.GetLength(1))
            return false;
        var c = grid[newX.Item1, newX.Item2];
        return c == check;
    }
    public bool CheckChar(int i, (int, int) x, (int, int) dirs, char[,] grid)
    {
        var newX = (x.Item1 + dirs.Item1, x.Item2 + dirs.Item2);
        if (newX.Item1 < 0)
            return false;
        if (newX.Item1 >= grid.GetLength(0))
            return false;
        if (newX.Item2 < 0)
            return false;
        if (newX.Item2 >= grid.GetLength(1))
            return false;
        var c = grid[newX.Item1, newX.Item2];
        if (c == "XMAS"[i])
        {
            if (i == 3)
                return true;
            else
                return CheckChar(i + 1, newX, dirs, grid);
        }
        else
        {
            return false;
        }
    }

    public int Xmas((int, int) x, char[,] grid)
    {
        int sum = 0;
        if (CheckChar(1, x, (1, 0), grid))
            sum++;
        if (CheckChar(1, x, (1, -1), grid))
            sum++;
        if (CheckChar(1, x, (1, 1), grid))
            sum++;
        if (CheckChar(1, x, (-1, -1), grid))
            sum++;
        if (CheckChar(1, x, (-1, 0), grid))
            sum++;
        if (CheckChar(1, x, (-1, 1), grid))
            sum++;
        if (CheckChar(1, x, (0, 1), grid))
            sum++;
        if (CheckChar(1, x, (0, -1), grid))
            sum++;
        return sum;
    }
    public List<(int, int)> Starts(char s, char[,] grid)
    {
        var x = new List<(int, int)>();
        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                if (grid[row, col] == s)
                {
                    x.Add((row, col));
                }
            }
        }
        return x;
    }

    public char[,] ParseRows(string input)
    {
        List<char[]> rows = new();
        using (StringReader sr = new StringReader(input))
        {
            string? line;
            while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
            {
                rows.Add(line.ToCharArray());
            }
        }
        char[,] r = new char[rows[0].Length, rows.Count];
        for (int row = 0; row < rows.Count; row++)
        {
            for (int col = 0; col < rows[0].Length; col++)
            {
                r[row, col] = rows[row][col];
            }
        }
        return r;
    }
}
