using System.Text.RegularExpressions;
using System.Collections.Immutable;

public class Day12 : IDay
{
  private static Dictionary<Tuple<string, int>, long> memos = new Dictionary<Tuple<string, int>, long>();
  public class Row
  {
    public string Springs { get; set; }
    public ImmutableList<int> Spec { get; set; }
    public Row()
    {
      Springs = "";
      Spec = ImmutableList.Create<int>();
    }
    public override string ToString()
    {
      return $"{new string(Springs)} {string.Join(',', Spec)}";
    }
  }
  public void Run(string input)
  {
    List<Row> rows = ParseRows(input).ToList();
    Task.WhenAll(rows.Select(r => Task.Run(() => UnfoldRow(r)))).Wait();
    IEnumerable<long> arrangements = rows.Select((r, i) => GetArrangements(r, i).Result);
    long sum = arrangements.Sum();
    Console.WriteLine(sum);
  }
  public void UnfoldRow(Row row)
  {
    int foldNum = 5;
    string start = "?" + row.Springs;
    List<int> spec = row.Spec.ToList();
    for (int i = 0; i < foldNum - 1; i++)
    {
      row.Springs = row.Springs + start;
      row.Spec = ImmutableList.CreateRange(row.Spec.Concat(spec));
    }
  }
  public Task<long> GetArrangements(Row row, int i)
  {
    return Task.Run(() =>
    {
      long arrangements = Arrangements(row.Spec, row.Springs, memos);
      return arrangements;
    });
  }

  public long Arrangements(ImmutableList<int> groups, string window, Dictionary<Tuple<string, int>, long> memos)
  {
    void saveMemo(string window, ImmutableList<int> groups, long value)
    {

      Tuple<string, int> tuple = new Tuple<string, int>(window, hashGroup(groups));
      memos[tuple] = value;
    }
    int hashGroup(ImmutableList<int> groups)
    {
      int hc = groups.Count();
      foreach (int val in groups)
      {
        hc = unchecked(hc * 314159 + val);
      }
      return hc;
    }
    long? getMemo(string window, ImmutableList<int> groups)
    {
      Tuple<string, int> tuple = new Tuple<string, int>(window, hashGroup(groups));
      if (memos.ContainsKey(tuple))
        return memos[tuple];
      return null;
    }
    if (window.Length == 0)
      if (groups.Any())
        return 0;
      else
        return 1;
    if (window[0] == '.')
      return Arrangements(groups, window.Substring(1), memos);
    if (window[0] == '?')
    {
      string brokenWindow = '#' + window.Substring(1);
      string workingWindow = '.' + window.Substring(1);
      long? broken = getMemo(brokenWindow, groups);
      long? working = getMemo(workingWindow, groups);
      if (broken == null)
      {
        broken = Arrangements(groups, brokenWindow, memos);
        saveMemo(brokenWindow, groups, broken.Value);
      }
      if (working == null)
      {
        working = Arrangements(groups, workingWindow, memos);
        saveMemo(workingWindow, groups, working.Value);
      }

      saveMemo(window, groups, broken.Value + working.Value);
      return broken.Value + working.Value;
    }
    else if (window[0] == '#')
    {
      if (!groups.Any())
        return 0;
      int offset = 0;
      while (offset < window.Length && Possible(window[offset]) && offset < groups.First())
      {
        offset++;
      }
      if (offset < groups.First())
      {
        saveMemo(window, groups, 0);
        return 0;
      }
      if (offset == window.Length)
      {
        ImmutableList<int> g = ImmutableList.CreateRange(groups.Skip(1));
        long? a = getMemo(window.Substring(offset), g);
        if (a.HasValue)
          return a.Value;
        a = Arrangements(g, window.Substring(offset), memos);
        saveMemo(window.Substring(offset), g, a.Value);
        return a.Value;
      }
      if (window[offset] == '#')
      {
        saveMemo(window, groups, 0);
        return 0;
      }
      ImmutableList<int> g2 = ImmutableList.CreateRange(groups.Skip(1));
      long? arr = getMemo(window.Substring(offset + 1), g2);
      if (arr.HasValue)
        return arr.Value;
      arr = Arrangements(g2, window.Substring(offset + 1), memos);
      saveMemo(window.Substring(offset + 1), g2, arr.Value);
      return arr.Value;
    }
    saveMemo(window, groups, 0);
    return 0;
  }
  public bool Possible(char c)
  {
    return c == '?' || c == '#';
  }
  public int GroupOptions(int groupSize, int start, char[] row)
  {
    //Console.WriteLine($"{groupSize} {start} {new string(row)}");
    if (start + groupSize >= row.Length)
    {
      return 0;
    }
    if (row.Take(groupSize).All(Possible))
    {
      if (row[start + groupSize] == '#')
        return 1;
      if (row[start] == '?')
        return 1 + GroupOptions(groupSize, start + 1, row);
    }
    return 0;
  }
  public IEnumerable<Row> ParseRows(string input)
  {
    List<Row> rows = new List<Row>();
    string? line;
    using (StringReader sr = new StringReader(input))
    {
      while ((line = sr.ReadLine()) != null)
      {
        rows.Add(ParseRow(line));
      }
    }
    return rows;
  }
  public Row ParseRow(string input)
  {
    Match match = Regex.Match(input, @"([#.\?]+)\s+((\d+,*)+)");
    if (!match.Success)
      throw new Exception("bad input");
    string rowData = match.Groups[1].Value;
    string specString = match.Groups[2].Value;
    IEnumerable<int> spec = specString.Split(',').Select(s => int.Parse(s));
    return new Row()
    {
      Springs = rowData,
      Spec = ImmutableList.CreateRange(spec)
    };
  }
}