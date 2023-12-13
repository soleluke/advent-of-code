using System.Drawing;

public class Day11 : IDay
{
  public class Galaxy
  {
    public Point Pos { get; set; }
  }
  public void Run(string input)
  {
    var space = ParseSpace(input);
    space = ExpandSpace(space);
    //Console.WriteLine(string.Join('\n', space.Select(s => new string(s.ToArray()))));
    IEnumerable<Galaxy> galaxies = GetGalaxies(space);
    IEnumerable<Tuple<Galaxy, Galaxy>> pairs = GetPairs(galaxies);
    int height = space.Count();
    int width = space.First().Count();
    Console.WriteLine($"{galaxies.Count()}");
    IEnumerable<Task<long>> tasks = pairs.Select(p => ShortestPath(p, space));
    long sum = Task.WhenAll(tasks).Result.Sum();
    Console.WriteLine(sum);

  }
  public long GetYExpansion(int x1, int x2, int y, List<List<char>> space)
  {
    if (x2 > x1)
    {
      return space.Skip(x1).Take(x2 - x1).Select(s => s[y]).Where(i => i == 'X').Count();
    }
    else if (x1 > x2)
    {
      return space.Skip(x2).Take(x1 - x2).Select(s => s[y]).Where(i => i == 'X').Count();
    }
    return 0;
  }
  public long GetXExpansion(int y1, int y2, int x, List<List<char>> space)
  {
    if (y2 > y1)
    {
      return space[x].Skip(y1).Take(y2 - y1).Where(i => i == 'X').Count();
    }
    else if (y1 > y2)
    {
      return space[x].Skip(y2).Take(y1 - y2).Where(i => i == 'X').Count();
    }
    return 0;
  }
  public Task<long> ShortestPath(Tuple<Galaxy, Galaxy> g, List<List<char>> space)
  {
    int expansion = 1000000;
    return Task.Run(() =>
    {
      long expandedX = GetXExpansion(g.Item1.Pos.X, g.Item2.Pos.X, g.Item1.Pos.Y, space);
      long xdiff = Math.Abs(g.Item1.Pos.X - g.Item2.Pos.X);
      xdiff -= expandedX;
      xdiff += expandedX * expansion;
      long expandedY = GetYExpansion(g.Item1.Pos.Y, g.Item2.Pos.Y, g.Item1.Pos.X, space);
      long ydiff = Math.Abs(g.Item1.Pos.Y - g.Item2.Pos.Y);
      ydiff -= expandedY;
      ydiff += expandedY * expansion;
      //Console.WriteLine($"{g.Item1.Pos} {g.Item2.Pos} {xdiff},{expandedX} {ydiff},{expandedY}");
      return xdiff + ydiff;
    });
  }
  public IEnumerable<Tuple<Galaxy, Galaxy>> GetPairs(IEnumerable<Galaxy> galaxies)
  {
    List<Tuple<Galaxy, Galaxy>> pairs = new List<Tuple<Galaxy, Galaxy>>();
    foreach (var g in galaxies)
    {
      pairs.AddRange(galaxies.Where(ga => !ga.Equals(g) && !pairs.Select(p => p.Item1).Distinct().Contains(ga)).Select(ga => new Tuple<Galaxy, Galaxy>(g, ga)));
    }
    return pairs;
  }
  public IEnumerable<Galaxy> GetGalaxies(List<List<char>> space)
  {
    List<Galaxy> galaxies = new List<Galaxy>();
    for (int i = 0; i < space.Count(); i++)
    {
      for (int j = 0; j < space[i].Count(); j++)
      {
        if (space[i][j] == '#')
        {
          galaxies.Add(new Galaxy() { Pos = new Point(j, i) });
        }
      }
    }
    return galaxies;
  }
  public List<List<char>> ExpandSpace(List<List<char>> space)
  {
    List<List<char>> newSpace = new List<List<char>>();
    foreach (List<char> row in space)
    {
      if (!row.Any(r => r == '#'))
      {
        newSpace.Add(row.Select(r => 'X').ToList());

      }
      else
      {
        newSpace.Add(row.ToList());
      }
    }
    int count = newSpace.First().Count();
    for (int i = 0; i < count; i++)
    {
      IEnumerable<char> col = newSpace.Select(r => r[i]);
      if (!col.Any(c => c == '#'))
      {
        foreach (var row in newSpace)
        {
          row[i] = 'X';
        }
      }
    }
    return newSpace;
  }
  public List<List<char>> ParseSpace(string input)
  {
    List<List<char>> space = new List<List<char>>();
    string? line;
    using (StringReader sr = new StringReader(input))
    {
      while ((line = sr.ReadLine()) != null)
      {
        space.Add(line.ToCharArray().ToList());
      }
    }
    return space;
  }
}