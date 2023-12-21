using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day20 : IDay
{
  public class MachineOn : Exception
  {
    public string Module { get; set; }
    public MachineOn(string m)
    {
      Module = m;
    }

  }
  public interface Module
  {
    public string Name { get; set; }
    public IEnumerable<string> Inputs { get; set; }
    public int LowPulses { get; set; }
    public int HighPulses { get; set; }
    public void Pulse(string input, bool s);
    public void ProcessPulse(Dictionary<string, Module> modules, Dictionary<string, long> test, bool debug = false);
    public Queue<(string input, bool s)> PulseQueue { get; set; }
    public IEnumerable<string> Destinations { get; set; }
  }
  public class Counter : Module
  {
    public override string ToString()
    {
      return $"Ctr {Name} inputs:{string.Join(',', Inputs)}";
    }
    public string Name { get; set; }
    public IEnumerable<string> Inputs { get; set; }
    public int LowPulses { get; set; }
    public int HighPulses { get; set; }
    public IEnumerable<string> Destinations { get; set; }
    public void Pulse(string input, bool s)
    {
      PulseQueue.Enqueue((input, s));
    }
    public virtual void ProcessPulse(Dictionary<string, Module> modules, Dictionary<string, long> test, bool debug = false)
    {
      (string input, bool s) = PulseQueue.Dequeue();
      if (debug)
        Console.WriteLine($"{input} -{(s ? 'h' : 'l')}-> {Name}");
      if (s)
        HighPulses++;
      else
        LowPulses++;
      if (test.ContainsKey(input))
        if (s)
          throw new MachineOn(input);
    }
    public Queue<(string input, bool s)> PulseQueue { get; set; }
    public Counter()
    {
      Name = "";
      PulseQueue = new Queue<(string input, bool s)>();
      Destinations = new List<string>();
      Inputs = new List<string>();
    }

  }
  public class FlipFlop : Counter
  {
    public override string ToString()
    {
      return $"FF {Name} {string.Join(',', Destinations)} inputs:{string.Join(',', Inputs)}";
    }
    public FlipFlop() : base()
    {
    }
    public bool State { get; private set; }
    public override void ProcessPulse(Dictionary<string, Module> modules, Dictionary<string, long> test, bool debug = false)
    {
      (string input, bool s) = PulseQueue.Dequeue();
      if (debug)
        Console.WriteLine($"{input} -{(s ? 'h' : 'l')}-> {Name}");
      if (s)
        HighPulses++;
      else
        LowPulses++;
      if (!s)
      {
        State = !State;
        foreach (string d in Destinations)
        {
          modules[d].Pulse(Name, State);
        }
      }
      if (test.ContainsKey(input))
        if (s)
          throw new MachineOn(input);
    }
  }
  public class Conjunction : Counter
  {
    public override string ToString()
    {
      return $"Con {Name} {string.Join(',', Destinations)} inputs:{string.Join(',', Inputs)}";
    }
    public Dictionary<string, bool> State { get; }
    public Conjunction() : base()
    {
      State = new Dictionary<string, bool>();
    }
    public override void ProcessPulse(Dictionary<string, Module> modules, Dictionary<string, long> test, bool debug = false)
    {
      (string input, bool s) = PulseQueue.Dequeue();
      if (debug)
        Console.WriteLine($"{input} -{(s ? 'h' : 'l')}-> {Name}");
      if (s)
        HighPulses++;
      else
        LowPulses++;
      State[input] = s;
      bool send = !State.Values.All(v => v);
      foreach (string d in Destinations)
      {
        modules[d].Pulse(Name, send);
      }
      if (test.ContainsKey(input))
        if (s)
          throw new MachineOn(input);
    }
  }
  public class Broadcaster : Counter
  {
    public override string ToString()
    {
      return $"{Name} {string.Join(',', Destinations)} inputs:{string.Join(',', Inputs)}";
    }
    public Broadcaster() : base()
    {
    }
    public override void ProcessPulse(Dictionary<string, Module> modules, Dictionary<string, long> test, bool debug = false)
    {
      (string input, bool s) = PulseQueue.Dequeue();
      if (debug)
        Console.WriteLine($"{input} -{(s ? 'h' : 'l')}-> {Name}");
      if (s)
        HighPulses++;
      else
        LowPulses++;
      foreach (string d in Destinations)
      {
        modules[d].Pulse(Name, s);
      }
      if (test.ContainsKey(input))
        if (s)
          throw new MachineOn(input);
    }
  }

  public void Run(string input)
  {
    Dictionary<string, Module> modules = ParseRows(input);
    Dictionary<string, long> test = new Dictionary<string, long>();
    foreach (string m in modules["rx"].Inputs)
    {
      foreach (string d in modules[m].Inputs)
      {
        test.Add(d, 0);
      }
    }


    long buttons = 0;
    try
    {
      while (true)
      {
        buttons++;
        modules["broadcaster"].Pulse("button", false);
        while (modules.Values.Any(v => v.PulseQueue.Count > 0))
        {
          var process = modules.Values.Where(v => v.PulseQueue.Count > 0).ToList();
          foreach (var m in process)
          {
            try
            {
              m.ProcessPulse(modules, test, false);
            }
            catch (MachineOn mo)
            {
              test[mo.Module] = buttons;
              if (test.Values.All(v => v != 0))
                throw new MachineOn("");
            }
          }
        }
      }
    }
    catch (MachineOn mo)
    {
      //just getting out of the loop
    }
    Console.WriteLine(string.Join(',', test.Select(t => $"{t.Key}:{t.Value}")));
    Console.WriteLine(LCM(test.Values.ToArray()));
    long low = modules.Values.Select(v => (long)v.LowPulses).Sum();
    long high = modules.Values.Select(v => (long)v.HighPulses).Sum();
    Console.WriteLine($"{low} {high} {low * high}");
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

  public Dictionary<string, Module> ParseRows(string input)
  {
    Dictionary<string, Module> modules = new Dictionary<string, Module>();
    using (StringReader sr = new StringReader(input))
    {
      string? line;
      while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
      {
        var split = line.Split("->");
        string name = split[0].Trim();
        string output = split[1].Trim();
        if (name == "broadcaster")
        {
          modules[name] = new Broadcaster() { Name = name, Destinations = output.Split(',').Select(s => s.Trim()) };
        }
        if (name.StartsWith("%"))
        {
          name = name.Substring(1);
          modules[name] = new FlipFlop() { Name = name, Destinations = output.Split(',').Select(s => s.Trim()) };
        }
        if (name.StartsWith("&"))
        {
          name = name.Substring(1);
          modules[name] = new Conjunction() { Name = name, Destinations = output.Split(',').Select(s => s.Trim()) };
        }
      }
    }
    foreach (var m in modules.Values.ToList())
    {
      foreach (string d in m.Destinations)
      {
        if (modules.ContainsKey(d) && modules[d] is Conjunction)
        {
          ((Conjunction)modules[d]).State[m.Name] = false;
        }
        if (!modules.ContainsKey(d))
          modules[d] = new Counter() { Name = d };
        modules[d].Inputs = modules[d].Inputs.Append(m.Name);
      }
    }
    return modules;
  }
}