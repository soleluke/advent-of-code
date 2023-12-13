using System.Text.RegularExpressions;
public class Day6 : IDay
{
  public class Race
  {
    public long Time { get; set; }
    public long Distance { get; set; }
    public override string ToString()
    {
      return $"{Time} {Distance}";
    }
  }
  public void Run(string input)
  {
    IEnumerable<Race> races = ParseRaces(input);
    IEnumerable<int> possibilities = races.Select(r => Possibilities(r));
    Console.WriteLine(possibilities.Sum());
  }
  public int Possibilities(Race race)
  {
    List<Task<bool>> tasks = new List<Task<bool>>();
    for (long i = 0; i < race.Time; i++)
    {
      tasks.Add(Distance(i, race));
    }
    return Task.WhenAll(tasks).Result.Where(r => r).Count();
  }
  public Task<bool> Distance(long speed, Race race)
  {
    return Task.Run(() =>
    {
      long travelTime = race.Time - speed;
      long dist = travelTime * speed;
      return dist > race.Distance;
    });
  }
  public IEnumerable<Race> ParseRaces(string input)
  {
    IEnumerable<Race> races = new List<Race>();
    using (StringReader reader = new StringReader(input))
    {
      string? line;
      line = reader.ReadLine();
      if (string.IsNullOrEmpty(line))
        throw new Exception("bad input");
      Match time = Regex.Match(line, @"Time:\s+(.+)\s*$");
      long times = ParseTime(time.Groups[1].Value);
      line = reader.ReadLine();
      if (string.IsNullOrEmpty(line))
        throw new Exception("bad input");
      Match dist = Regex.Match(line, @"Distance:\s+(.+)\s*$");
      long dists = ParseTime(dist.Groups[1].Value);
      return new List<Race>() { new Race() { Time = times, Distance = dists } };
      /*foreach (var thing in times.Zip(dists))
      {
        Race race = new Race()
        {
          Time = thing.First,
          Distance = thing.Second
        };
        races = races.Append(race);
      }*/
    }
    //return races;
  }
  public long ParseTime(string input)
  {
    return long.Parse(Regex.Replace(input, @"\s+", ""));
  }
  public IEnumerable<int> ParseTimes(string input)
  {
    IEnumerable<int> times = Regex.Split(input, @"\s+").Select(t => int.Parse(t));
    return times;
  }
  public IEnumerable<int> ParseDistances(string input)
  {
    IEnumerable<int> distances = Regex.Split(input, @"\s+").Select(d => int.Parse(d));
    return distances;
  }
}