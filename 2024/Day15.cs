using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day15 : IDay
{

    public void Run(string input)
    {
        var (map, moves) = ParseRows(input);
        var moves2 = moves.ToList();
        var exp = ExpandMap(map);

        (int, int) robotStart = (-1, -1);
        for (int r = 0; r < map.GetLength(0); r++)
        {
            for (int c = 0; c < map.GetLength(1); c++)
            {
                if (map[r, c] == '@')
                    robotStart = (r, c);
            }
        }
        if (robotStart == (-1, -1))
            throw new NotImplementedException();
        Move(robotStart, moves, map);

        long sum = 0;
        for (int r = 0; r < map.GetLength(0); r++)
        {
            for (int c = 0; c < map.GetLength(1); c++)
            {
                if (map[r, c] == 'O')
                {
                    sum += (100 * r) + c;
                }
            }
        }
        Console.WriteLine(sum);
        robotStart = (-1, -1);
        for (int r = 0; r < exp.GetLength(0); r++)
        {
            for (int c = 0; c < exp.GetLength(1); c++)
            {
                if (exp[r, c] == '@')
                    robotStart = (r, c);
            }
        }
        if (robotStart == (-1, -1))
            throw new NotImplementedException();
        Move(robotStart, moves2, exp);
        PrintMap(exp);
        sum = 0;
        for (int r = 0; r < exp.GetLength(0); r++)
        {
            for (int c = 0; c < exp.GetLength(1); c++)
            {
                if (exp[r, c] == '[')
                {
                    sum += (100 * r) + c;
                }
            }
        }
        Console.WriteLine(sum);

    }
    public char[,] ExpandMap(char[,] m)
    {
        List<List<char>> map = new List<List<char>>();
        for (int r = 0; r < m.GetLength(0); r++)
        {
            List<char> row = new List<char>();
            for (int c = 0; c < m.GetLength(1); c++)
            {
                var ec = ExpandChar(m[r, c]);
                row.Add(ec.Item1);
                row.Add(ec.Item2);
            }
            map.Add(row);
        }
        char[,] ret = new char[map.Count, map[0].Count];
        for (int r = 0; r < ret.GetLength(0); r++)
        {
            for (int c = 0; c < ret.GetLength(1); c++)
            {
                ret[r, c] = map[r][c];
            }
        }
        return ret;
    }
    public (char, char) ExpandChar(char c)
    {
        switch (c)
        {
            case '#':
                return ('#', '#');
            case 'O':
                return ('[', ']');
            case '.':
                return ('.', '.');
            case '@':
                return ('@', '.');
        }
        throw new NotImplementedException();
    }

    public void Move((int, int) cur, List<(int, int)> moves, char[,] m)
    {
        if (moves.Count == 0)
            return;
        var move = moves[0];
        moves.RemoveAt(0);
        var n = GoDir(cur, move);
        var (rn, cn) = n;
        switch (GetDir(cur, move, m))
        {
            case '.':
                MoveItem(cur, move, m);
                Move(GoDir(cur, move), moves, m);
                return;
            case 'O':
                if (MoveBox(n, move, m))
                {
                    MoveItem(cur, move, m);
                    Move(n, moves, m);
                    return;
                }
                else
                {
                    Move(cur, moves, m);
                    return;
                }
            case '#':
                Move(cur, moves, m);
                return;
            case '[':
            case ']':
                if (MoveWideBox(n, move, m))
                {
                    MoveItem(cur, move, m);
                    Move(n, moves, m);
                    return;
                }
                else
                {
                    Move(cur, moves, m);
                    return;
                }
        }
    }
    public void MoveItem((int, int) i, (int, int) dir, char[,] m)
    {
        var n = GoDir(i, dir);
        var (rn, cn) = n;
        var (r, c) = i;

        char item = m[r, c];
        m[rn, cn] = item;
        m[r, c] = '.';

    }
    public bool MoveWideBox((int, int) box, (int, int) dir, char[,] m)
    {
        (int, int) left;
        (int, int) right;
        var (br, bc) = box;
        if (m[br, bc] == '[')
        {
            left = box;
            right = (br, bc + 1);
            if (m[br, bc + 1] != ']')
            {
                throw new Exception($"expected ] at {(br, bc + 1)}");
            }
        }
        else
        {
            right = box;
            left = (br, bc - 1);
            if (m[br, bc - 1] != '[')
            {
                throw new Exception($"expected [ at {(br, bc - 1)}");
            }
        }
        if (dir == LEFT)
        {
            switch (GetDir(left, dir, m))
            {
                case null:
                    return false;
                case ']':
                    if (MoveWideBox(GoDir(left, dir), dir, m))
                        break;
                    else
                        return false;
                case '#':
                    return false;
                case '.':
                    break;
            }
            MoveItem(left, dir, m);
            MoveItem(right, dir, m);
        }
        else if (dir == RIGHT)
        {
            switch (GetDir(right, dir, m))
            {
                case null:
                    return false;
                case '[':
                    if (MoveWideBox(GoDir(right, dir), dir, m))
                        break;
                    else
                        return false;
                case '#':
                    return false;
                case '.':
                    break;
            }
            MoveItem(right, dir, m);

            MoveItem(left, dir, m);
        }
        else if (dir == DOWN || dir == UP)
        {
            var cl = GetDir(left, dir, m);
            var cr = GetDir(right, dir, m);
            if (cl == '#' || cr == '#')
                return false;
            var lBox = cl == '[' || cl == ']';
            var rBox = cr == '[' || cr == ']';
            var sameBox = cl == '[' && cr == ']';
            if (lBox || rBox)
            {
                if (lBox && !CheckWideBox(GoDir(left, dir), dir, m))
                    return false;
                if (rBox && !sameBox && !CheckWideBox(GoDir(right, dir), dir, m))
                    return false;
                if (lBox)
                {
                    var success = MoveWideBox(GoDir(left, dir), dir, m);
                    if (!success)
                        throw new NotSupportedException("couldnt move box even though check succeeded");
                }
                if (rBox && !sameBox)
                {
                    var success = MoveWideBox(GoDir(right, dir), dir, m);
                    if (!success)
                        throw new NotSupportedException("couldnt move box even though check succeeded");

                }
            }

            MoveItem(left, dir, m);
            MoveItem(right, dir, m);
        }

        return true;

    }
    public bool CheckWideBox((int, int) box, (int, int) dir, char[,] m)
    {
        (int, int) left;
        (int, int) right;
        var (br, bc) = box;
        if (m[br, bc] == '[')
        {
            left = box;
            right = (br, bc + 1);
        }
        else
        {
            right = box;
            left = (br, bc - 1);
        }
        if (dir == LEFT)
        {
            switch (GetDir(left, dir, m))
            {
                case null:
                    return false;
                case ']':
                    if (CheckWideBox(GoDir(left, dir), dir, m))
                        break;
                    else
                        return false;
                case '#':
                    return false;
                case '.':
                    break;
            }
        }
        else if (dir == RIGHT)
        {
            switch (GetDir(left, dir, m))
            {
                case null:
                    return false;
                case '[':
                    if (CheckWideBox(GoDir(right, dir), dir, m))
                        break;
                    else
                        return false;
                case '#':
                    return false;
                case '.':
                    break;
            }
        }
        else if (dir == DOWN || dir == UP)
        {
            var cl = GetDir(left, dir, m);
            var cr = GetDir(right, dir, m);
            if (cl == '#' || cr == '#')
                return false;
            if (cl == '[' || cl == ']')
                if (!CheckWideBox(GoDir(left, dir), dir, m))
                    return false;
            if (cr == '[' || cr == ']')
                if (!CheckWideBox(GoDir(right, dir), dir, m))
                    return false;

        }
        return true;

    }
    public bool MoveBox((int, int) box, (int, int) dir, char[,] m)
    {
        if (CheckDir(box, dir, m))
        {
            MoveItem(box, dir, m);
            return true;
        }
        else
        {
            var n = GoDir(box, dir);
            var (rn, cn) = n;
            switch (m[rn, cn])
            {
                case 'O':
                    if (MoveBox(n, dir, m))
                    {
                        MoveItem(box, dir, m);
                        return true;
                    }
                    else
                        return false;
                case '#':
                    return false;
                default:
                    return false;
            }
        }
    }
    public (int, int) GoDir((int, int) cur, (int, int) dir)
    {
        return (cur.Item1 + dir.Item1, cur.Item2 + dir.Item2);
    }
    public char? GetDir((int, int) cur, (int, int) dir, char[,] map)
    {
        var (r, c) = GoDir(cur, dir);
        if (r < 0)
            return null;
        if (c < 0)
            return null;
        if (r >= map.GetLength(0))
            return null;
        if (c >= map.GetLength(1))
            return null;
        var (cr, cc) = cur;
        return map[r, c];

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
        var (cr, cc) = cur;
        var test = map[r, c];
        return test == '.';
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
    static (int, int) UP = (-1, 0);
    static (int, int) DOWN = (1, 0);
    static (int, int) LEFT = (0, -1);
    static (int, int) RIGHT = (0, 1);

    public (char[,], List<(int, int)>) ParseRows(string input)
    {
        List<char[]> map = new();
        List<(int, int)> movements = new List<(int, int)>();

        using (StringReader sr = new StringReader(input))
        {
            string? line;
            while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
            {
                map.Add(line.ToCharArray().ToArray());
            }
            while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
            {
                movements.AddRange(line.ToCharArray().Select(c =>
                {
                    switch (c)
                    {
                        case '<':
                            return LEFT;
                        case '>': return RIGHT;
                        case '^': return UP;
                        case 'v': return DOWN;
                        default: throw new NotSupportedException("bad movement");
                    }
                }));
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

        return (m, movements);
    }

}

