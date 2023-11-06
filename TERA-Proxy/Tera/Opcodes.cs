using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tera.Opcodes
{
    public class OpCodeNamer
    {
        private Dictionary<string, ushort> _opCodeCodes;
        private Dictionary<ushort, string> _opCodeNames;
        private readonly string _path;

        private OpCodeNamer(IEnumerable<KeyValuePair<ushort, string>> names)
        {
            var namesArray = names.ToArray();
            _opCodeNames = namesArray.ToDictionary(parts => parts.Key, parts => parts.Value);
            _opCodeCodes = namesArray.ToDictionary(parts => parts.Value, parts => parts.Key);
        }

        public OpCodeNamer(string filename)
            : this(ReadOpCodeFile(filename))
        {
            _path = Path.GetDirectoryName(filename);
        }

        public string GetName(ushort opCode)
        {
            string name;
            if (_opCodeNames.TryGetValue(opCode, out name))
                return name;
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
            ushort code;
            if (_opCodeCodes.TryGetValue(name, out code))
                return code;
            Console.WriteLine("Missing opcode: " + name);
            return 0;
            //throw new ArgumentException($"Unknown name '{name}'");
        }
    }
}