using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day05 : IDay
{

    public void Run(string input)
    {
        var (pagePairs, updates) = ParseRows(input);
        var rules = new Dictionary<int, List<int>>();
        foreach (var pp in pagePairs)
        {
            if (rules.ContainsKey(pp.Item2))
            {
                rules[pp.Item2].Add(pp.Item1);
            }
            else
            {
                rules[pp.Item2] = new List<int>() { pp.Item1 };
            }
        }
        var check = updates.Select(u => (CheckUpdate(u, rules), u));
        var correct = check.Where(ch => ch.Item1).Select(ch => ch.u);
        var incorrect = check.Where(ch => !ch.Item1).Select(ch => ch.u);
        var mids = new List<int>();
        foreach (var c in correct)
        {
            int i = c.Count / 2;
            mids.Add(c[i]);
        }
        Console.WriteLine("Correct mids");
        //Console.WriteLine(string.Join(',', mids));
        Console.WriteLine(mids.Sum());

        // Console.WriteLine("Incorrect:");
        foreach (var i in incorrect)
        {
            //    Console.WriteLine(string.Join(',', i));
        }
        var corrected = new List<List<int>>();
        foreach (var i in incorrect)
        {
            corrected.Add(CorrectUpdate(i, rules));
        }
        Console.WriteLine("Corrected: ");
        foreach (var c in corrected)
        {
            //     Console.WriteLine(string.Join(',', c));
            //     
        }
        var icMids = new List<int>();
        foreach (var c in corrected)
        {
            int i = c.Count / 2;
            icMids.Add(c[i]);
        }

        Console.WriteLine(icMids.Sum());

    }

    public List<int> CorrectUpdate(List<int> update, Dictionary<int, List<int>> rules)
    {
        List<int> processed = new List<int>();
        int badIndex = -1;
        for (int i = 0; i < update.Count; i++)
        {
            var u = update[i];
            if (rules.ContainsKey(u) && rules[u].Any())
            {
                foreach (var r in rules[u])
                {
                    if (update.Contains(r) && !processed.Contains(r))
                    {
                        badIndex = i;
                        break;
                    }
                }
                if (badIndex != -1)
                {
                    break;
                }
            }
            processed.Add(u);
        }
        //Console.WriteLine($"badIndex: {badIndex}");
        if (badIndex == -1)
        {
            Console.WriteLine($"processed: {string.Join(',', processed)} update:{string.Join(',', processed)}");
        }
        if (badIndex >= update.Count)
            throw new NotImplementedException("bad index too large");
        int e = update[badIndex];
        var rule = rules[e];
        //Console.WriteLine($"e:{e} rule:{string.Join(',', rule)} processed:{string.Join(',', processed)}");
        var relevant = rule.Where(r => update.Contains(r) && !processed.Contains(r)).ToList();
        for (int i = badIndex + 1; i < update.Count; i++)
        {
            var u = update[i];
            if (relevant.Contains(u))
            {
                relevant.Remove(u);
                processed.Add(u);
                if (relevant.Count == 0)
                {
                    processed.Add(e);
                }
            }
            else
            {
                processed.Add(u);
            }

        }
        if (CheckUpdate(processed, rules))
        {
            return processed;
        }
        else
        {
            return CorrectUpdate(processed, rules);
        }
    }

    public bool CheckUpdate(List<int> update, Dictionary<int, List<int>> rules)
    {
        //Console.WriteLine($"Checking update {string.Join(',', update)}");
        List<int> processed = new List<int>();
        foreach (var u in update)
        {
            //Console.WriteLine($"{u} {string.Join(',', rules.GetValueOrDefault(u) ?? [])} {string.Join(',', processed)}");

            if (rules.ContainsKey(u) && rules[u].Any())
            {
                foreach (var r in rules[u])
                {
                    if (update.Contains(r) && !processed.Contains(r))
                        return false;
                }
            }
            processed.Add(u);
        }
        //Console.WriteLine("Update correct");
        return true;
    }

    public (List<(int, int)>, List<List<int>>) ParseRows(string input)
    {
        List<(int, int)> pagePairs = new();
        List<List<int>> updates = new();
        using (StringReader sr = new StringReader(input))
        {
            string? line;
            while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
            {
                var split = line.Split('|');
                var ints = split.Select(s => int.Parse(s.Trim())).ToList();
                if (ints.Count() < 2)
                {
                    throw new NotImplementedException("bad input");
                }
                pagePairs.Add((ints[0], ints[1]));
            }

            while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
            {
                updates.Add(line.Split(',').Select(l => int.Parse(l)).ToList());
            }

        }
        return (pagePairs, updates);
    }
}
