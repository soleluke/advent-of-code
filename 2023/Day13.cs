using System.Text.RegularExpressions;
using System.Collections.Immutable;
using System.Drawing;

public class Day13 : IDay
{
  public void PrintPattern(char[][] pattern)
  {
    Console.WriteLine(string.Join('\n', pattern.Select(c => new string(c))));
  }
  public void Run(string input)
  {
    int sum = getSum(new List<int> { 1, 2, 3, 4, 5 }, new List<int>() { 1, 2, 3 });
    if (sum != 615)
      throw new Exception($"failed test: {sum}");
    IEnumerable<char[][]> patterns = ParseRows(input);
    //Console.WriteLine(string.Join(',', VerticalReflection(patterns.First())));
    List<int> verts = new List<int>();
    List<int> horz = new List<int>();
    if (true)
    {
      foreach (char[][] pattern in patterns)
      {
        string[] p = pattern.Select(p => new string(p)).ToArray();
        //PrintPattern(pattern);
        int? vert = VerticalReflection(pattern);
        if (vert != null)
        {
          verts.Add(vert.Value);
        }
        //Console.WriteLine("horiz");
        int? hor = HorizontalReflection(pattern);
        if (vert == null && hor != null)
        {
          if (vert != null)
          {
            Console.WriteLine(string.Join('\n', p));
            throw new Exception("already found a line for this pattern");
          }
          horz.Add(hor.Value);
        }

      }
    }
    Console.WriteLine(getSum(verts, horz));
  }
  public int getSum(IEnumerable<int> verts, IEnumerable<int> horz)
  {
    int vertSum = 0;
    if (verts.Any())
      vertSum = verts.Sum();
    int horzSum = 0;
    if (horz.Any())
      horzSum = horz.Select(h => h * 100).Sum();
    return vertSum + horzSum;
  }
  public int? HorizontalReflection(char[][] pattern)
  {
    List<char[]> rotate = new List<char[]>();
    for (int i = 0; i < pattern[0].Length; i++)
    {
      rotate.Add(pattern.Select(p => p[i]).ToArray());
    }
    return VerticalReflection(rotate.ToArray());
  }
  public int? VerticalReflection(char[][] pattern)
  {
    List<Tuple<int, Point>> reflections = new List<Tuple<int, Point>>();

    for (int i = 1; i < pattern[0].Length; i++)
    {
      bool isReflected = true;
      int smudges = 0;
      int lastSmudgeRow = -1;
      int lastSmudgeCol = -1;
      foreach ((char[] row, int rowNum) in pattern.Select((p, n) => (p, n)))
      {
        (bool reflect, int? smudge) = IsReflected(row, i);

        isReflected = isReflected && reflect;
        if (smudge != null)
        {
          smudges++;
          lastSmudgeCol = smudge.Value;
          lastSmudgeRow = rowNum;
        }

        if (!isReflected)
          break;
      }
      if (isReflected && smudges == 1)
      {
        //Console.WriteLine($"{i} {lastSmudgeRow} {lastSmudgeCol}");
        reflections.Add(new Tuple<int, Point>(i, new Point(lastSmudgeCol, lastSmudgeRow)));
      }
    }
    if (reflections.Count() == 1)
    {
      var tup = reflections.First();
      Console.WriteLine(tup.Item2);
      pattern[tup.Item2.Y][tup.Item2.X] = FixSmudge(pattern[tup.Item2.Y][tup.Item2.X]);
      return tup.Item1;
    }
    return null;
  }
  public char FixSmudge(char cur)
  {
    switch (cur)
    {
      case '#':
        return '.';
      case '.':
        return '#';
    }
    return ' ';
  }
  public (bool, int?) IsReflected(char[] row, int index)
  {
    char[] start = row.Take(index).ToArray();
    char[] end = row.Skip(index).Take(index).ToArray();
    //Console.WriteLine($"{index} {new string(start)} {new string(end)} ");
    (bool reflect, int? smudge) = CheckEqual(start, end);
    if (smudge != null)
    {
      string oldStart = new string(start);
      start[smudge.Value] = FixSmudge(start[smudge.Value]);
      (bool r, int? s) = CheckEqual(start, end);
      if (!r)
        throw new Exception($"changed {oldStart} to {new string(start)} to match {new string(end)}");
    }
    return (reflect, smudge);
  }
  public (bool, int?) CheckEqual(char[] x, char[] y)
  {
    y = y.Concat(x.Take(x.Length - y.Length).Reverse()).ToArray();
    x = x.Reverse().ToArray();
    bool equal = true;
    int inequal = 0;
    int? smudge = null;
    for (int i = 0; i < x.Length; i++)
    {
      bool equals = (x[i] == y[i]);
      equal = equal && equals;
      if (!equals)
      {
        smudge = x.Length - i;
        inequal++;
      }
    }
    if (inequal > 1)
      smudge = null;
    if (smudge != null)
      return (true, smudge - 1);
    return (equal, smudge);
  }

  public IEnumerable<char[][]> ParseRows(string input)
  {
    List<char[][]> patterns = new List<char[][]>();
    List<char[]> rows = new List<char[]>();
    string? line;
    using (StringReader sr = new StringReader(input))
    {
      while ((line = sr.ReadLine()) != null)
      {
        if (string.IsNullOrEmpty(line))
        {
          patterns.Add(rows.ToArray());
          rows = new List<char[]>();
        }
        else
          rows.Add(line.ToCharArray());
      }
      patterns.Add(rows.ToArray());
    }
    return patterns;
  }
}