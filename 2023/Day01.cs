using System.Text.RegularExpressions;

public class Day01 : IDay
{
  private bool DEBUG;
  public Day01(bool debug)
  {
    DEBUG = debug;
  }
  private string convertWords(string input)
  {
    switch (input)
    {
      case "one":
        return "1";
      case "two":
        return "2";
      case "three":
        return "3";
      case "four":
        return "4";
      case "five":
        return "5";
      case "six":
        return "6";
      case "seven":
        return "7";
      case "eight":
        return "8";
      case "nine":
        return "9";
      default:
        return input;
    }
  }
  private int checkLineRegex(string line)
  {
    string pattern = @"(\d|zero|one|two|three|four|five|six|seven|eight|nine)";
    string first = convertWords(Regex.Match(line, pattern).Value);
    string last = convertWords(Regex.Match(line, pattern, RegexOptions.RightToLeft).Value);
    return int.Parse(first + last);
  }
  public void Run(string input)
  {
    int sum = 0;
    using (StringReader reader = new StringReader(input))
    {
      string? line;
      while ((line = reader.ReadLine()) != null)
      {
        int correct = checkLineRegex(line);
        int current = checkLine(line);
        if (correct != current)
          Console.WriteLine($"{correct} {current} {line}");
        sum += checkLine(line);
      }
    }
    Console.WriteLine(sum);
  }
  public int checkLine(string line)
  {
    IEnumerable<int> digits = new List<int>();
    IEnumerable<string> tokens = new List<string>();
    int index = 0;
    int lookahead = 0;
    char[] lineChars = line.ToCharArray();

    Action<string, int, int, string> checkDigit = delegate (string val, int value, int length, string token)
    {
      if (DEBUG) Console.Write($"Checking {token}:");
      bool rest = checkRest(lineChars, lookahead, length, val);
      if (rest)
      {
        digits = digits.Append(value);
        tokens = tokens.Append(token);
      }
      index++;
    };
    while (index < lineChars.Length)
    {
      if (DEBUG)
      {
        Console.WriteLine(String.Join(',', digits));
        Console.WriteLine(String.Join(',', tokens));
        Console.WriteLine($"{index} {lineChars.Length} {lineChars[index]}");

      }
      lookahead = index;
      switch (lineChars[index])
      {
        case > '0' and <= '9':
          digits = digits.Append(lineChars[index] - '0');
          tokens = tokens.Append(lineChars[index].ToString());
          index++;
          break;
        case 'o':
          checkDigit("ne", 1, 2, "one");
          break;
        case 't':
          lookahead = index + 1;
          if (lineChars.Length > lookahead)
          {
            switch (lineChars[lookahead])
            {
              case 'w':
                checkDigit("o", 2, 1, "two");
                break;
              case 'h':
                checkDigit("ree", 3, 3, "three");
                break;
              default:
                index = lookahead;
                break;
            }
          }
          else
          {
            index++;
          }
          break;
        case 'f':
          lookahead = index + 1;
          if (lineChars.Length > lookahead)
          {
            switch (lineChars[lookahead])
            {
              case 'o':
                checkDigit("ur", 4, 2, "four");
                break;
              case 'i':
                checkDigit("ve", 5, 2, "five");
                break;
              default:
                index = lookahead;
                break;
            }
          }
          else
          {
            index++;
          }
          break;
        case 's':
          lookahead = index + 1;
          if (lineChars.Length > lookahead)
          {
            switch (lineChars[lookahead])
            {
              case 'i':
                checkDigit("x", 6, 1, "six");
                break;
              case 'e':
                checkDigit("ven", 7, 3, "seven");
                break;
              default:
                index = lookahead;
                break;
            }
          }
          else
          {
            index++;
          }
          break;
        case 'e':
          checkDigit("ight", 8, 4, "eight");
          break;
        case 'n':
          checkDigit("ine", 9, 3, "nine");
          break;
        case 'z':
          checkDigit("ero", 0, 3, "zero");
          break;
        default:
          index++;
          break;
      }
    }
    string calValue = digits.First().ToString() + digits.Last().ToString();
    int cal = int.Parse(calValue);

    return cal;
  }
  bool checkRest(char[] chars, int index, int length, string val)
  {
    char[] subset = chars.Skip(index + 1).Take(length).ToArray();
    if (DEBUG) Console.WriteLine($"{new string(subset)} {val}");
    return val.Equals(new string(subset));
  }
}