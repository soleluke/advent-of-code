using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day02 : IDay
{
  public class Step
  {
    public char Dir { get; set; }
    public int Dist { get; set; }
  }

  public void Run(string input)
  {
    var position = (h: 0, v: 0, a: 0);
    var steps = ParseRows(input);
    foreach (Step s in steps)
    {
      position = Move(position, s);
    }
    Console.WriteLine(position.h * position.v);

  }
  public (int h, int v, int a) Move((int h, int v, int a) pos, Step s)
  {
    switch (s.Dir)
    {
      case 'f':
        return (pos.h + s.Dist, pos.v + (pos.a * s.Dist), pos.a);
      case 'd':
        return (pos.h, pos.v, pos.a + s.Dist);
      case 'u':
        return (pos.h, pos.v, pos.a - s.Dist);
    }
    throw new Exception("bad direction");
  }

  public Step[] ParseRows(string input)
  {
    List<Step> steps = new List<Step>();
    using (StringReader sr = new StringReader(input))
    {
      string? line;
      while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
      {
        string[] split = line.Split(' ');
        steps.Add(new Step
        {
          Dir = split[0][0],
          Dist = int.Parse(split[1])
        });
      }

    }
    return steps.ToArray();
  }
}