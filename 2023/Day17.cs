using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day17 : IDay
{

  public void PrintPattern(char[][] pattern)
  {
    Console.WriteLine("Pattern----------");
    Console.WriteLine(string.Join('\n', pattern.Select(c => new string(c))));
  }
  public enum D
  {
    Up,
    Down,
    Left,
    Right,
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
    ImmutableArray<bool[]> visited = ImmutableArray.Create(pattern.Select(p => p.Select(c => false).ToArray()).ToArray());
    var path = Dijkstra(0, 0, pattern.Length - 1, pattern[0].Length - 1, pattern);
    int sum = 0;
    foreach (var item in path)
    {
      (int r, int c) = item.Item3;
      char v = pattern[r][c];
      sum += v - '0';
      switch (item.Item1)
      {
        case D.Up:
          v = '^';
          break;
        case D.Down:
          v = 'v';
          break;
        case D.Left:
          v = '<';
          break;
        case D.Right:
          v = '>';
          break;
      }
      pattern[r][c] = v;
    }
    //PrintPattern(pattern);
    Console.WriteLine(sum);
  }
  public void Visualize(IEnumerable<(D, int, (int, int))> path, char[][] pattern)
  {

  }
  public (int, int) GoDirection((int, int) current, D dir)
  {
    return GoDirection(current.Item1, current.Item2, dir);
  }
  public (int, int) GoDirection(int r, int c, D dir)
  {
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
  public D Opposite(D dir)
  {
    if (dir == D.Up)
      return D.Down;
    if (dir == D.Down)
      return D.Up;
    if (dir == D.Left)
      return D.Right;
    if (dir == D.Right)
      return D.Left;
    throw new Exception("bad direction");
  }
  public IEnumerable<D> Possible(int r, int c, D? curDir)
  {
    List<D> dirs = new List<D>() { D.Up, D.Down, D.Left, D.Right };
    if (curDir == null)
      return dirs;
    dirs.Remove(Opposite(curDir.Value));
    return dirs;
  }
  public IEnumerable<D> Possible((int, int) current, D? curDir)
  {
    return Possible(current.Item1, current.Item2, curDir);
  }
  public IEnumerable<(D, int, (int, int))> ReconstructPath(Dictionary<(D, int, (int, int)), (D, int, (int, int))?> cameFrom, (D, int, (int, int)) current)
  {
    List<(D, int, (int, int))> nodes = new();
    while (current.Item3 != (0, 0))
    {
      nodes.Add(current);
      var node = cameFrom[current];
      if (node != null)
        current = node.Value;
      else
        break;
    }
    return nodes;

  }
  public IEnumerable<(D, int, (int, int))> Dijkstra(int r, int c, int endR, int endC, char[][] pattern)
  {
    int Cost((int, int) n)
    {
      return pattern[n.Item1][n.Item2] - '0';
    }
    var frontier = new PriorityQueue<(D, int, (int, int)), int>();
    frontier.Enqueue((D.Right, 0, (r, c)), 0);
    frontier.Enqueue((D.Down, 0, (r, c)), 0);
    var cameFrom = new Dictionary<(D, int, (int, int)), (D, int, (int, int))?>();
    var costSoFar = new Dictionary<(D, int, (int, int)), int>();
    costSoFar[(D.Right, 0, (r, c))] = 0;
    costSoFar[(D.Down, 0, (r, c))] = 0;
    while (frontier.Count > 0)
    {
      var node = frontier.Dequeue();
      var (dir, run, current) = node;

      if (current == (endR, endC))
      {
        return ReconstructPath(cameFrom, node);
      }
      var last = cameFrom.GetValueOrDefault(node, null);
      var neighbors = Possible(current, dir).Select(d => (d, run: d == dir ? run + 1 : 1, GoDirection(current, d)));
      neighbors = neighbors.Where(n => (n.d != dir && run > 3) || (n.d == dir && run < 10))
        .Where(n => n.Item3.Item1 >= 0)
        .Where(n => n.Item3.Item2 >= 0)
        .Where(n => n.Item3.Item1 < pattern.Length)
        .Where(n => n.Item3.Item2 < pattern[0].Length);
      foreach (var thing in neighbors)
      {
        var neighbor = thing.Item3;
        D nDir = thing.Item1;
        int nRun = thing.Item2;
        int newCost = costSoFar[node] + Cost(neighbor);
        if (!costSoFar.ContainsKey(thing) || newCost < costSoFar[thing])
        {
          costSoFar[thing] = newCost;
          cameFrom[thing] = node;
          frontier.Enqueue(thing, newCost);
        }

      }
    }
    throw new Exception("failed");
  }

  public char[][] ParseRows(string input)
  {
    IEnumerable<char[]> patterns = input.Split('\n').Select(l => l.ToCharArray());
    return patterns.ToArray();
  }
}