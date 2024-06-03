using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day21 : IDay
{
  public enum D
  {
    Up,
    Down,
    Left,
    Right,
  }
  public (int r, int c) GoDirection((int r, int c) coord, D dir)
  {
    (var r, var c) = coord;
    if (dir == D.Up)
      return (r - 1, c);
    if (dir == D.Down)
      return (r + 1, c);
    if (dir == D.Left)
      return (r, c - 1);
    if (dir == D.Right)
      return (r, c + 1);
    throw new Exception("bad direction");
  }
  public void IterateGrid<T>(T[,] pattern, Action<T, (int r, int c)> action)
  {
    for (int r = 0; r < pattern.GetLength(0); r++)
    {
      for (int c = 0; c < pattern.GetLength(1); c++)
      {
        action(pattern[r, c], (r, c));
      }
    }
  }

  public void Run(string input)
  {
    ((int r, int c) start, char[,] pattern) = ParseRows(input);
    Console.WriteLine(Plots(start, 26501365, pattern));
    return;
    //Print(pattern);
    //I didn't really find this problem interesting to solve, so mostly grabbed code from 
    //https://github.com/rtrinh3/AdventOfCode/blob/1dc62df619c95a28fb09b9d0c32bd6debcc0c5e0/Aoc2023/Day21.cs
    long maps = 26501365 / pattern.GetLength(0);
    int halfHeight = pattern.GetLength(0) / 2;
    long oddTiles = Plots(start, pattern.GetLength(0), pattern);
    long evenTiles = Plots(start, pattern.GetLength(0) + 1, pattern);

    long topCorner = Plots((pattern.GetLength(0) - 1, halfHeight), pattern.GetLength(0) - 1, pattern);
    long bottomCorner = Plots((0, halfHeight), pattern.GetLength(0) - 1, pattern);
    long leftCorner = Plots((halfHeight, pattern.GetLength(1) - 1), pattern.GetLength(0) - 1, pattern);
    long rightCorner = Plots((halfHeight, 0), pattern.GetLength(0) - 1, pattern);

    long topRightEvenDiagonal = Plots((pattern.GetLength(0) - 1, 0), halfHeight - 1, pattern);
    long topRightOddDiagonal = Plots((pattern.GetLength(0) - 1, 0), pattern.GetLength(0) + halfHeight - 1, pattern);
    long bottomRightEvenDiagonal = Plots((0, 0), halfHeight - 1, pattern);
    long bottomRightOddDiagonal = Plots((0, 0), pattern.GetLength(0) + halfHeight - 1, pattern);
    long bottomLeftEvenDiagonal = Plots((0, pattern.GetLength(1) - 1), halfHeight - 1, pattern);
    long bottomLeftOddDiagonal = Plots((0, pattern.GetLength(1) - 1), pattern.GetLength(0) + halfHeight - 1, pattern);
    long topLeftEvenDiagonal = Plots((pattern.GetLength(0) - 1, pattern.GetLength(1) - 1), halfHeight - 1, pattern);
    long topLeftOddDiagonal = Plots((pattern.GetLength(0) - 1, pattern.GetLength(1) - 1), pattern.GetLength(0) + halfHeight - 1, pattern);
    long sum = oddTiles +
                topCorner + bottomCorner + leftCorner + rightCorner +
                maps * (topRightEvenDiagonal + bottomRightEvenDiagonal + bottomLeftEvenDiagonal + topLeftEvenDiagonal) +
                (maps - 1) * (topRightOddDiagonal + bottomRightOddDiagonal + bottomLeftOddDiagonal + topLeftOddDiagonal);
    // case i == 0 already handled as the first term of the sum above
    for (long i = 1; i < maps; i++)
    {
      if (i % 2 == 0)
      {
        sum += 4 * i * oddTiles;
      }
      else
      {
        sum += 4 * i * evenTiles;
      }
    }
    Console.WriteLine(sum);
  }
  public void Print(char[,] pattern)
  {
    for (int r = 0; r < pattern.GetLength(0); r++)
    {
      for (int c = 0; c < pattern.GetLength(1); c++)
      {
        Console.Write(pattern[r, c]);

      }
      Console.WriteLine();
    }
  }
  public IEnumerable<D> Possible()
  {
    List<D> dirs = new List<D>() { D.Up, D.Down, D.Left, D.Right };
    return dirs;
  }
  public long Plots((int r, int c) start, int goal, char[,] pattern)
  {
    int height = pattern.GetLength(0);
    int width = pattern.GetLength(1);

    Queue<(int r, int c)> queue = new();
    queue.Enqueue(start);
    Dictionary<(int r, int c), ((int r, int c), int distance)> parentsDistances = new();
    parentsDistances[start] = (start, 0);
    while (queue.Count > 0)
    {
      var current = queue.Dequeue();
      var possible = Possible().Select(d => GoDirection(current, d))
          .Select(n => (r: ((n.r % height) + height) % height, c: ((n.c % width) + width) % width))
          .Where(n => n.r >= 0)
          .Where(n => n.c >= 0)
          .Where(n => n.r < pattern.GetLength(0))
          .Where(n => n.c < pattern.GetLength(1))
          .Where(n => pattern[n.r, n.c] != '#');
      foreach (var next in possible)
      {
        if (!parentsDistances.ContainsKey(next))
        {
          parentsDistances[next] = (current, parentsDistances[current].distance + 1);
          queue.Enqueue(next);
        }
      }
    }
    int count = parentsDistances.Count(x => x.Value.distance <= goal && x.Value.distance % 2 == (goal % 2));
    return count;
  }
  public ((int r, int c), char[,]) ParseRows(string input)
  {
    (int r, int c) = (0, 0);
    List<List<char>> rows = new List<List<char>>();
    int ln = 0;
    foreach (string line in input.Split('\n'))
    {
      if (line.Contains("S"))
      {
        r = ln;
        c = line.IndexOf("S");
      }
      rows.Add(line.ToCharArray().ToList());
      ln++;
    }
    char[,] ret = new char[rows.Count, rows[0].Count];
    for (int ri = 0; ri < rows.Count; ri++)
    {
      for (int ci = 0; ci < rows[0].Count; ci++)
      {
        ret[ri, ci] = rows[ri][ci];
      }
    }
    return ((r, c), ret);
  }
}