using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day18 : IDay
{
  public class Instruction
  {
    public char Direction { get; set; }
    public long Distance { get; set; }
    public string Color { get; set; }
    public Instruction()
    {
      Color = "";
    }
  }
  public int Count(char[,] pit)
  {
    int sum = 0;
    for (int ri = 0; ri < pit.GetLength(0); ri++)
    {
      for (int ci = 0; ci < pit.GetLength(1); ci++)
      {
        if (pit[ri, ci] != '.')
          sum++;
      }
    }
    return sum;
  }
  public void PrintPattern(char[,] pit)
  {
    Console.WriteLine("Pattern----------");
    for (int ri = 0; ri < pit.GetLength(0); ri++)
    {
      for (int ci = 0; ci < pit.GetLength(1); ci++)
      {
        Console.Write(pit[ri, ci]);
      }
      Console.WriteLine();
    }
  }
  public void Print(IEnumerable<Instruction> instructions)
  {
    foreach (var i in instructions)
    {
      Console.WriteLine($"{i.Direction} {i.Distance} {i.Color}");
    }
  }
  public void Run(string input)
  {
    IEnumerable<Instruction> instructions = ParseRows(input);
    (char[,] pit, IEnumerable<(long r, long c)> path, long perimeter) = DigPit(instructions);
    //PrintPattern(pit);
    long shoelace = ShoeLace(path.ToList());
    //Console.WriteLine(shoelace);
    long area = ((Math.Abs(shoelace) + (perimeter)) >> 1) + 1;
    Console.WriteLine(area);
  }
  //shoelace with pick's theorem
  public long ShoeLace(IList<(long r, long c)> path)
  {
    return path.Zip(path.Skip(1)).Select(p =>
    {
      return (long)(p.First.r * p.Second.c - p.Second.r * p.First.c);
    }).Sum() + (long)((path.First().r * path.Last().c - path.Last().r * path.First().c));
  }
  public (char[,] pit, IEnumerable<(long r, long c)> path, long perimeter) DigPit(IEnumerable<Instruction> instructions)
  {
    char[,] pit = new char[1, 1];
    long perimeter = 0;
    IEnumerable<(long r, long c)> path = new List<(long r, long c)>();
    long r = 0;
    long c = 0;
    pit[r, c] = '#';
    foreach (Instruction i in instructions)
    {
      (r, c) = GoDirection(i.Direction, i.Distance, (r, c));
      perimeter += i.Distance;
      /*if (r >= pit.GetLength(0))
        (pit, path) = ExpandPit((r - pit.GetLength(0)) + 1, 0, pit, path);
      if (c >= pit.GetLength(1))
        (pit, path) = ExpandPit(0, (c - pit.GetLength(1)) + 1, pit, path);
      if (r < 0)
      {
        (pit, path) = ExpandPit(-1 * r, 0, pit, path);
        r = 0;
      }
      if (c < 0)
      {
        (pit, path) = ExpandPit(0, -1 * c, pit, path);
        c = 0;
      }
      pit[r, c] = '#';*/
      path = path.Append((r, c));
      //PrintPattern(pit);
    }
    return (pit, path, perimeter);
  }

  public (long, long) GoDirection(char d, long dist, (long r, long c) n)
  {
    (long r, long c) = n;
    switch (d)
    {
      case '2':
      case 'L':
        return (r, c - dist);
      case '0':
      case 'R':
        return (r, c + dist);
      case '3':
      case 'U':
        return (r - dist, c);
      case '1':
      case 'D':
        return (r + dist, c);
    }
    throw new Exception("bad direction");
  }

  public IEnumerable<Instruction> ParseRows(string input)
  {
    IEnumerable<Instruction> patterns = input.Split('\n').Select(l =>
    {
      Match match = Regex.Match(l, @"([RDLU])\s+(\d+)\s+\(#(\S+)\)");
      string color = match.Groups[3].Value;
      char dir = color.ToCharArray().Last();
      long dist = Convert.ToInt64(color.Substring(0, color.Length - 1), 16);
      return new Instruction()
      {
        Direction = dir,
        Distance = dist,
        Color = color

      };
    });
    return patterns;
  }
}