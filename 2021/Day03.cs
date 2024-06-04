using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day03 : IDay
{


  public void Run(string input)
  {
    var dr = ParseRows(input);
    List<bool> gamma = new List<bool>();
    List<bool> epsilon = new List<bool>();
    for (int c = 0; c < dr[0].Length; c++)
    {
      int tc = dr.Count(r => r[c]);
      int fc = dr.Count() - tc;
      gamma.Insert(c, tc > fc);
      epsilon.Insert(c, fc > tc);
    }
    IEnumerable<bool[]> og = dr.ToList();
    IEnumerable<bool[]> cs = dr.ToList();
    for (int c = 0; c < dr[0].Length; c++)
    {
      int tc = og.Count(r => r[c]);
      int fc = og.Count() - tc;
      og = og.Where(r => r[c] == tc >= fc).ToList();
      if (og.Count() == 1)
        break;
    }
    for (int c = 0; c < dr[0].Length; c++)
    {
      int tc = cs.Count(r => r[c]);
      int fc = cs.Count() - tc;
      cs = cs.Where(r => r[c] == fc > tc).ToList();
      if (cs.Count() == 1)
        break;
    }

    IEnumerable<bool> ogr = og.First();
    IEnumerable<bool> csr = cs.First();
    Print(ogr);
    ogr = ogr.Reverse();
    csr = csr.Reverse();
    gamma.Reverse();
    epsilon.Reverse();
    var g = ParseBits(gamma);
    var e = ParseBits(epsilon);
    var o = ParseBits(ogr);
    var s = ParseBits(csr);
    Console.WriteLine(g);
    Console.WriteLine(e);

    Console.WriteLine(g * e);
    Console.WriteLine("Ratings");
    Console.WriteLine(o);
    Console.WriteLine(s);
    Console.WriteLine(o * s);

  }
  void Print(IEnumerable<bool[]> bits)
  {
    foreach (var r in bits)
    {
      Print(r);
    }
  }
  void Print(IEnumerable<bool> bits)
  {
    Console.WriteLine(String.Join(' ', bits.Select(b => b ? '1' : '0')));
  }
  double ParseBits(IEnumerable<bool> bits)
  {
    double g = 0;
    int pow = 0;
    foreach (bool b in bits)
    {
      if (b)
        g += Math.Pow(2, pow);

      pow++;
    }
    return g;
  }

  public bool[][] ParseRows(string input)
  {
    List<bool[]> dr = new List<bool[]>();

    using (StringReader sr = new StringReader(input))
    {
      string? line;
      while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
      {
        dr.Add(line.Trim().Select(c => c == '1').ToArray());
      }

    }
    return dr.ToArray();
  }
}