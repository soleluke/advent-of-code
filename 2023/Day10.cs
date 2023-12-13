using System.Drawing;
using System.Text.RegularExpressions;

internal class Day010
{
  private static ((int dx1, int dy1), (int dx2, int dy2)) GetPipeConnections(char pipe, int x, int y)
  {
    return pipe switch
    {
      '|' => ((x, y - 1), (x, y + 1)),
      '-' => ((x - 1, y), (x + 1, y)),
      'L' => ((x, y - 1), (x + 1, y)),
      'J' => ((x, y - 1), (x - 1, y)),
      '7' => ((x - 1, y), (x, y + 1)),
      'F' => ((x + 1, y), (x, y + 1)),
      _ => throw new Exception($"Bad Pipe: {pipe}"),
    };
  }

  public static (string part1, string part2) Run(string input)
  {
    // get our pipes
    var pipes = new Dictionary<(int x0, int y0), ((int x1, int y1) connection1, (int x2, int y2) connection2)>();
    var grid = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    int height = grid.Length;
    int width = grid[0].Length;
    var start = (x: -1, y: -1);
    for (int y = 0; y < height; y++)
    {
      for (int x = 0; x < width; x++)
      {
        switch (grid[y][x])
        {
          case '.':
            continue;
          case 'S':
            start = (x, y);
            break;
          default:
            pipes.Add((x, y), GetPipeConnections(grid[y][x], x, y));
            break;
        }
      }
    }

    // build our graph
    var graph = new Dictionary<(int x0, int y0), List<(int x1, int y1)>>();
    foreach (var pipe in pipes)
    {
      var (x, y) = pipe.Key;
      var ((dx1, dy1), (dx2, dy2)) = pipe.Value;
      if (!graph.ContainsKey((x, y)))
      {
        graph[(x, y)] = new List<(int dx, int dy)>();
      }
      AddConnectionToGraph(pipe.Key, (dx1, dy1));
      AddConnectionToGraph(pipe.Key, (dx2, dy2));
    }
    void AddConnectionToGraph((int x, int y) current, (int dx, int dy) connection)
    {
      if (pipes.TryGetValue(connection, out var connections) &&
          (connections.connection1 == current || connections.connection2 == current))
      {
        graph[current].Add(connection);
      }
    }

    // manually add the start, it's the only one with more than two possible connections
    graph[start] = new List<(int x, int y)>();
    foreach (var pipe in pipes)
    {
      if (pipe.Key != start && (pipe.Value.connection1 == start || pipe.Value.connection2 == start))
      {
        graph[start].Add(pipe.Key);
      }
    }

    var visited = new bool[width, height];
    var maskedLoop = new bool[width, height];
    var enclosedByLoop = new bool[width, height];
    var distances = new int[width, height];
    var maxDistance = -1;
    var queue = new Queue<((int x, int y) p, int n, List<(int x, int y)> path)>();
    queue.Enqueue((start, 0, new List<(int x, int y)>()));
    visited[start.x, start.y] = true;
    while (queue.Count > 0)
    {
      var current = queue.Dequeue();
      var hasConnection = false;
      foreach (var connection in graph[current.p])
      {
        hasConnection |= ProcessPipe(connection, current.n, current.path);
      }
      // end of path
      if (!hasConnection)
      {
        foreach (var p in current.path)
        {
          maskedLoop[p.x, p.y] = true;
        }
      }
    }
    bool ProcessPipe((int x, int y) point, int n, List<(int x, int y)> path)
    {
      if (graph.ContainsKey(point) && !visited[point.x, point.y])
      {
        var pathNew = new List<(int x, int y)>(path);
        n += 1;
        pathNew.Add(point);
        visited[point.x, point.y] = true;
        distances[point.x, point.y] = n;
        maxDistance = Math.Max(maxDistance, n);
        queue.Enqueue((point, n, pathNew));
        return true;
      }

      return false;
    }

    var loopPoints = new List<(int x, int y)>();
    var currentPoint = start;
    var previousPoint = (-1, -1); // Invalid initial point
    do
    {
      loopPoints.Add(currentPoint);
      // Find the next point in the loop that is not the previous point
      var nextPoint = graph[currentPoint].FirstOrDefault(p => p != previousPoint);
      previousPoint = currentPoint;
      currentPoint = nextPoint;
    } while (currentPoint != start && currentPoint != (0, 0)); // Assuming (0, 0) won't be part of the loop

    var enclosedCount = 0;
    for (var i = 0; i < width; i++)
    {
      for (var j = 0; j < height; j++)
      {
        var enclosed = IsPointInsideLoop(loopPoints, (i, j));
        enclosedByLoop[i, j] = enclosed;
        if (enclosed)
        {
          enclosedCount++;
        }
      }
    }
    return (maxDistance.ToString(), enclosedCount.ToString());
  }
  public static bool IsPointInsideLoop(List<(int x, int y)> loopPoints, (int x, int y) point)
  {
    if (loopPoints.Contains(point))
    {
      return false;
    }

    int intersections = 0;
    int n = loopPoints.Count;

    for (int i = 0; i < n; i++)
    {
      (int x1, int y1) = loopPoints[i];
      (int x2, int y2) = loopPoints[(i + 1) % n];
      if ((y1 > point.y) != (y2 > point.y))
      {
        int intersectX = x1 + (point.y - y1) * (x2 - x1) / (y2 - y1);

        if (intersectX > point.x)
        {
          intersections++;
        }
      }
    }

    // When odd, we are starting from inside the loop (crossed to the other side and not back in)
    return intersections % 2 != 0;
  }

}

public class Day10 : IDay
{
  public enum Direction
  {
    East,
    West,
    North,
    South
  }
  private static Direction GetOpposite(Direction d)
  {
    switch (d)
    {
      case Direction.East:
        return Direction.West;
      case Direction.West:
        return Direction.East;
      case Direction.South:
        return Direction.North;
      case Direction.North:
        return Direction.South;
    }
    throw new Exception("invalid Direction");
  }
  private static bool CheckDirection(Direction source, Direction dest)
  {
    return dest == GetOpposite(source);
  }
  public class Pipe
  {
    public override string ToString()
    {
      return $"{Position.x} {Position.y} {string.Join(',', Connections)}";
    }
    public bool InLoop { get; set; }
    public bool Visited { get; set; }
    public bool Enclosed { get; set; }
    public (int x, int y) Position { get; set; }
    public bool Equals(Pipe pipe)
    {
      return Position.x == pipe.Position.x && Position.y == pipe.Position.y;
    }
    public bool Equals((int x, int y) pos)
    {
      return Position.x == pos.x && Position.y == pos.y;
    }
    public ICollection<Direction> Connections { get; set; }
    public Pipe(Direction one, Direction two, int x, int y)
    {
      Position = new(x, y);
      Connections = new List<Direction>() { one, two };
    }
    public Pipe(int x, int y)
    {
      Position = new(x, y);
      Connections = new List<Direction>();
    }
    public bool ConnectsTo(Pipe pipe)
    {
      if (!this.IsAdjacent(pipe))
        return false;
      return Connections.Zip(pipe.Connections).Select(z => CheckDirection(z.First, z.Second)).Any();
    }
    public bool IsAdjacent(Pipe pipe)
    {
      if (Position.Item1 == pipe.Position.Item1 && Math.Abs(Position.Item2 - pipe.Position.Item2) <= 1)
        return true;
      if (Position.Item2 == pipe.Position.Item2 && Math.Abs(Position.Item1 - pipe.Position.Item1) <= 1)
        return true;
      return false;
    }
    public Pipe GetConnected(Direction d, Map map)
    {

      if (!Connections.Contains(d))
        throw new Exception($"invalid connection: {d} - {string.Join(',', Connections)}");
      int x = Position.Item1;
      int y = Position.Item2;
      switch (d)
      {
        case Direction.North:
          y--;
          break;
        case Direction.South:
          y++;
          break;
        case Direction.West:
          x--;
          break;
        case Direction.East:
          x++;
          break;
      }
      Pipe pipe = map.Pipes[y][x];
      if (!pipe.Connections.Any())
        throw new Exception("bad connection");
      return pipe;
    }
  }
  public class Map
  {
    public IList<IList<Pipe>> Pipes { get; set; }
    public (int x, int y) AnimalPos { get; set; }
    public Map()
    {
      AnimalPos = new(0, 0);
      Pipes = new List<IList<Pipe>>();
    }

  }
  static IEnumerable<Point> vectors = new Point[] { new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0) };
  static char[][] directions = { new char[] { '|', 'F', '7', 'S' }, new char[] { '-', 'J', '7', 'S' }, new char[] { '|', 'J', 'L', 'S' }, new char[] { '-', 'L', 'F', 'S' } };
  public void Run(string input)
  {
    //Map map = ParseStuff(input);
    //Console.WriteLine(string.Join('\n', map.Pipes.Select(p => string.Join(',', p.Select(a => a?.ToString() ?? ".")))));
    //IEnumerable<Pipe> loop = GetLoop(map);
    //Console.WriteLine(Math.Ceiling((double)loop.Count() / 2));
    //IEnumerable<Task<bool>> enclosed = map.Pipes.SelectMany((p, y) => p.Select((p2, x) => IsEnclosed(new Tuple<int, int>(x, y), loop, map)));
    //Console.WriteLine("enclosed: " + Task.WhenAll(enclosed).Result.Where(r => r).Count());
    //FloodFill(map.Pipes.First().First(), map);
    //Console.WriteLine(map.Pipes.Select(p => p.Count(p => !p.Visited && !p.InLoop)).Sum());
    //Console.WriteLine(map.Pipes.Select(p => p.Count(p => p.Visited && !p.InLoop)).Sum());
    //Console.WriteLine(CalculateTotalEnclosedTiles(loop, map));



    Console.WriteLine(Day010.Run(input));


  }

  public int CalculateTotalEnclosedTiles(IEnumerable<Pipe> loop, Map map)
  {
    var total = 0;

    var polygonVertices = loop
        .Where(pos =>
        {
          return (pos.Connections.Contains(Direction.North) && pos.Connections.Contains(Direction.South))
          && (pos.Connections.Contains(Direction.East) && pos.Connections.Contains(Direction.West));
        })
        .ToArray();

    for (var row = 0; row < map.Pipes.Count(); row++)
    {
      for (var col = 0; col < map.Pipes.First().Count(); col++)
      {
        var position = map.Pipes[row][col];

        // Random pipes that are not connected to the main loop also count
        if (!loop.Contains(position) && IsTileInsidePolygon(position, polygonVertices))
        {
          total++;
        }
      }
    }

    return total;
  }
  private static bool IsTileInsidePolygon(Pipe tilePosition, IList<Pipe> polygonVertices)
  {
    // Ray Casting Algorithm to determine if a point is inside a polygon (pipe loop)
    var isInside = false;

    // Iterates over adjacent positions of the polygon
    // For each pair, it checks if the horizontal line extending to the right intersects with the polygon edge
    var j = polygonVertices.Count() - 1;
    for (int i = 0; i < polygonVertices.Count(); i++)
    {
      var vertexA = polygonVertices[i];
      var vertexB = polygonVertices[j];

      if (vertexA.Position.y < tilePosition.Position.y && vertexB.Position.y >= tilePosition.Position.y
          || vertexB.Position.y < tilePosition.Position.y && vertexA.Position.y >= tilePosition.Position.y)
      {
        if (vertexA.Position.x
                + (tilePosition.Position.y - vertexA.Position.y)
                / (vertexB.Position.y - vertexA.Position.y)
                * (vertexB.Position.x - vertexA.Position.x)
            < tilePosition.Position.x)
        {
          isInside = !isInside;
        }
      }

      j = i;
    }

    return isInside;
  }
  public void FloodFill(Pipe pipe, Map map)
  {
    if (pipe.InLoop || pipe.Enclosed)
      return;
    pipe.Visited = true;
    int x = pipe.Position.Item1;
    int y = pipe.Position.Item2;
    if (y < map.Pipes.Count() - 1)
      FloodFill(map.Pipes[y + 1][x], map);
    if (y > 0)
      FloodFill(map.Pipes[y - 1][x], map);
    if (x < map.Pipes.First().Count())
      FloodFill(map.Pipes[y][x + 1], map);
    if (x > 0)
      FloodFill(map.Pipes[y][x - 1], map);
  }
  public Task<bool> IsEnclosed(Tuple<int, int> point, IEnumerable<Pipe> loop, Map map)
  {
    int x = point.Item1;
    int y = point.Item2;
    return Task.Run(() =>
    {

      Pipe pPipe = map.Pipes[y][x];
      if (pPipe.InLoop)
        return false;
      int up = map.Pipes.Where((p, i) => i < y).Select(p => p[x]).Count(p => p.InLoop);
      int down = map.Pipes.Where((p, i) => i > y).Select(p => p[x]).Count(p => p.InLoop);
      int left = map.Pipes[y].Where((p, i) => i < x).Count(p => p.InLoop);
      int right = map.Pipes[y].Where((p, i) => i > x).Count(p => p.InLoop);
      int vert = up + down;
      int hor = left + right;
      bool enclosed = vert % 2 != 0 && hor % 2 != 0;
      return enclosed;
    });
  }
  public static IEnumerable<Pipe> GetRay((int x, int y) start, Direction d, Map map)
  {
    int x = start.Item1;
    int y = start.Item2;
    switch (d)
    {
      case Direction.North:
        return map.Pipes.Where((p, i) => i < y).Select(p => p[x]);
      case Direction.South:
        return map.Pipes.Where((p, i) => i > y).Select(p => p[x]);
      case Direction.West:
        return map.Pipes[y].Where((p, i) => i < x);
      case Direction.East:
        return map.Pipes[y].Where((p, i) => i > x);
    }
    throw new Exception("NO");
  }
  public async Task<bool> CheckRay((int x, int y) start, Direction d, IEnumerable<Pipe> loop, Map map)
  {
    IEnumerable<Pipe> ray = GetRay(start, d, map);
    Pipe? prev = null;
    bool loopFound = false;
    foreach (Pipe pipe in ray)
    {
      if (pipe.InLoop)
      {
        loopFound = true;
        break;
      }
    }
    if (loopFound && prev != null)
    {
      List<Task<bool>> tasks = new List<Task<bool>>();
      if (d == Direction.North || d == Direction.South)
      {
        Task<bool> e = CheckRay(prev.Position, Direction.East, loop, map);
        Task<bool> w = CheckRay(prev.Position, Direction.West, loop, map);
        tasks.Add(e);
        tasks.Add(w);
      }
      else
      {
        Task<bool> n = CheckRay(prev.Position, Direction.North, loop, map);
        Task<bool> s = CheckRay(prev.Position, Direction.South, loop, map);
        tasks.Add(n);
        tasks.Add(s);
      }
      bool[] res = await Task.WhenAll(tasks);
      return res.All(r => r);
    }
    return false;
  }
  public IEnumerable<Pipe> GetLoop(Map map)
  {
    List<Pipe> loop = new List<Pipe>();
    //find connecting pipes to starting pos
    int x = map.AnimalPos.Item1;
    int y = map.AnimalPos.Item2;
    Pipe? start = map.Pipes[y][x];
    if (start == null)
      throw new Exception("bad start: " + map.AnimalPos.ToString());
    List<Pipe?> adjacents = new List<Pipe?>();
    Func<Pipe?, bool> predicate = p => p != null && p.ConnectsTo(start) && !p.Equals(start);
    //Console.WriteLine(start.ToString());
    if (y > 0)
      adjacents.AddRange(map.Pipes[y - 1].Where(predicate));
    if (y < map.Pipes.Count() - 1)
      adjacents.AddRange(map.Pipes[y + 1].Where(predicate));
    if (x > 0)
      adjacents.AddRange(map.Pipes.Select((p) => p[x - 1]).Where(predicate));
    if (x < map.Pipes.First().Count() - 1)
      adjacents.AddRange(map.Pipes.Select((p) => p[x + 1]).Where(predicate));
    List<Direction> dirs = new List<Direction>();
    foreach (Pipe? p in adjacents)
    {
      if (p != null)
      {
        if (p.Position.Item2 == y && p.Position.Item1 == x + 1 && p.Connections.Any(d => d == Direction.West))
          dirs.Add(Direction.East);
        if (p.Position.Item2 == y && p.Position.Item1 == x - 1 && p.Connections.Any(d => d == Direction.East))
          dirs.Add(Direction.West);
        if (p.Position.Item1 == x && p.Position.Item2 == y + 1 && p.Connections.Any(d => d == Direction.North))
          dirs.Add(Direction.South);
        if (p.Position.Item1 == x && p.Position.Item2 == y - 1 && p.Connections.Any(d => d == Direction.South))
          dirs.Add(Direction.North);
      }
    }
    if (dirs.Count() != 2)
      throw new Exception("bad start: " + dirs.Count());
    start.Connections = dirs;
    return GetChain(start, start.Connections.First(), map);
  }
  public IEnumerable<Pipe> GetChain(Pipe pipe, Direction source, Map map)
  {
    pipe.InLoop = true;
    Pipe next = pipe.GetConnected(source, map);
    //Console.WriteLine(pipe.ToString() + " " + next.ToString());
    if (next.Equals(map.AnimalPos))
      return new List<Pipe>() { pipe };
    return new List<Pipe>() { pipe }.Concat(GetChain(next, next.Connections.First(c => c != GetOpposite(source)), map));
  }
  private Map ParseStuff(string input)
  {
    Map map = new Map();
    using (StringReader reader = new StringReader(input))
    {
      string? line;

      while ((line = reader.ReadLine()) != null)
      {
        IList<Pipe> pipes = line.ToCharArray().Select((c, i) => CharToPipe(c, i, map.Pipes.Count())).ToList();
        Pipe? animal = pipes.FirstOrDefault(p => p != null && p.Connections.Distinct().Count() == 1);
        if (animal != null)
          map.AnimalPos = animal.Position;
        map.Pipes.Add(pipes);
      }
    }
    return map;
  }
  private Pipe CharToPipe(char input, int x, int y)
  {
    switch (input)
    {
      case '|':
        return new Pipe(Direction.North, Direction.South, x, y);
      case '-':
        return new Pipe(Direction.East, Direction.West, x, y);
      case 'L':
        return new Pipe(Direction.North, Direction.East, x, y);
      case 'J':
        return new Pipe(Direction.North, Direction.West, x, y);
      case '7':
        return new Pipe(Direction.South, Direction.West, x, y);
      case 'F':
        return new Pipe(Direction.South, Direction.East, x, y);
      case 'S':
        return new Pipe(Direction.North, Direction.North, x, y);
      default:
        return new Pipe(x, y);

    }
  }
}