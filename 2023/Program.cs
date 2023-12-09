
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
    string text = File.ReadAllText(args[1]);
    Dictionary<int, IDay> days = new Dictionary<int, IDay>{
      {1, new Day1(DEBUG)},
      {2,new Day2(DEBUG)},
      {3,new Day3(DEBUG)},
      {4,new Day4(DEBUG)},
      {5,new Day5()},
      {6,new Day6()},
      {7,new Day7()},
      {8,new Day8()},
      {9,new Day9()}
    };
    Stopwatch sw = new Stopwatch();
    sw.Start();
    days[day].Run(text);
    sw.Stop();
    Console.WriteLine($"Code ran in {sw.ElapsedMilliseconds} milliseconds");
  }

}