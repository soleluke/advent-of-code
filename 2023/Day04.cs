using System.Text.RegularExpressions;

public class Day04 : IDay
{
  private bool DEBUG { get; set; }
  public Day04(bool debug)
  {
    DEBUG = debug;
  }
  private class ScratchCard
  {
    public int Id { get; set; }
    public IEnumerable<int> Winners { get; set; }
    public IEnumerable<int> Have { get; set; }
    public ScratchCard()
    {
      Winners = new List<int>();
      Have = new List<int>();
    }

    public override string ToString()
    {
      return $"Card {Id}: {string.Join(',', Winners)} | {string.Join(',', Have)}";
    }
  }
  public void Run(string input)
  {
    IList<ScratchCard> cards = ParseStuff(input);
    IList<ScratchCard> toProcess = cards.ToList();

    Dictionary<int, int> cardCounts = new Dictionary<int, int>();
    foreach (ScratchCard card in cards)
    {
      CountCards(card, cards, cardCounts);
    }
    Console.WriteLine($"{string.Join('\n', cardCounts.Select(kv => $"{kv.Key}: {kv.Value}"))}");
    Console.WriteLine(cardCounts.Values.Sum());
  }
  private void CountCards(ScratchCard card, IList<ScratchCard> cards, Dictionary<int, int> counts)
  {
    if (counts.ContainsKey(card.Id))
    {
      counts[card.Id]++;
    }
    else
    {
      counts[card.Id] = 1;
    }

    //    Console.WriteLine($"{card.Id}------------------------");
    //  Console.WriteLine($"{string.Join('\n', counts.Select(kv => $"{kv.Key}: {kv.Value}"))}");
    int matches = Matches(card);
    IEnumerable<ScratchCard> toAdd = cards.Where(c => c.Id <= card.Id + matches && c.Id > card.Id);
    foreach (ScratchCard c in toAdd)
    {
      CountCards(c, cards, counts);
    }
  }
  private int Matches(ScratchCard card)
  {
    return card.Have.Intersect(card.Winners).Count();
  }
  private double Points(ScratchCard card)
  {
    IEnumerable<int> winners = card.Have.Intersect(card.Winners);
    if (!winners.Any())
      return 0;
    double points = Math.Pow(2, winners.Count() - 1);
    return points;
  }
  private IList<ScratchCard> ParseStuff(string input)
  {
    IList<ScratchCard> list = new List<ScratchCard>();
    using (StringReader reader = new StringReader(input))
    {
      string? line;
      while ((line = reader.ReadLine()) != null)
      {
        list.Add(ParseCard(line));
      }
    }
    return list;
  }
  private ScratchCard ParseCard(string line)
  {
    Regex rg = new Regex(@"^Card\s+(\d+):([^|]+)\|([^|]+)$");
    Match match = rg.Match(line);
    string winners = match.Groups[2].Value;
    string have = match.Groups[3].Value;
    string[] w = Regex.Split(winners.Trim(), @"\s+");
    string[] h = Regex.Split(have.Trim(), @"\s+");
    ScratchCard card = new ScratchCard()
    {
      Id = int.Parse(match.Groups[1].Value),
      Winners = w.Select(w => int.Parse(w)),
      Have = h.Select(h => int.Parse(h))
    };
    return card;
  }
}