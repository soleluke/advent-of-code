using System.Text.RegularExpressions;

public class Day08 : IDay
{
  public class Directions
  {
    public char[] LR { get; set; }
    public Dictionary<string, Tuple<string, string>> Nodes { get; set; }
    public override string ToString()
    {
      return $"{new string(LR)}";
    }
    public Directions()
    {
      LR = new char[0];
      Nodes = new Dictionary<string, Tuple<string, string>>();
    }
  }
  public void Run(string input)
  {
    Directions dirs = ParseDirections(input);
    //Console.WriteLine(dirs.ToString());
    Console.WriteLine(NavigateGhost(dirs));
  }
  public long NavigateGhost(Directions dirs)
  {
    string[] nodes = dirs.Nodes.Keys.Where(k => k.EndsWith('A')).ToArray();
    Task<long>[] tasks = nodes.Select(n => Navigate(n, dirs)).ToArray();
    return LCM(Task.WhenAll(tasks).Result);
  }
  public long LCM(long[] stuff)
  {
    return stuff.Aggregate((a, i) => (a / GCF(a, i)) * i);
  }
  public long GCF(long a, long b)
  {
    while (b != 0)
    {
      long temp = b;
      b = a % b;
      a = temp;
    }
    return a;
  }
  public Task<long> Navigate(string startNode, Directions dirs)
  {
    return Task.Run(() =>
    {
      long steps = 0;
      string node = startNode;
      int curLR = 0;
      while (!node.EndsWith('Z'))
      {
        char dir = dirs.LR[curLR];
        var n = dirs.Nodes[node];
        //Console.WriteLine($"{dir} {n.Item1} {n.Item2}");
        switch (dir)
        {
          case 'L':
            node = n.Item1;
            break;
          case 'R':
            node = n.Item2;
            break;
        }
        steps++;
        curLR++;
        curLR = curLR % (dirs.LR.Length);
      }
      return steps;
    });
  }
  private Directions ParseDirections(string input)
  {
    Directions dirs = new Directions();
    using (StringReader reader = new StringReader(input))
    {
      string? line;
      line = reader.ReadLine();
      if (string.IsNullOrEmpty(line))
        throw new Exception("bad input");
      Match dirsMatch = Regex.Match(line, @"[LR]+\s*");
      dirs.LR = dirsMatch.Value.ToCharArray();
      while ((line = reader.ReadLine()) != null)
      {
        if (!string.IsNullOrEmpty(line))
        {
          var node = ParseNode(line);
          dirs.Nodes.Add(node.Key, node.Value);
        }
      }
    }
    return dirs;
  }
  private KeyValuePair<string, Tuple<string, string>> ParseNode(string line)
  {
    Match match = Regex.Match(line, @"([A-Z1-9]+)\s+\=\s+\(([A-Z1-9]+),\s*([A-Z1-9]+)\)");
    if (!match.Success)
      throw new Exception("bad line: " + line);
    string node = match.Groups[1].Value;
    string left = match.Groups[2].Value;
    string right = match.Groups[3].Value;
    return new KeyValuePair<string, Tuple<string, string>>(node,
      new Tuple<string, string>(left, right));
  }
}