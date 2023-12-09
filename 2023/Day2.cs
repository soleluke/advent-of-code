using System.Text.RegularExpressions;

public class Day2 : IDay
{
  private class Game
  {
    public class Set
    {
      public int Green { get; set; }
      public int Red { get; set; }
      public int Blue { get; set; }
      public override string ToString()
      {
        return $"{Red} red, {Blue} blue, {Green} green";
      }
    }
    public int Number { get; set; }
    public IEnumerable<Set> Sets { get; set; }
    public Game()
    {
      Sets = new List<Set>();
    }
    public override string ToString()
    {
      return $"Game {Number}: {string.Join("; ", Sets.Select(s => s.ToString()))}";
    }
  }
  private class Bag
  {
    public int Green { get; set; }
    public int Red { get; set; }
    public int Blue { get; set; }
  }
  private bool Debug;

  public Day2(bool debug)
  {
    Debug = debug;
  }
  public void Run(string input)
  {
    Bag bag = new Bag()
    {
      Green = 13,
      Red = 12,
      Blue = 14
    };
    IEnumerable<Game> games = ParseGames(input);
    IEnumerable<int> ids = new List<int>();
    IEnumerable<Game.Set> minimumSets = new List<Game.Set>();
    foreach (Game game in games)
    {
      Game.Set minSet = new Game.Set() { Red = 0, Blue = 0, Green = 0 };
      foreach (Game.Set set in game.Sets)
      {
        if (set.Blue > minSet.Blue)
          minSet.Blue = set.Blue;
        if (set.Red > minSet.Red)
          minSet.Red = set.Red;
        if (set.Green > minSet.Green)
          minSet.Green = set.Green;
      }
      minimumSets = minimumSets.Append(minSet);
      if (game.Sets.All(s =>
        s.Green <= bag.Green &&
        s.Red <= bag.Red &&
        s.Blue <= bag.Blue
      ))
      {
        ids = ids.Append(game.Number);
      }
    }
    Console.WriteLine(minimumSets.Select((s) => SetPower(s)).Aggregate((a, i) => a += i));
    Console.WriteLine(string.Join(',', ids));
    Console.WriteLine(ids.Aggregate((a, i) => a += i));
  }
  private IEnumerable<Game> ParseGames(string input)
  {
    IEnumerable<Game> games = new List<Game>();
    using (StringReader reader = new StringReader(input))
    {
      string? line;
      while ((line = reader.ReadLine()) != null)
      {
        games = games.Append(ParseGame(line));
      }
    }
    return games;
  }
  private Game ParseGame(string input)
  {
    Regex rg = new Regex(@"^Game\s(\d+)\:\s+(.+)$");
    Game game = new Game();
    Match match = rg.Match(input);
    game.Number = int.Parse(match.Groups[1].Value);
    string gameDesc = match.Groups[2].Value;
    IEnumerable<string> setStrings = gameDesc.Split(';');
    Regex colorRg = new Regex(@"(\d+)\s+(blue|red|green)");
    foreach (string setString in setStrings)
    {
      Game.Set set = new Game.Set();
      MatchCollection colorMatch = colorRg.Matches(setString);
      Match? blueMatch = colorMatch.FirstOrDefault(cm => cm.Groups[2].Value == "blue");
      if (blueMatch != null)
      {
        set.Blue = int.Parse(blueMatch.Groups[1].Value);
      }
      Match? redMatch = colorMatch.FirstOrDefault(cm => cm.Groups[2].Value == "red");
      if (redMatch != null)
      {
        set.Red = int.Parse(redMatch.Groups[1].Value);
      }
      Match? greenMatch = colorMatch.FirstOrDefault(cm => cm.Groups[2].Value == "green");
      if (greenMatch != null)
      {
        set.Green = int.Parse(greenMatch.Groups[1].Value);
      }
      game.Sets = game.Sets.Append(set);
    }
    return game;
  }
  private int SetPower(Game.Set set)
  {
    return set.Blue * set.Green * set.Red;
  }
}