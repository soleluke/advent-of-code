
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
      {11,new Day11()}
    };
    Stopwatch sw = new Stopwatch();
    sw.Start();
    days[day].Run(text);
    sw.Stop();
    Console.WriteLine($"Code ran in {sw.ElapsedMilliseconds} milliseconds");
  }

}