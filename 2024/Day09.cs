using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Immutable;
using System.Drawing;

public class Day09 : IDay
{
    public void SplitFile(int[] disk)
    {
        List<int> filesSeen = new List<int>();
        int cur = disk[0];
        foreach (int d in disk)
        {
            if (d != -1 && d != cur)
            {
                if (filesSeen.Contains(d))
                    Console.WriteLine($"split file {d}");
                else
                {
                    filesSeen.Add(d);
                    cur = d;
                }
            }
        }
        Console.WriteLine($" Max: {filesSeen.Max()} len: {filesSeen.Count}");
    }
    public void TestDefrag()
    {
        string disk = "000...11....2223334444";
        int[] d = disk.ToCharArray().Select(s => s == '.' ? -1 : s - '0').ToArray();
        PrintDisk(d);
        var df = Defrag(d, d.Length - 1);
        PrintDisk(df);
    }

    public void Run(string input)
    {
        var map = ParseRows(input);
        SetIds(map);
        Console.WriteLine(map.Select(m => m.Id).Max());
        var disk = MakeDisk(map);
        var nd = Compact(disk);
        var checksum = Checksum(nd);
        Console.WriteLine($"compacted: {checksum}");
        var df = Defrag((int[])disk.Clone(), disk.Length - 1);
        var dfcs = Checksum(df);
        SplitFile(df);
        Console.WriteLine(dfcs);
    }
    public long Checksum(int[] nd)
    {
        long checksum = 0;
        for (int i = 0; i < nd.Length; i++)
        {
            if (nd[i] >= 0)
            {
                checksum += nd[i] * i;
            }
        }
        return checksum;
    }
    public static List<int> FIDS = new List<int>();
    public int[] Defrag(int[] nd, int fi)
    {
        if (fi < 0)
            return nd;
        if (nd[fi] == -1)
            return Defrag(nd, fi - 1);
        int fr = 0;
        int fiStart = fi;
        int fId = nd[fi];
        while (fiStart > 0 && nd[fiStart] == fId)
            fiStart--;
        if (fiStart == 0)
            return nd;
        int fiLen = fi - fiStart;
        while (true)
        {
            while (fr < fiStart && nd[fr] >= 0)
                fr++;
            if (fr >= fiStart)
                return Defrag(nd, fiStart);
            int frEnd = fr;
            while (nd[frEnd] < 0)
                frEnd++;
            int frLen = frEnd - fr;
            if (frLen >= fiLen)
                break;
            else
            {
                fr = frEnd + 1;
            }
            if (frEnd == fiStart)
                //couldn't move file
                return Defrag(nd, fiStart);
        }
        fiStart += 1;
        if (!FIDS.Contains(fId))
        {
            FIDS.Add(fId);
            for (int i = 0; i < fiLen; i++)
            {
                nd[fr + i] = nd[fiStart + i];
                nd[fiStart + i] = -1;
            }
        }
        return Defrag(nd, fiStart - 1);

    }
    public int[] Compact(int[] disk)
    {
        int[] nd = (int[])disk.Clone();
        int fr = 0;
        int fi = nd.Length - 1;
        while (fr < fi)
        {
            //Console.Write($"fr: {fr} fi: {fi} ");
            //PrintDisk(nd);
            if (nd[fr] < 0)
            {
                while (nd[fi] == -1)
                    fi--;
                //free space
                nd[fr] = nd[fi];
                nd[fi] = -1;
                fi--;
            }
            fr++;
        }
        return nd;
    }
    public void PrintDisk(int[] disk)
    {

        Console.WriteLine(disk.Select(d => d >= 0 ? d.ToString() : ".").Aggregate((d, a) => d += " " + a));
    }
    public int[] MakeDisk(List<Block> l)
    {
        List<int> disk = new();
        foreach (var b in l)
        {
            switch (b.T)
            {
                case Type.File:
                    for (int i = 0; i < b.L; i++)
                    {
                        disk.Add(b.Id.Value);
                    }
                    break;
                case Type.Free:
                    for (int i = 0; i < b.L; i++)
                    {
                        disk.Add(-1);
                    }
                    break;
            }
        }
        return disk.ToArray();
    }
    public void SetIds(List<Block> l)
    {
        int curId = 0;
        foreach (var b in l)
        {
            if (b.T == Type.File)
            {
                b.Id = curId;
                curId++;
            }
        }
    }
    public enum Type
    {
        Free,
        File
    }
    public class Block
    {
        public Type T { get; set; }
        public int L { get; set; }
        public int? Id { get; set; }
        public override string ToString()
        {
            return $"{T}: {L} {Id?.ToString() ?? ""}";
        }
    }

    public List<Block> ParseRows(string input)
    {
        List<Block> diskMap = new();
        char[] map = [];
        using (StringReader sr = new StringReader(input))
        {
            string? line;
            while ((line = sr.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
            {
                if (map.Length > 0)
                    throw new NotImplementedException("should only have one line");
                map = line.ToCharArray();
            }

        }
        bool free = false;
        foreach (var c in map)
        {
            Type t;
            if (!free)
            {
                t = Type.File;
            }
            else
            {
                t = Type.Free;
            }
            diskMap.Add(new Block() { T = t, L = c - '0' });
            free = !free;
        }
        return diskMap;
    }
}
