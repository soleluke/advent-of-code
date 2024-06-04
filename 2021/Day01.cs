using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day01 : IDay
{

  public void Run(string input)
  {
    var depths = ParseRows(input);
    int increases = 0;
    int cur = 0;
    int windowSize = 3;
    while (cur < depths.Count() - windowSize)
    {
      int windowA = depths.Skip(cur).Take(windowSize).Sum();
      int windowB = depths.Skip(cur + 1).Take(windowSize).Sum();
      if (windowB > windowA)
        increases++;
      cur++;
    }
    Console.WriteLine(increases);
  }

  public int[] ParseRows(string input)
  {
    IEnumerable<int> depths = new List<int>();

    using (StringReader sr = new StringReader(input))
    {
      string? line;
      while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
      {
        depths = depths.Append(int.Parse(line));
      }

    }
    return depths.ToArray();
  }
}