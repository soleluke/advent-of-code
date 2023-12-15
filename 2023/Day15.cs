using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day15 : IDay
{
  public class Lens
  {
    public string Label { get; set; }
    public int Length { get; set; }
    public Lens()
    {
      Label = "";
    }
  }
  public static List<List<Lens>> boxes = new List<List<Lens>>();
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
    for (int i = 0; i < 256; i++)
    {
      boxes.Add(new List<Lens>());
    }
    List<string> patterns = ParseRows(input);
    foreach (var p in patterns)
    {
      Process(p);
    }
    int power = 0;
    for (int i = 0; i < boxes.Count(); i++)
    {
      if (boxes[i].Any())
      {
        //Console.WriteLine($"box {i}");
        for (int j = 0; j < boxes[i].Count(); j++)
        {
          Lens lens = boxes[i][j];
          int val = (i + 1) * (j + 1) * lens.Length;
          //Console.Write(val + ",");
          power += val;
        }
        //Console.WriteLine();
      }
    }
    Console.WriteLine($"power {power}");
  }
  public void Process(string val)
  {
    Match match = Regex.Match(val, @"(\w+)([-=])(\d*)");
    string label = match.Groups[1].Value;
    string op = match.Groups[2].Value;
    int? length = null;
    if (!string.IsNullOrEmpty(match.Groups[3].Value))
      length = int.Parse(match.Groups[3].Value);
    int box = Hash(label);
    Lens lens = new Lens()
    {
      Label = label,
      Length = length ?? 0
    };
    switch (op)
    {
      case "=":
        Lens? existing = boxes[box].Find(b => b.Label == lens.Label);
        if (existing != null)
        {
          int index = boxes[box].IndexOf(existing);
          boxes[box][index] = lens;
        }
        else
        {
          boxes[box].Add(lens);
        }
        break;
      case "-":
        Lens? l = boxes[box].Find(len => len.Label == lens.Label);
        if (l != null)
          boxes[box].Remove(l);
        break;
    }
  }

  public int Hash(string val)
  {
    int value = 0;
    foreach (char c in val.ToCharArray())
    {
      value += (int)c; ;
      value *= 17;
      value = value % 256;

    }
    return value;
  }

  public List<string> ParseRows(string input)
  {
    List<string> patterns = input.Split(',').ToList();
    return patterns;
  }
}