using System.Text.RegularExpressions;
using System.Collections.Immutable;
using System.Drawing;

public class Day14 : IDay
{
  public void PrintPattern(char[][] pattern)
  {
    Console.WriteLine(string.Join('\n', pattern.Select(c => new string(c))));
  }
  public void Run(string input)
  {
    IEnumerable<char[][]> patterns = ParseRows(input);


  }

  public IEnumerable<char[][]> ParseRows(string input)
  {
    List<char[][]> patterns = new List<char[][]>();
    string? line;
    using (StringReader sr = new StringReader(input))
    {
      while ((line = sr.ReadLine()) != null)
      {

      }
    }
    return patterns;
  }
}