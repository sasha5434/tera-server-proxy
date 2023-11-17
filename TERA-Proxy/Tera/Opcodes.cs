using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tera.Opcodes
{
    public class OpCodeNamer
    {
        private Dictionary<string, ushort> OpCodeCodes;
        private Dictionary<ushort, string> OpCodeNames;
        private readonly string _path;

        private OpCodeNamer(IEnumerable<KeyValuePair<ushort, string>> names)
        {
            var namesArray = names.ToArray();
            OpCodeNames = namesArray.ToDictionary(parts => parts.Key, parts => parts.Value);
            OpCodeCodes = namesArray.ToDictionary(parts => parts.Value, parts => parts.Key);
        }

        public OpCodeNamer(string filename)
            : this(ReadOpCodeFile(filename))
        {
            _path = Path.GetDirectoryName(filename);
        }

        public string GetName(ushort opCode)
        {
            if (OpCodeNames.TryGetValue(opCode, out string name))
                return name;
            Console.WriteLine($"Not found name for {opCode}!!!");
            return opCode.ToString("X4");
        }

        private static IEnumerable<KeyValuePair<ushort, string>> ReadOpCodeFile(string filename)
        {
            if (!File.Exists(filename)) { return new List<KeyValuePair<ushort, string>>(); }
            var names = File.ReadLines(filename)
                .Select(s => Regex.Replace(s.Replace("=", " "), @"\s+", " ").Split(' ').ToArray())
                .Select(parts => new KeyValuePair<ushort, string>(ushort.Parse(parts[1]), parts[0]));
            return names;
        }

        public ushort GetCode(string name)
        {
            if (OpCodeCodes.TryGetValue(name, out ushort code))
                return code;
            Console.WriteLine("Missing opcode: " + name);
            return 0;
            //throw new ArgumentException($"Unknown name '{name}'");
        }
    }
}