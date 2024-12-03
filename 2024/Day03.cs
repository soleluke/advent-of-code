using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day03 : IDay
{

    public void Run(string input)
    {
        var rows = ParseRows(input);
        foreach (var row in rows)
        {
            Console.WriteLine($"{row.Item1} {row.Item2}");
        }

        Console.WriteLine(rows.Select(r => r.Item1 * r.Item2).Sum());
    }

    public bool Consume(char expected, Queue<char> q)
    {
        char check = q.Peek();
        if (check == expected)
        {
            q.Dequeue();
            return true;
        }
        return false;

    }
    public bool Consume(string expected, Queue<char> q)
    {
        foreach (char c in expected.ToCharArray())
        {
            if (!Consume(c, q))
                return false;
        }
        return true;
    }

    public (bool, (int, int)) ConsumeMul(Queue<char> inp)
    {
        if (!Consume("mul(", inp))
            return (false, (0, 0));
        List<char> num1 = new();
        while (Char.IsDigit(inp.Peek()))
        {
            var c = inp.Dequeue();
            num1.Add(c);
        }
        if (!Consume(',', inp))
            return (false, (0, 0));
        List<char> num2 = new();
        while (Char.IsDigit(inp.Peek()))
        {
            var c = inp.Dequeue();
            num2.Add(c);
        }
        if (!Consume(')', inp))
            return (false, (0, 0));
        int n1 = int.Parse(new String(num1.ToArray()));
        int n2 = int.Parse(new String(num2.ToArray()));
        return (true, (n1, n2));

    }


    public List<(int, int)> ParseRows(string input)
    {
        List<(int, int)> rows = new();

        Queue<char> inp = new Queue<char>(input.ToCharArray());
        bool enabled = true;
        while (inp.Any())
        {
            switch (inp.Peek())
            {
                case 'd':
                    if (!Consume("do", inp))
                        break;
                    switch (inp.Peek())
                    {
                        case '(':
                            if (!Consume("()", inp))
                                break;
                            enabled = true;
                            break;
                        case 'n':
                            if (!Consume("n't()", inp))
                                break;
                            enabled = false;
                            break;
                    }
                    break;
                case 'm':
                    var (good, mul) = ConsumeMul(inp);
                    if (good && enabled)
                    {
                        rows.Add(mul);
                    }
                    break;
                default:
                    inp.Dequeue();
                    break;
            }

        }
        return rows;
    }
}
