using System.Text.RegularExpressions;

public class Day9 : IDay
{
  public void Run(string input)
  {
    IEnumerable<IEnumerable<int>> histories = ParseStuff(input);
    IEnumerable<Task<IEnumerable<int>>> tasks = histories.Select(h => Extrapolate(h));
    IEnumerable<IEnumerable<int>> lasts = Task.WhenAll(tasks).Result;
    Console.WriteLine(string.Join('\n', lasts.Select(l => string.Join(',', l))));
    var sums = lasts.Select(r => r.First());
    Console.WriteLine(sums.Aggregate((a, i) => a += i));
  }
  public Task<IEnumerable<int>> Extrapolate(IEnumerable<int> history)
  {
    return Task.Run(() =>
    {
      if (!history.All(h => h == 0))
      {
        var seq = Extrapolate(NextSequence(history)).Result;
        return new List<int>() { history.First() - seq.First() }.Concat(history.Append(history.Last() + seq.Last()));
      }
      return history.Append(0);
    });
  }
  public IEnumerable<int> NextSequence(IEnumerable<int> history)
  {
    return history.Zip(history.Skip(1)).Select(z => z.Second - z.First);
  }
  private IEnumerable<IEnumerable<int>> ParseStuff(string input)
  {
    IEnumerable<IEnumerable<int>> histories = new List<IEnumerable<int>>();
    using (StringReader reader = new StringReader(input))
    {
      string? line;

      while ((line = reader.ReadLine()) != null)
      {
        histories = histories.Append(Regex.Split(line, @"\s+").Select(n => int.Parse(n)));
      }
    }
    return histories;
  }
}