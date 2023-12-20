
using System.Diagnostics;

public class Program
{
  const bool DEBUG = false;
  static void Main(string[] args)
  {
    if (args.Length < 2)
    {
      Console.WriteLine("usage: dotnet run <day> <input-file>");
      return;
    }
    int day = int.Parse(args[0]);
    string fileName = args[1];
    string file = $"inputs/day{day}/{fileName}";
    string text = File.ReadAllText(file);
    Dictionary<int, IDay> days = new Dictionary<int, IDay>{
      {1, new Day01(DEBUG)},
      {2,new Day02(DEBUG)},
      {3,new Day03(DEBUG)},
      {4,new Day04(DEBUG)},
      {5,new Day05()},
      {6,new Day06()},
      {7,new Day07()},
      {8,new Day08()},
      {9,new Day09()},
      {10,new Day10()},
      {11,new Day11()},
      {12,new Day12()},
      {13,new Day13()},
      {14, new Day14()},
      {15,new Day15()},
      {16,new Day16()},
      {17,new Day17()},
      {18,new Day18()},
      {19,new Day19()},
      {20,new Day20()}
    };
    Stopwatch sw = new Stopwatch();
    sw.Start();
    days[day].Run(text);
    sw.Stop();
    Console.WriteLine($"Code ran in {sw.ElapsedMilliseconds} milliseconds");
  }

}