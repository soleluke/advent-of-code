using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day04 : IDay
{

  public class Board
  {
    public bool Won { get; set; }
    public Board(int[][] r)
    {
      Rows = r;
      Marks = new bool[r.Length, r[0].Length];
    }
    public int[][] Rows { get; set; }
    public bool[,] Marks { get; set; }
    public void Print()
    {
      for (int r = 0; r < Rows.Length; r++)
      {
        for (int c = 0; c < Rows[0].Length; c++)
        {
          if (!Marks[r, c])
          {
            Console.Write($" {Rows[r][c],2} ");
          }
          else
          {
            Console.Write(" XX ");
          }

        }
        Console.WriteLine();
      }
    }
    public void Mark(int draw)
    {
      for (int r = 0; r < Rows.Length; r++)
      {
        for (int c = 0; c < Rows[0].Length; c++)
        {
          if (Rows[r][c] == draw)
          {
            Marks[r, c] = true;
          }
        }
      }
    }
    public bool Winning()
    {
      for (int r = 0; r < Marks.GetLength(0); r++)
      {
        bool row = true;
        for (int c = 0; c < Marks.GetLength(1); c++)
        {
          row = row && Marks[r, c];
        }
        if (row)
          return true;
      }
      for (int c = 0; c < Marks.GetLength(1); c++)
      {
        bool col = true;
        for (int r = 0; r < Marks.GetLength(0); r++)
        {
          col = col && Marks[r, c];
        }
        if (col)
          return true;
      }
      return false;
    }
    public int Score()
    {
      int score = 0;
      for (int r = 0; r < Rows.Length; r++)
      {
        for (int c = 0; c < Rows[0].Length; c++)
        {
          if (!Marks[r, c])
          {
            score += Rows[r][c];
          }
        }
      }
      return score;
    }
  }
  public void Run(string input)
  {
    var res = ParseRows(input);
    var draws = res.Item1;
    var boards = res.Item2;

    Board? lastWinner = null;
    foreach (int draw in draws)
    {
      MarkBoards(boards, draw);
      List<Board> winner = CheckBoards(boards);
      if (winner.Any())
      {
        foreach (var w in winner)
        {
          Console.WriteLine($"{draw}: {w.Score() * draw}");
          w.Print();
        }
        if (boards.All(b => b.Won))
          break;
      }
    }

  }
  public void MarkBoards(Board[] boards, int draw)
  {
    foreach (var b in boards)
    {
      b.Mark(draw);

    }
  }
  public List<Board> CheckBoards(Board[] boards)
  {
    List<Board> winners = new List<Board>();
    foreach (var b in boards.Where(b => !b.Won))
    {
      if (b.Winning())
        b.Won = true;
      winners.Add(b);
    }
    return winners;
  }


  public (int[], Board[]) ParseRows(string input)
  {
    int[] draws;
    List<Board> boards = new List<Board>();
    using (StringReader sr = new StringReader(input))
    {
      string? line;
      line = sr.ReadLine();
      draws = line.Split(',').Select(s => int.Parse(s)).ToArray();
      //empty space before boards
      line = sr.ReadLine();
      List<int[]> rows = new List<int[]>();
      while ((line = sr.ReadLine()) != null)
      {
        if (!string.IsNullOrWhiteSpace(line))
        {
          rows.Add(System.Text.RegularExpressions.Regex.Split(line.Trim(), @"\s+").Select(s => int.Parse(s)).ToArray());
        }
        else
        {
          boards.Add(new Board(rows.ToArray()));
          rows = new List<int[]>();
        }
      }
      boards.Add(new Board(rows.ToArray()));
    }
    return (draws, boards.ToArray());
  }
}