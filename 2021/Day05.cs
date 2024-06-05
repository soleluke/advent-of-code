using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day05 : IDay
{
  public class Coord : IEquatable<Coord>
  {
    public Coord(int x, int y)
    {
      X = x;
      Y = y;
    }
    public int X { get; set; }
    public int Y { get; set; }
    public override bool Equals(object? obj)
    {
      Coord? c = (Coord?)obj;
      return X == c?.X && Y == c.Y;
    }

    public bool Equals(Coord? other)
    {
      return X == other?.X && Y == other.Y;
    }
    public override int GetHashCode()
    {
      return X.GetHashCode() ^ Y.GetHashCode();
    }
  }
  public class Vent
  {
    public Vent(Coord start, Coord end)
    {
      Start = start;
      End = end;
    }
    public Coord Start { get; set; }
    public Coord End { get; set; }
    public void Print()
    {
      Console.WriteLine($"{Start.X},{Start.Y} -> {End.X},{End.Y}");
    }
    public bool Vertical => Start.X == End.X;
    public bool Horizontal => Start.Y == End.Y;
  }

  public void Run(string input)
  {
    var vents = ParseRows(input);
    Dictionary<Coord, int> hits = new Dictionary<Coord, int>();
    foreach (var v in vents)
    {
      //v.Print();
      //Console.WriteLine("Range:");
      foreach (var c in Range(v))
      {
        //Console.WriteLine($"{c.X},{c.Y}");
        if (hits.ContainsKey(c))
        {
          hits[c]++;
        }
        else
        {
          hits[c] = 1;
        }
      }
    }
    //PrintHits(hits);
    Console.WriteLine(hits.Values.Where(v => v > 1).Count());
  }
  public void PrintHits(Dictionary<Coord, int> hits)
  {
    for (int r = 0; r < 10; r++)
    {
      for (int c = 0; c < 10; c++)
      {
        var coord = new Coord(c, r);
        if (hits.ContainsKey(coord))
        {
          Console.Write(hits[coord]);
        }
        else
        {
          Console.Write('.');
        }
      }

      Console.WriteLine();
    }
  }
  public IEnumerable<Coord> Range(Vent v)
  {
    List<Coord> line = new List<Coord>();
    int xStep = 0;
    int yStep = 0;
    if (v.Start.X > v.End.X)
      xStep = -1;
    else if (v.Start.X < v.End.X)
      xStep = 1;
    else
      xStep = 0;
    if (v.Start.Y > v.End.Y)
      yStep = -1;
    else if (v.Start.Y < v.End.Y)
      yStep = 1;
    else
      yStep = 0;
    Coord cur = v.Start;
    while (!cur.Equals(v.End))
    {
      line.Add(cur);
      cur = new Coord(cur.X + xStep, cur.Y + yStep);
    }
    line.Add(cur);
    return line;
  }

  public IEnumerable<Vent> ParseRows(string input)
  {
    List<Vent> vents = new List<Vent>();
    using (StringReader sr = new StringReader(input))
    {
      string? line;
      while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
      {

        Match match = Regex.Match(line, @"(\d+,\d+)\s+\-\>\s+(\d+,\d+)");
        if (!match.Success)
          throw new Exception("bad input");
        var start = match.Groups[1].Value.Split(',').Select(s => int.Parse(s)).ToArray();
        var end = match.Groups[2].Value.Split(',').Select(s => int.Parse(s)).ToArray();
        vents.Add(new Vent(new Coord(start[0], start[1]), new Coord(end[0], end[1])));
      }
    }
    return vents;
  }
}