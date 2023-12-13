public class Day3 : IDay
{
  private class Number
  {
    public int Value { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public override string ToString()
    {
      return $"({Value}: {x},{y})";
    }
  }
  private class Part
  {
    public bool isGear { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public IEnumerable<Number> Numbers { get; set; }
    public Part()
    {
      Numbers = new List<Number>();
    }
    public override string ToString()
    {
      return $"({x},{y}): {isGear}\n{string.Join(',', Numbers.Select(n => n.ToString()))}";
    }
  }
  private bool DEBUG { get; set; }
  public Day3(bool debug)
  {
    DEBUG = debug;
  }
  public void Run(string input)
  {
    char[][] schematic = ParseStuff(input);
    IEnumerable<Part> parts = new List<Part>();
    for (int i = 0; i < schematic.Length; i++)
    {
      for (int j = 0; j < schematic[i].Length; j++)
      {
        char test = schematic[i][j];
        if (!Char.IsDigit(test) && test != '.')
        {
          parts = parts.Append(new Part()
          {
            x = i,
            y = j,
            isGear = test == '*'
          });
        }
      }
    }
    IEnumerable<Part> partsNumbers = new List<Part>();
    foreach (Part part in parts)
    {
      partsNumbers = partsNumbers.Append(getPartNumbers(part, schematic));
    }
    IEnumerable<Number> allNumbers = partsNumbers.SelectMany(p => p.Numbers);
    int sum = allNumbers.DistinctBy((n) => new { n.x, n.y }).Select(n => n.Value).Sum();
    int gearSum = partsNumbers.Where(p => p.isGear && p.Numbers.Count() == 2).Select(p => p.Numbers.Select(n => n.Value).Sum()).Sum();
    Console.WriteLine($"parts {sum}");
    Console.WriteLine($"gears {gearSum}");
    //38682924 too low

  }
  private Number getNumber(int x, int y, char[][] schematic)
  {
    //lines are affected by y
    int start = y;
    while (start > 0 && Char.IsDigit(schematic[x][start]))
    {
      start--;
    }
    if (!Char.IsDigit(schematic[x][start]))
    {
      start++;
    }
    int end = y;
    while (end < schematic[x].Length && Char.IsDigit(schematic[x][end]))
    {
      end++;
    }
    return new Number()
    {
      Value = int.Parse(new string(schematic[x].Skip(start).Take(end - start).ToArray())),
      x = x,
      y = start,
    };
  }
  private bool coordValid(int x, int y, char[][] schematic)
  {
    if (x < 0) return false;
    if (y < 0) return false;
    if (x >= schematic.Length) return false;
    if (y >= schematic[x].Length) return false;
    return true;
  }
  private Part getPartNumbers(Part part, char[][] schematic)
  {
    int x = part.x;
    int y = part.y;
    List<Tuple<int, int>> corners = new List<Tuple<int, int>>(){
      new Tuple<int, int>(x-1,y-1), // top left
      new Tuple<int, int>(x,y-1), // left
      new Tuple<int, int>(x+1,y-1), // down left 
      new Tuple<int, int>(x-1,y), //top middle
      new Tuple<int, int>(x+1,y), // down middle
      new Tuple<int, int>(x-1,y+1),//top right
      new Tuple<int, int>(x,y+1), //right
      new Tuple<int, int>(x+1,y+1), // down right
    };
    IEnumerable<Tuple<int, int>> numbers = new List<Tuple<int, int>>();
    foreach (Tuple<int, int> corner in corners.Where(c => coordValid(c.Item1, c.Item2, schematic)))
    {
      if (Char.IsDigit(schematic[corner.Item1][corner.Item2]))
      {
        numbers = numbers.Append(corner);
      }
    }
    //remove dupes
    foreach (Tuple<int, int> num in numbers)
    {
      Number number = getNumber(num.Item1, num.Item2, schematic);
      bool isValid = !part.Numbers.Any(n => n.y == number.y && n.x == number.x);
      //if on same line as any
      if (isValid)
      {
        part.Numbers = part.Numbers.Append(number);
      }
    }
    return part;
  }

  private bool CheckSymbol(char c)
  {
    if (c == '.') return false;
    if (Char.IsDigit(c)) return false;
    return true;
  }

  private char[][] ParseStuff(string input)
  {
    IEnumerable<char[]> schematic = new List<char[]>();
    using (StringReader reader = new StringReader(input))
    {
      string? line;
      while ((line = reader.ReadLine()) != null)
      {
        schematic = schematic.Append(line.ToCharArray());
      }
    }
    return schematic.ToArray();
  }
}