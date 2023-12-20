using System.Text.RegularExpressions;

public class Day07 : IDay
{
  public enum Card
  {
    Ace = 1,
    King = 2,
    Queen = 3,
    Joker = 14,
    Ten = 5,
    Nine = 6,
    Eight = 7,
    Seven = 8,
    Six = 9,
    Five = 10,
    Four = 11,
    Three = 12,
    Two = 13,
    //Joker = -1,
  }
  public enum Type
  {
    FiveOfAKind = 1,
    FourOfAKind = 2,
    FullHouse = 3,
    ThreeOfAKind = 4,
    TwoPair = 5,
    OnePair = 6,
    HighCard = 7
  }
  public class Hand
  {
    public IEnumerable<Card> Cards { get; set; }
    public long Bet { get; set; }
    public Hand()
    {
      Cards = new List<Card>();
    }
    public override string ToString()
    {
      return $"{string.Join(',', Cards)} {Bet}";
    }
    private bool CheckCount(IEnumerable<IGrouping<Card, Card>> groups, int count)
    {
      return groups.Count() == count ||
      (groups.Count() == count + 1 && groups.Any(g => g.Key == Card.Joker));
    }
    public Type Type()
    {
      IEnumerable<IGrouping<Card, Card>> groups = Cards.GroupBy(c => c);

      if (CheckCount(groups, 1))
        return Day07.Type.FiveOfAKind;
      if (CheckCount(groups, 2))
      {
        int jokerCount = groups.FirstOrDefault(g => g.Key == Card.Joker)?.Count() ?? 0;
        if (groups.Any(g => g.Count() == 4) || groups.Any(g => g.Count() + jokerCount == 4))
        {
          return Day07.Type.FourOfAKind;
        }
        else
        {
          return Day07.Type.FullHouse;
        }
      }
      if (CheckCount(groups, 3))
      {
        int jokerCount = groups.FirstOrDefault(g => g.Key == Card.Joker)?.Count() ?? 0;
        if (groups.Count(g => g.Count() == 2) == 2
        || jokerCount > 2
        )
          return Day07.Type.TwoPair;
        else
          return Day07.Type.ThreeOfAKind;
      }
      if (CheckCount(groups, 4))
        return Day07.Type.OnePair;
      if (CheckCount(groups, 5))
        return Day07.Type.HighCard;
      throw new Exception("failed to parse type " + this.ToString());

    }
    public int Compare(Hand hand2)
    {
      int typeComp = this.Type() - hand2.Type();
      if (typeComp != 0)
        return typeComp * -1;
      foreach (var thing in this.Cards.Zip(hand2.Cards))
      {
        if (thing.First != thing.Second)
        {
          return -1 * (thing.First - thing.Second);
        }
      }
      return 0;
    }

  }
  public void Run(string input)
  {
    List<Hand> hands = ParseHands(input).ToList();
    hands.Sort((x, y) => x.Compare(y));
    List<long> winnings = new List<long>();
    for (int i = 0; i < hands.Count; i++)
    {
      int rank = i + 1;
      //Console.WriteLine(hands[i].ToString() + " " + hands[i].Type() + " " + rank);
      winnings.Add(rank * hands[i].Bet);
    }

    Console.WriteLine(winnings.Sum());
  }
  private IEnumerable<Hand> ParseHands(string input)
  {
    IEnumerable<Hand> list = new List<Hand>();
    using (StringReader reader = new StringReader(input))
    {
      string? line;
      while ((line = reader.ReadLine()) != null)
      {
        Regex handRg = new Regex(@"^([AKQJT98765432]+)\s+(\d+)$");
        Match match = handRg.Match(line);
        Hand hand = new Hand()
        {
          Cards = ParseCards(match.Groups[1].Value),
          Bet = long.Parse(match.Groups[2].Value)
        };
        list = list.Append(hand);
      }
    }
    return list;
  }
  IEnumerable<Card> ParseCards(string input)
  {
    IList<Card> cards = new List<Card>();
    foreach (char c in input.ToCharArray())
    {
      cards.Add(ParseCard(c));
    }
    return cards;
  }
  Card ParseCard(char input)
  {
    //A, K, Q, J, T, 9, 8, 7, 6, 5, 4, 3, or 2
    switch (input)
    {
      case 'A':
        return Card.Ace;
      case 'K':
        return Card.King;
      case 'Q':
        return Card.Queen;
      case 'J':
        return Card.Joker;
      case 'T':
        return Card.Ten;
      case '9':
        return Card.Nine;
      case '8':
        return Card.Eight;
      case '7':
        return Card.Seven;
      case '6':
        return Card.Six;
      case '5':
        return Card.Five;
      case '4':
        return Card.Four;
      case '3':
        return Card.Three;
      case '2':
        return Card.Two;
      default:
        throw new Exception("invalid card");
    }
  }
}