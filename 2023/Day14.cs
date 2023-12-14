using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day14 : IDay
{
  public List<char[][]> patterns = new List<char[][]>();
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
    char[][] platform = ParseRows(input);
    ImmutableArray<char[]> original = ImmutableArray.Create(platform);
    int cycles = 1000000000;
    int repeat = 0;
    int cycleStart = 0;
    for (int i = 0; i < cycles; i++)
    {
      Console.WriteLine($"loads {i}: {LoadSum(platform)}");
      char[][]? p = patterns.FirstOrDefault(p => Compare(p, platform));
      if (p == null)
      {
        patterns.Add(platform);
        platform = Spin(platform.ToArray());
      }
      else
      {
        cycleStart = patterns.IndexOf(p);
        Console.WriteLine($"cycleStart: {cycleStart}");
        repeat = i - cycleStart;
        break;
      }
    }
    Console.WriteLine($"repeats: {repeat}");
    platform = original.ToArray();
    for (int i = 0; i < repeat + cycleStart; i++)
    {
      Console.WriteLine($"loads {i}: {LoadSum(platform)}");
      platform = Spin(platform.ToArray());
    }
    int extra = (cycles - cycleStart) % repeat;
    Console.WriteLine($"extra: {extra}");
    platform = PatternAt(cycleStart, original.ToArray()).Result;
    platform = PatternAt(extra, platform).Result;

    //PrintPattern(platform);
    int sum = LoadSum(platform);
    Console.WriteLine(sum);

    //104395 too low
  }
  public int LoadSum(char[][] platform)
  {
    int[,] loads = Loads(platform);
    int sum = 0;
    foreach (var thing in loads)
    {
      sum += thing;
    }
    return sum;
  }
  public (int, int) BrentCycle(int start, char[][] pattern)
  {
    int power = 1;
    int lambda = 1;
    int tIndex = 0;
    int hIndex = 1;
    char[][] tortoise = pattern.ToArray();
    char[][] hare = PatternAt(hIndex, pattern).Result;
    while (GetKey(tortoise) != GetKey(hare))
    {
      if (power == lambda)
      {
        tortoise = hare;
        tIndex = hIndex;
        power *= 2;
        lambda = 0;
      }

      hare = Spin(hare);
      lambda++;
    }
    tortoise = pattern.ToArray();
    tIndex = 0;
    hare = PatternAt(lambda, pattern).Result;
    hIndex = lambda;
    int mu = 0;
    while (GetKey(tortoise) != GetKey(hare))
    {
      tortoise = Spin(tortoise);
      hare = Spin(hare);
      mu++;
    }
    return (lambda, mu);
  }
  public Task<char[][]> PatternAt(int cycles, char[][] platform)
  {
    return Task.Run(() =>
    {
      for (int i = 0; i < cycles; i++)
      {
        platform = Spin(platform);
      }
      return platform;
    });
  }
  public int[,] Loads(char[][] pattern)
  {
    int[,] loads = new int[pattern.Length, pattern[0].Length];
    for (int r = 0; r < pattern.Length; r++)
    {
      for (int c = 0; c < pattern[r].Length; c++)
      {
        char thing = pattern[r][c];
        switch (thing)
        {
          case '.':
          case '#':
            break;
          case 'O':
            loads[r, c] = pattern.Length - r;
            break;
        }
      }
    }
    return loads;
  }
  private string GetKey(char[][] pattern)
  {
    StringBuilder sb = new StringBuilder();
    foreach (string s in pattern.Select(p => new string(p)))
    {
      sb.Append(s);
    }
    return sb.ToString();
  }
  public char[][] Spin(char[][] pattern)
  {
    pattern = ParallelTilt(pattern);
    pattern = Rotate(pattern);
    pattern = ParallelTilt(pattern);
    pattern = Rotate(pattern);
    pattern = ParallelTilt(pattern);
    pattern = Rotate(pattern);
    pattern = ParallelTilt(pattern);
    pattern = Rotate(pattern);
    return pattern;
  }
  public char[][] TiltNorth(char[][] pattern)
  {
    int[] lowestPoints = new int[pattern[0].Length];

    for (int r = 0; r < pattern.Length; r++)
    {
      for (int c = 0; c < pattern[r].Length; c++)
      {
        char thing = pattern[r][c];
        switch (thing)
        {
          case 'O':
            pattern[r][c] = '.';
            pattern[lowestPoints[c]][c] = 'O';
            lowestPoints[c] = lowestPoints[c] + 1;
            break;
          case '#':
            lowestPoints[c] = r + 1;
            break;
          case '.':
            break;
        }
        //Console.WriteLine(string.Join(',', lowestPoints));
        //PrintPattern(pattern);
      }
    }
    return pattern;
  }
  public char[][] ParallelTilt(char[][] pattern)
  {
    List<Task<char[]>> tasks = new List<Task<char[]>>();
    for (int c = 0; c < pattern[0].Length; c++)
    {
      tasks.Add(TiltCol(pattern.Select(r => r[c]).ToArray()));
    }
    char[][] result = Task.WhenAll(tasks).Result;
    List<char[]> newPat = new List<char[]>();
    for (int c = 0; c < pattern[0].Length; c++)
    {
      newPat.Add(result.Select(r => r[c]).ToArray());
    }
    return newPat.ToArray();
  }
  public Task<char[]> TiltCol(char[] col)
  {
    return Task.Run(() =>
    {
      int lowest = 0;
      for (int r = 0; r < col.Length; r++)
      {
        char thing = col[r];
        switch (thing)
        {
          case 'O':
            col[r] = '.';
            col[lowest] = 'O';
            lowest++;
            break;
          case '#':
            lowest = r + 1;
            break;
          case '.':
            break;
        }
      }
      return col;
    });
  }
  public char[][] Rotate(char[][] pattern)
  {
    List<char[]> rotate = new List<char[]>();
    for (int i = 0; i < pattern[0].Length; i++)
    {
      rotate.Add(pattern.Select(p => p[i]).Reverse().ToArray());
    }
    return rotate.ToArray();
  }

  public char[][] ParseRows(string input)
  {
    List<char[]> patterns = new List<char[]>();
    string? line;
    using (StringReader sr = new StringReader(input))
    {
      while ((line = sr.ReadLine()) != null)
      {
        patterns.Add(line.ToCharArray());
      }
    }
    return patterns.ToArray();
  }
}