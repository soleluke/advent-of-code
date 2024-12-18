using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;

public class Day17 : IDay
{
    public class Computer
    {
        public long A { get; set; }
        public long B { get; set; }
        public long C { get; set; }
        public List<long> Outs { get; set; }
        public Computer()
        {
            Outs = new();
            Outputs = new();
            Leading = new();
        }
        public long GetCombo(long op)
        {
            if (op < 4)
                return op;
            switch (op)
            {
                case 4: return A;
                case 5: return B;
                case 6: return C;
                default:
                    throw new NotSupportedException();
            }
        }
        public long IP { get; set; }
        public List<(long, long)> Leading { get; set; }
        public List<(long, List<(long, long)>)> Outputs { get; set; }
        public bool Op(long opcode, long operand)
        {
            long opc = opcode;
            long op = operand;

            Leading.Add((opc, op));
            switch (opc)
            {
                case 0:
                    op = GetCombo(op);
                    A = (long)double.Truncate(A / Math.Pow(2, op));
                    break;
                case 1:
                    B = B ^ op;
                    break;
                case 2:
                    op = GetCombo(op);
                    B = op % 8;
                    break;
                case 3:
                    if (A == 0)
                        break;
                    IP = op;
                    return false;
                case 4:
                    B = B ^ C;
                    break;
                case 5:
                    Outputs.Add((op, Leading));
                    Leading = new();
                    op = GetCombo(op);

                    Outs.Add(op % 8);
                    break;
                case 6:
                    op = GetCombo(op);
                    B = (long)double.Truncate(A / Math.Pow(2, op));
                    break;
                case 7:
                    op = GetCombo(op);
                    C = (long)double.Truncate(A / Math.Pow(2, op));
                    break;

            }
            return true;

        }
        public void Run(long[] program)
        {
            IP = 0;
            while (IP < program.Length - 1)
            {
                long opcode = program[IP];
                long operand = program[IP + 1];
                var inc = Op(opcode, operand);
                if (inc)
                {
                    IP += 2;
                }
            }
        }
    }
    public (int, int) ValAt(int i, int a)
    {

        int res = a;
        res = res % 8;
        res = res ^ 6;
        res = res ^ (int)double.Truncate(a / Math.Pow(2, 5));
        res = res ^ 7;
        if (i == 0)
            return (res % 8, (int)double.Truncate(a / Math.Pow(2, 5)));
        else
            return ValAt(i, (int)double.Truncate(a / Math.Pow(2, 5)));

    }
    public void Run(string input)
    {
        var (comp, prog) = ParseRows(input);
        int start = 0;
        var progString = string.Join(',', prog);
        var longProg = prog.Select(p => (long)(p - '0')).ToArray();
        var vars = new PriorityQueue<int, long>();
        vars.Enqueue(prog.Length - 1, 0);
        while (vars.TryDequeue(out var pIndex, out var a))
        {
            for (var i = 0; i < 8; i++)
            {
                var c = new Computer();
                var next = (a << 3) + i;
                c.A = next;
                c.Run(longProg);

                if (c.Outs.SequenceEqual(longProg[pIndex..]))
                {
                    Console.WriteLine($"{next}: {string.Join(',', c.Outs)}");
                    if (pIndex == 0)
                        Console.WriteLine($"Found : {next}");
                    if (pIndex > 0)
                    {
                        vars.Enqueue(pIndex - 1, next);
                    }

                }
            }
        }
    }
    public int Calc(long[] p, int i)
    {
        if (i == p.Length)
            return 0;
        var next = Calc(p, i + 1);
        next = next * 8;
        var progString = string.Join(',', p.Skip(i));
        var outs = "";
        while (outs != progString)
        {
            var c = new Computer();
            c.A = next;
            c.Run(p);
            outs = string.Join(',', c.Outs);
            next++;
        }
        Console.WriteLine($"{next - 1}: {outs}");
        return next - 1;

    }

    public (Computer, char[]) ParseRows(string input)
    {

        using (StringReader sr = new StringReader(input))
        {
            string? line;
            while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
            {
                var regA = new Regex(@"Register\ A\: (\d+)");
                var regB = new Regex(@"Register\ B\: (\d+)");
                var regC = new Regex(@"Register\ C\: (\d+)");
                var prog = new Regex(@"Program\:\s+((\d\,*)+)");
                var a = regA.Match(line);
                line = sr.ReadLine();
                var b = regB.Match(line);
                line = sr.ReadLine();
                var c = regC.Match(line);
                sr.ReadLine();
                line = sr.ReadLine();
                var p = prog.Match(line);

                var comp = new Computer();
                comp.A = int.Parse(a.Groups[1].Value);
                comp.B = int.Parse(b.Groups[1].Value);
                comp.C = int.Parse(c.Groups[1].Value);
                var program = p.Groups[1].Value.Split(',');
                return (comp, program.Select(s => char.Parse(s.Trim())).ToArray());

            }

        }
        throw new NotSupportedException();
    }
}
