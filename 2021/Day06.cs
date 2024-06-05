using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day06 : IDay
{

  public void Run(string input)
  {
    var fish = ParseRows(input);
    int days = 256;
    long[] state = new long[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    foreach (var f in fish)
    {
      state[f]++;
    }
    for (int d = 0; d < days; d++)
    {
      //Console.WriteLine(string.Join(',', state));
      long[] newState = new long[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
      for (int i = 0; i <= 8; i++)
      {
        if (i == 0)
        {
          newState[6] = state[0];
          newState[8] = state[0];
        }
        else
        {
          newState[i - 1] += state[i];
        }
      }
      state = newState;
    }
    Console.WriteLine(state.Sum());
  }

  public IEnumerable<int> ParseRows(string input)
  {

    using (StringReader sr = new StringReader(input))
    {
      string? line;
      line = sr.ReadLine();
      if (!string.IsNullOrWhiteSpace(line))
      {
        return line.Split(',').Select(s => int.Parse(s));
      }

    }
    throw new Exception("bad input");
  }
}