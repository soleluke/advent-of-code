using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day19 : IDay
{
  public class Part
  {
    public int X { get; set; }
    public int M { get; set; }
    public int A { get; set; }
    public int S { get; set; }
  }
  public class Compare
  {
    public int Val { get; set; }
    public string Op { get; set; }
    public string Next { get; set; }
    public Compare()
    {
      Op = "";
      Next = "";
    }
    public override string ToString()
    {
      return $"{Op} {Val} {Next}";
    }
  }
  public class Range
  {
    public int Min { get; }
    public int Max { get; }
    public Range()
    {
      Min = 1;
      Max = 4000;
    }
    public Range(int min, int max)
    {
      Min = min;
      Max = max;
    }
    public long Possible()
    {
      return (long)(Max - Min) + 1;
    }
    public override string ToString()
    {
      return $"{Min}-{Max}";
    }
    public (Range? a, Range? r) Split(Compare c)
    {
      if (c.Val < Min)
        return (null, this);
      if (c.Val > Max)
        return (this, null);
      if (c.Op == "<")
        return (new Range(Min, c.Val - 1), new Range(c.Val, Max));
      else
        return (new Range(c.Val + 1, Max), new Range(Min, c.Val));
    }
  }
  public class Constraints
  {
    public Dictionary<string, Range> Ranges { get; }
    public Constraints()
    {
      Ranges = new Dictionary<string, Range>(){
        {"X",new Range()},
        { "M",new Range()},
        { "A",new Range()},
        { "S",new Range()}
      };
    }
    public Range R(string key)
    {
      return Ranges[key];
    }
    public Constraints(Dictionary<string, Range> dict)
    {
      Ranges = dict;
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      foreach ((var k, var v) in Ranges)
      {
        sb.Append($"{k} {v?.ToString()} ");
      }
      return sb.ToString();
    }
    public long Possible()
    {
      long p = 1;
      foreach ((var k, var v) in Ranges)
      {
        if (v != null)
          p *= v.Possible();
      }
      return p;
    }
    public (Constraints?, Constraints?) Split(Compare c, string field)
    {
      Constraints? approved = null;
      Constraints? rejected = null;
      var split = R(field).Split(c);


      if (split.a != null)
      {
        var aDict = Ranges.ToDictionary(e => e.Key, e => e.Value);
        aDict[field] = split.a;
        approved = new Constraints(aDict);
      }
      if (split.r != null)
      {
        var rDict = Ranges.ToDictionary(e => e.Key, e => e.Value);
        rDict[field] = split.r;
        rejected = new Constraints(rDict);
      }
      return (approved, rejected);
    }
  }
  public class Workflow
  {
    public List<(string, Compare)> Rules { get; }
    public string Default { get; set; }
    public Workflow()
    {
      Rules = new List<(string, Compare)>();
      Default = "";
    }
    public List<(string, Constraints)> GetSplits(Constraints con)
    {
      List<(string, Constraints)> cs = new List<(string, Constraints)>();
      foreach ((var k, var comp) in Rules)
      {
        var xSplit = con.Split(comp, k);
        if (xSplit.Item1 != null)
          cs.Add((comp.Next, xSplit.Item1));
        if (xSplit.Item2 != null)
          con = xSplit.Item2;
      }
      cs.Add((this.Default, con));
      return cs;
    }
  }
  public void Run(string input)
  {
    (var parts, var workflow) = ParseRows(input);
    Constraints xmas = new Constraints();
    long p = Possibilities(xmas, "in", workflow);
    long p2 = PossQueue(xmas, "in", workflow);
    Console.WriteLine(p);
    //Console.WriteLine(string.Join(',', workflow.Keys));
    //IEnumerable<Task<int>> work = parts.Select(p => Work(p, workflow));
    //IEnumerable<int> results = Task.WhenAll(work).Result;
    //Console.WriteLine(results.Sum());
  }
  public long PossQueue(Constraints xmas, string start, Dictionary<string, Workflow> workflow)
  {
    long accepted = 0;
    var queue = new Queue<(string, Constraints)>();
    queue.Enqueue((start, xmas));
    List<Constraints?> ranges = new List<Constraints?>();
    while (queue.Count > 0)
    {
      (var key, var con) = queue.Dequeue();
      var flow = workflow[key];

      foreach ((var next, var c) in flow.GetSplits(con))
      {
        switch (next)
        {
          case "A":
            accepted += c.Possible();
            ranges.Add(c);
            break;
          case "R":
            break;
          default:
            queue.Enqueue((next, c));
            break; ;
        }
      }
    }
    return ranges.Select(r => r?.Possible() ?? 0).Sum();
  }
  public long Possibilities(Constraints xmas, string current, Dictionary<string, Workflow> workflow)
  {
    string test = "";
    var f = new Constraints(xmas.Ranges.ToDictionary(e => e.Key, e => e.Value));
    if (current == test)
      Console.WriteLine($"starting rule {current} {f}");
    if (current == "A")
    {
      return f.Possible();
    }
    if (current == "R")
      return 0L;
    long p = 0L;
    Workflow w = workflow[current];
    foreach ((var k, var v) in w.Rules)
    {
      Range? range = f.Ranges[k];
      if (range != null)
      {
        (var app, var rej) = f.Split(v, k);
        if (app != null)
          p += Possibilities(app, v.Next, workflow);
        if (rej != null)
          f = rej;
      }
    }
    p += Possibilities(f, w.Default, workflow);
    return p;
  }
  public Task<int> Work(Part p, Dictionary<string, Func<Part, string>> workflow)
  {
    return Task.Run(() =>
    {
      string next = "in";
      while (next != "A" && next != "R")
      {
        next = workflow[next](p);
      }
      if (next == "A")
      {
        return p.X + p.M + p.A + p.S;
      }
      return 0;
    });
  }

  public Func<Part, string> GetCheck(Func<Part, int> field, Func<int, int, bool> check, int toCheck, string val, Func<Part, string> then)
  {
    return p => check(field(p), toCheck) ? val : then(p);
  }
  public (IEnumerable<Part> parts, Dictionary<string, Workflow> workflow) ParseRows(string input)
  {
    List<Part> parts = new List<Part>();

    //Dictionary<string, Func<Part, string>> workflow = new Dictionary<string, Func<Part, string>>();
    Dictionary<string, Workflow> workflow = new Dictionary<string, Workflow>();
    using (StringReader sr = new StringReader(input))
    {
      string? line;
      while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
      {
        Match match = Regex.Match(line, @"(\w+)\{(.+)\}");
        string key = match.Groups[1].Value;
        string ruleString = match.Groups[2].Value;
        IEnumerable<string> rules = ruleString.Split(',').Reverse();
        string def = rules.First();
        Func<Part, string> check = p => def;
        Workflow w = new Workflow()
        {
          Default = def
        };
        foreach (string rule in rules.Skip(1).Reverse())
        {
          Match m = Regex.Match(rule, @"([xmas])([<>])(\d+)\:(\w+)");
          Func<Part, int> field;
          switch (m.Groups[1].Value)
          {
            case "x":
              field = p => p.X;
              break;
            case "m":
              field = p => p.M;
              break;
            case "a":
              field = p => p.A;
              break;
            case "s":
              field = p => p.S;
              break;
            default:
              throw new Exception("bad field:" + m.Groups[1].Value);
          }
          string op = m.Groups[2].Value;
          Func<int, int, bool> compare;
          switch (op)
          {
            case ">":
              compare = (a, b) => a > b;
              break;
            case "<":
              compare = (a, b) => a < b;
              break;
            default:
              throw new Exception("bad op:" + op);
          }
          int toCheck = int.Parse(m.Groups[3].Value);
          string val = m.Groups[4].Value;
          check = GetCheck(field, compare, toCheck, val, check);
          switch (m.Groups[1].Value)
          {
            case "x":
              w.Rules.Add(("X", new Compare() { Val = toCheck, Op = op, Next = val }));
              break;
            case "m":
              w.Rules.Add(("M", new Compare() { Val = toCheck, Op = op, Next = val }));
              break;
            case "a":
              w.Rules.Add(("A", new Compare() { Val = toCheck, Op = op, Next = val }));
              break;
            case "s":
              w.Rules.Add(("S", new Compare() { Val = toCheck, Op = op, Next = val }));
              break;
            default:
              throw new Exception("bad field:" + m.Groups[1].Value);
          }
        }
        workflow[key] = w;
      }
      while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
      {
        line = line.Replace("{", "");
        line = line.Replace("}", "");
        Part p = new Part();
        foreach (var s in line.Split(','))
        {
          if (s.StartsWith("x="))
            p.X = int.Parse(s.Substring(2));
          if (s.StartsWith("m="))
            p.M = int.Parse(s.Substring(2));
          if (s.StartsWith("a="))
            p.A = int.Parse(s.Substring(2));
          if (s.StartsWith("s="))
            p.S = int.Parse(s.Substring(2));
        }
        parts.Add(p);
      }

    }
    return (parts, workflow);
  }
}