using System.Text.RegularExpressions;

public class Day5 : IDay
{
  public class SeedRange
  {
    public long Start { get; set; }
    public long Range { get; set; }
  }
  public class Map
  {
    public long Source { get; set; }
    public long Dest { get; set; }
    public long Range { get; set; }
    public override string ToString()
    {
      return $"{Dest} {Source} {Range}";
    }
  }
  public class Almanac
  {

    public IEnumerable<SeedRange> Seeds { get; set; }
    public IEnumerable<Map> SeedSoil { get; set; }
    public IEnumerable<Map> SoilFertilizer { get; set; }
    public IEnumerable<Map> FertilizerWater { get; set; }
    public IEnumerable<Map> WaterLight { get; set; }
    public IEnumerable<Map> LightTemperature { get; set; }
    public IEnumerable<Map> TemperatureHumidity { get; set; }
    public IEnumerable<Map> HumidityLocation { get; set; }
    public Almanac()
    {
      Seeds = new List<SeedRange>();
      SeedSoil = new List<Map>();
      SoilFertilizer = new List<Map>();
      FertilizerWater = new List<Map>();
      WaterLight = new List<Map>();
      LightTemperature = new List<Map>();
      TemperatureHumidity = new List<Map>();
      HumidityLocation = new List<Map>();
    }

  }
  public void Run(string input)
  {
    Almanac almanac = ParseStuff(input);
    //Console.WriteLine(almanac.ToString());
    //IEnumerable<Map> map = BuildIntervals(almanac);
    List<long> locations = new List<long>();
    foreach (SeedRange seed in almanac.Seeds)
    {
      List<Task<long>> tasks = new List<Task<long>>();
      for (long i = seed.Start; i < seed.Start + seed.Range; i++)
      {
        tasks.Add(GetLocation(i, almanac));
      }
      long[] results = Task.WhenAll(tasks).Result;

      locations.Add(results.Min());
    }
    Console.WriteLine(locations.Min());
  }
  public IEnumerable<Map> BuildIntervals(Almanac almanac)
  {
    long maxSeed = almanac.Seeds.Max((sr => sr.Start + sr.Range));
    IEnumerable<Map> maps = FillMap(almanac.SeedSoil, maxSeed);

    Console.WriteLine("seed-soil-fert");
    maps = CollapseMap(maps, almanac.SoilFertilizer);
    Console.WriteLine("seed-soil-fert-water");
    maps = CollapseMap(maps, almanac.FertilizerWater);
    Console.WriteLine("seed-soil-fert-water-light");
    maps = CollapseMap(maps, almanac.WaterLight);
    Console.WriteLine("seed-soil-fert-water-lght-temp");
    maps = CollapseMap(maps, almanac.LightTemperature);
    Console.WriteLine("seed-soil-fert-water-lght-temp-hum");
    maps = CollapseMap(maps, almanac.TemperatureHumidity);
    Console.WriteLine("seed-soil-fert------loc");
    maps = CollapseMap(maps, almanac.HumidityLocation);
    return maps;
  }
  public IEnumerable<Map> FillMap(IEnumerable<Map> source, long maxVal)
  {
    long curVal = 0;
    IEnumerable<Map> results = source.ToList();
    foreach (Map map in source.OrderBy(s => s.Source))
    {
      if (map.Source > curVal)
      {
        Map toAdd = new Map
        {
          Source = curVal,
          Dest = curVal,
          Range = map.Source - curVal
        };
        curVal = map.Source + map.Range;
        results = results.Append(toAdd);
      }
    }
    if (curVal < maxVal)
    {
      results = results.Append(new Map()
      {
        Source = curVal,
        Dest = curVal,
        Range = maxVal - curVal
      });
    }
    return results.OrderBy(r => r.Source);
  }
  public IEnumerable<Map> CollapseMap(IEnumerable<Map> source, IEnumerable<Map> dest)
  {
    IEnumerable<Map> result = new List<Map>();

    foreach (Map map in source)
    {
      long maxMapped = map.Source;
      long maxDest = map.Dest;
      long remainingRange = map.Range;
      Map? missedMap = null;
      while (maxMapped < map.Source + map.Range)
      {
        Map? res = dest.FirstOrDefault(d => d.Source + d.Range < maxDest + remainingRange && d.Source >= maxDest);
        if (res != null)
        {
          Map interval = new Map() { Source = maxMapped, Dest = res.Dest };
          long range;
          if (res.Range > remainingRange)
          {
            range = remainingRange;
          }
          else
          {
            remainingRange = remainingRange - res.Range;

            range = res.Range;
          }
          maxMapped += range;
          maxDest += range;
          interval.Range = range;
          if (missedMap != null)
          {
            result = result.Append(missedMap);
            missedMap = null;
          }
          result = result.Append(interval);
        }
        else
        {
          if (missedMap == null)
          {
            missedMap = new Map()
            {
              Source = maxMapped,
              Dest = maxDest,
              Range = 1
            };
          }
          if (missedMap != null && missedMap.Source + missedMap.Range - 1 == maxMapped)
          {
            missedMap.Range++;
            maxMapped++;
            remainingRange--;
            maxDest++;
          }
        }
      }
      if (missedMap != null)
      {
        missedMap.Range--;
        result = result.Append(missedMap);
        missedMap = null;
      }
    }
    Console.WriteLine("results:");
    Console.WriteLine(string.Join('\n', result.Select(r => r.ToString())));
    return result;
  }
  private Task<long> GetLocationMap(long seed, IEnumerable<Map> maps)
  {
    return Task.Run(() => Lookup(seed, maps));
  }
  private Task<long> GetLocation(long seed, Almanac almanac)
  {

    return Task.Run(() =>
    {
      long soil = Lookup(seed, almanac.SeedSoil);
      long fertilizer = Lookup(soil, almanac.SoilFertilizer);
      long water = Lookup(fertilizer, almanac.FertilizerWater);
      long light = Lookup(water, almanac.WaterLight);
      long temperature = Lookup(light, almanac.LightTemperature);
      long humidity = Lookup(temperature, almanac.TemperatureHumidity);
      long location = Lookup(humidity, almanac.HumidityLocation);
      return location;
    });
  }
  public long Lookup(long input, IEnumerable<Map> map)
  {
    Map? result = map.FirstOrDefault(m => input >= m.Source && input < m.Source + m.Range);
    if (result == null)
    {
      return input;
    }
    return result.Dest + (input - result.Source);
  }
  private Almanac ParseStuff(string input)
  {
    Almanac almanac = new Almanac();
    using (StringReader reader = new StringReader(input))
    {
      string? line;
      line = reader.ReadLine();
      if (string.IsNullOrEmpty(line))
      {
        throw new Exception("bad input");
      }
      Regex seedRg = new Regex(@"seeds\:\s+(.+)\s*$");
      Match seedMatch = seedRg.Match(line);
      if (seedMatch.Success)
      {
        IList<long> vals = Regex.Split(seedMatch.Groups[1].Value, @"\s+").Select(s => long.Parse(s)).ToList();
        IEnumerable<SeedRange> seeds = new List<SeedRange>();
        for (int i = 0; i < vals.Count() - 1; i += 2)
        {
          seeds = seeds.Append(new SeedRange() { Start = vals[i], Range = vals[i + 1] });
        }
        almanac.Seeds = seeds;
      }
      else
      {
        Console.WriteLine(line);
        throw new Exception("bad input");
      }
      reader.ReadLine();
      almanac.SeedSoil = ParseMap(reader, "seed-to-soil map:");
      almanac.SoilFertilizer = ParseMap(reader, "soil-to-fertilizer map:");
      almanac.FertilizerWater = ParseMap(reader, "fertilizer-to-water map:");
      almanac.WaterLight = ParseMap(reader, "water-to-light map:");
      almanac.LightTemperature = ParseMap(reader, "light-to-temperature map:");
      almanac.TemperatureHumidity = ParseMap(reader, "temperature-to-humidity map:");
      almanac.HumidityLocation = ParseMap(reader, "humidity-to-location map:");
    }
    return almanac;
  }
  private IEnumerable<Map> ParseMap(StringReader reader, string label)
  {
    IEnumerable<Map> dict = new List<Map>();
    string? line = reader.ReadLine();
    if (!line?.StartsWith(label) ?? false)
    {
      Console.WriteLine(line);
      throw new Exception("bad input");
    }
    while ((line = reader.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
    {
      List<string> sl = Regex.Split(line, @"\s+").ToList();
      List<long> l = sl.Select((s => long.Parse(s))).ToList();
      long dest = l[0];
      long source = l[1];
      long len = l[2];

      Map map = new Map()
      {
        Dest = dest,
        Source = source,
        Range = len
      };
      dict = dict.Append(map);
    }
    return dict;
  }
}