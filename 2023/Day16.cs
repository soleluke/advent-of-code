using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day16 : IDay
{
  //public static Dictionary<Tuple<int, int, Directions>, int> memos = new Dictionary<Tuple<int, int, Directions>, int>();
  public class Node
  {
    public List<Directions> Visited { get; set; }
    public bool Energized { get; set; }
    public Node()
    {
      Visited = new List<Directions>();
    }
  }

  public void PrintPattern(char[][] pattern)
  {
    Console.WriteLine("Pattern----------");
    Console.WriteLine(string.Join('\n', pattern.Select(c => new string(c))));
  }
  public bool Compare(char[][] p1, char[][] p2)
  {
    for (int r = 0; r < p1.Length; r++)
    {
      if (!p1[r].SequenceEqual(p2[r]))
        return false;
    }
    return true;
  }
  public void Run(string input)
  {
    char[][] pattern = ParseRows(input);
    Node[,] energized = new Node[pattern.Length, pattern[0].Length];

    InitializeEnergized(pattern.Length, pattern[0].Length);
    List<Task<int>> e = new List<Task<int>>();
    e.Add(ProcessSpot(0, 0, Directions.Right, pattern, InitializeEnergized(pattern.Length, pattern[0].Length)));
    for (int i = 1; i < energized.GetLength(0); i++)
    {
      e.Add(ProcessSpot(i, 0, Directions.Right, pattern, InitializeEnergized(pattern.Length, pattern[0].Length)));
    }
    e.Add(ProcessSpot(0, 0, Directions.Down, pattern, InitializeEnergized(pattern.Length, pattern[0].Length)));
    for (int i = 1; i < energized.GetLength(1); i++)
    {
      e.Add(ProcessSpot(0, i, Directions.Down, pattern, InitializeEnergized(pattern.Length, pattern[0].Length)));
    }
    e.Add(ProcessSpot(energized.GetLength(0), energized.GetLength(1), Directions.Up, pattern, InitializeEnergized(pattern.Length, pattern[0].Length)));
    e.Add(ProcessSpot(energized.GetLength(0), energized.GetLength(1), Directions.Left, pattern, InitializeEnergized(pattern.Length, pattern[0].Length)));
    e.Add(ProcessSpot(0, energized.GetLength(1), Directions.Down, pattern, InitializeEnergized(pattern.Length, pattern[0].Length)));
    e.Add(ProcessSpot(0, energized.GetLength(1), Directions.Left, pattern, InitializeEnergized(pattern.Length, pattern[0].Length)));
    for (int i = 1; i < energized.GetLength(0); i++)
    {
      e.Add(ProcessSpot(i, energized.GetLength(1), Directions.Left, pattern, InitializeEnergized(pattern.Length, pattern[0].Length)));
    }
    e.Add(ProcessSpot(energized.GetLength(0), 0, Directions.Up, pattern, InitializeEnergized(pattern.Length, pattern[0].Length)));
    for (int i = 1; i < energized.GetLength(0); i++)
    {
      e.Add(ProcessSpot(energized.GetLength(0), i, Directions.Up, pattern, InitializeEnergized(pattern.Length, pattern[0].Length)));
    }
    e.Add(ProcessSpot(energized.GetLength(0), 0, Directions.Right, pattern, InitializeEnergized(pattern.Length, pattern[0].Length)));
    //PrintEnergized(pattern);
    Console.WriteLine(Task.WhenAll(e).Result.Max());


  }
  public Node[,] Clone(Node[,] e)
  {
    Node[,] copy = new Node[e.GetLength(0), e.GetLength(1)];
    for (int r = 0; r < e.GetLength(0); r++)
    {
      for (int c = 0; c < e.GetLength(1); c++)
      {
        copy[r, c] = new Node()
        {
          Visited = e[r, c].Visited.ToList(),
          Energized = e[r, c].Energized
        };
      }
    }
    return copy;
  }
  public int CalcEnergized(Node[,] energized)
  {
    int sum = 0;
    for (int r = 0; r < energized.GetLength(0); r++)
    {
      for (int c = 0; c < energized.GetLength(1); c++)
      {
        if (energized[r, c].Energized)
          sum++;
      }
    }
    return sum;
  }
  public void PrintEnergized(Node[,] energized)
  {
    for (int r = 0; r < energized.GetLength(0); r++)
    {
      for (int c = 0; c < energized.GetLength(1); c++)
      {
        if (energized[r, c].Energized)
          Console.Write('#');
        else
          Console.Write('.');
      }
      Console.WriteLine();
    }
  }
  public Node[,] InitializeEnergized(int row, int col)
  {
    Node[,] energized = new Node[row, col];
    for (int r = 0; r < energized.GetLength(0); r++)
    {
      for (int c = 0; c < energized.GetLength(1); c++)
      {
        energized[r, c] = new Node();
      }
    }
    return energized;
  }
  public enum Directions
  {
    Up,
    Down,
    Left,
    Right
  }
  public (int, int) GoInDirection(int r, int c, Directions dir)
  {
    if (dir == Directions.Up)
      return (r - 1, c);
    if (dir == Directions.Down)
      return (r + 1, c);
    if (dir == Directions.Left)
      return (r, c - 1);
    if (dir == Directions.Right)
      return (r, c + 1);
    throw new Exception("bad direction");
  }
  public Task<int> ProcessSpot(int r, int c, Directions dir, char[][] pattern, Node[,] energized)
  {
    return Task.Run(() =>
    {
      if (r >= pattern.Length || r < 0)
        return 0;
      if (c >= pattern[0].Length || c < 0)
        return 0;
      if (energized[r, c].Visited.Contains(dir))
        return 0;
      Tuple<int, int, Directions> key = new(r, c, dir);
      // if (memos.ContainsKey(key))
      // return memos[key];
      int add = 0;
      if (!energized[r, c].Energized)
        add++;
      energized[r, c].Energized = true;
      energized[r, c].Visited.Add(dir);
      int nr, nc;

      switch (pattern[r][c])
      {
        case '.':
          (nr, nc) = GoInDirection(r, c, dir);
          add += ProcessSpot(nr, nc, dir, pattern, energized).Result;
          break;
        case '|':
          if (dir == Directions.Left || dir == Directions.Right)
          {
            (nr, nc) = GoInDirection(r, c, Directions.Up);
            add += ProcessSpot(nr, nc, Directions.Up, pattern, energized).Result;
            (nr, nc) = GoInDirection(r, c, Directions.Down);
            add += ProcessSpot(nr, nc, Directions.Down, pattern, energized).Result;
          }
          else
          {
            (nr, nc) = GoInDirection(r, c, dir);
            add += ProcessSpot(nr, nc, dir, pattern, energized).Result;
          }
          break;
        case '-':
          if (dir == Directions.Up || dir == Directions.Down)
          {
            Node[,] copy = Clone(energized);
            (nr, nc) = GoInDirection(r, c, Directions.Left);
            add += ProcessSpot(nr, nc, Directions.Left, pattern, energized).Result;
            (nr, nc) = GoInDirection(r, c, Directions.Right);
            add += ProcessSpot(nr, nc, Directions.Right, pattern, energized).Result;
          }
          else
          {
            (nr, nc) = GoInDirection(r, c, dir);
            add += ProcessSpot(nr, nc, dir, pattern, energized).Result;
          }
          break;
        case '\\':
          if (dir == Directions.Left)
            dir = Directions.Up;
          else if (dir == Directions.Right)
            dir = Directions.Down;
          else if (dir == Directions.Up)
            dir = Directions.Left;
          else if (dir == Directions.Down)
            dir = Directions.Right;
          (nr, nc) = GoInDirection(r, c, dir);
          add += ProcessSpot(nr, nc, dir, pattern, energized).Result;
          break;
        case '/':
          if (dir == Directions.Left)
            dir = Directions.Down;
          else if (dir == Directions.Right)
            dir = Directions.Up;
          else if (dir == Directions.Up)
            dir = Directions.Right;
          else if (dir == Directions.Down)
            dir = Directions.Left;
          (nr, nc) = GoInDirection(r, c, dir);
          add += ProcessSpot(nr, nc, dir, pattern, energized).Result;
          break;
        default:
          throw new Exception("bad char: " + pattern[r][c]);
      }
      //memos[key] = add;
      return add;
    });
  }

  public char[][] ParseRows(string input)
  {
    IEnumerable<char[]> patterns = input.Split('\n').Select(l => l.ToCharArray());
    return patterns.ToArray();
  }
}