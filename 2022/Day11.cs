using System.Text.RegularExpressions;
using System.Collections.Immutable;
using System.Drawing;

public class Day11 : IDay
{
  public static long MaxTest = 1;
  public class Monkey
  {
    public long Inspections { get; set; }
    public int Id { get; set; }
    public Queue<long> Items { get; set; }
    public Func<long, long> Operation { get; set; }
    public long TestVal { get; set; }
    public int trueTarg { get; set; }
    public int falseTarg { get; set; }
    public Monkey? TrueTarget { get; set; }
    public Monkey? FalseTarget { get; set; }
    public void TestAndThrow(long item)
    {
      item = item % MaxTest;
      bool test = item % TestVal == 0;
      if (test)
        TrueTarget?.Items.Enqueue(item);
      else
        FalseTarget?.Items.Enqueue(item);
    }
    public Monkey()
    {
      TrueTarget = null;
      FalseTarget = null;
      Items = new Queue<long>();
      Operation = (i) => i;
    }
  }
  public void PrintPattern(char[][] pattern)
  {
    Console.WriteLine(string.Join('\n', pattern.Select(c => new string(c))));
  }
  public void Run(string input)
  {
    IEnumerable<Monkey> monkeys = ParseMonkeys(input);
    ArrangeMonkeys(monkeys);
    MaxTest = monkeys.Select(m => m.TestVal).Aggregate((a, i) => a *= i);
    for (int i = 0; i < 10000; i++)
    {
      foreach (Monkey monkey in monkeys)
      {
        DoRound(monkey, monkeys);
      }
      //Console.WriteLine(string.Join('\n', monkeys.Select(m => $"{m.Id} {m.Inspections} {string.Join(',', m.Items)}")));
    }
    foreach (Monkey m in monkeys)
    {
      Console.WriteLine($"{m.Id} {m.Inspections}");
    }
    long business = monkeys.OrderByDescending(m => m.Inspections).Take(2).Select(m => m.Inspections).Aggregate((a, i) => a * i);
    Console.WriteLine($"business {business}");
  }
  public void ArrangeMonkeys(IEnumerable<Monkey> monkeys)
  {
    foreach (Monkey monkey in monkeys)
    {
      monkey.TrueTarget = monkeys.First(m => m.Id == monkey.trueTarg);
      monkey.FalseTarget = monkeys.First(m => m.Id == monkey.falseTarg);
    }
  }
  public void DoRound(Monkey monkey, IEnumerable<Monkey> monkeys)
  {
    while (monkey.Items.Any())
    {
      long item = monkey.Items.Dequeue();
      //Console.WriteLine($"Inspect {item}");
      item = monkey.Operation(item);
      //Console.WriteLine($"operate {item}");
      //Console.WriteLine($"{monkey.Test(item)}");
      //item = ((int)Math.Floor((double)item / 3));
      if (item < 0)
      {
        throw new Exception($"{item} overflowed");
      }
      monkey.TestAndThrow(item);
      monkey.Inspections++;
    }
  }
  public Monkey ParseMonkey(StringReader sr)
  {
    Monkey monkey = new Monkey();
    Match m = MatchLine(sr, @"Monkey\s(\d+)\:");
    monkey.Id = int.Parse(m.Groups[1].Value);
    m = MatchLine(sr, @"Starting\s+items:\s+((\d+,*\s*)+)");
    IEnumerable<long> items = m.Groups[1].Value.Split(',').Select(s => long.Parse(s));
    monkey.Items = new Queue<long>(items);
    m = MatchLine(sr, @"Operation\: new\s+\=\s+old\s+([+*])\s+(\d+|old)");
    string op = m.Groups[1].Value;
    string val = m.Groups[2].Value;
    monkey.Operation = GetOperation(op, val);
    m = MatchLine(sr, @"Test\:\s+divisible\s+by\s+(\d+)");
    monkey.TestVal = long.Parse(m.Groups[1].Value);
    m = MatchLine(sr, @"If true\: throw to monkey (\d+)");
    int trueId = int.Parse(m.Groups[1].Value);
    m = MatchLine(sr, @"If false\: throw to monkey (\d+)");
    int falseId = int.Parse(m.Groups[1].Value);
    monkey.falseTarg = falseId;
    monkey.trueTarg = trueId;
    return monkey;
  }
  public Func<long, long> GetOperation(string op, string val)
  {
    if (val == "old")
      switch (op)
      {
        case "+":
          return (i) => i + i;
        case "*":
          return (i) => i * i;
      }
    int value = int.Parse(val);
    switch (op)
    {
      case "+":
        return (i) => i + value;
      case "*":
        return (i) => i * value;
    }
    throw new Exception("bad op:" + op);
  }
  public Match MatchLine(StringReader sr, string pattern)
  {
    string? line = sr.ReadLine();
    if (line == null)
      throw new Exception("bad monkey");
    Match m = Regex.Match(line, pattern);
    if (!m.Success)
      throw new Exception("bad monkey:" + line);
    return m;
  }

  public IEnumerable<Monkey> ParseMonkeys(string input)
  {
    List<Monkey> m = new List<Monkey>();
    using (StringReader sr = new StringReader(input))
    {
      do
      {
        m.Add(ParseMonkey(sr));
        if (sr.Peek() == '\n')
          sr.ReadLine();
      }
      while (sr.Peek() != -1);
    }
    return m;
  }
}