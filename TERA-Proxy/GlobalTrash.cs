using Tera.Opcodes;
using TeraPartyMonitor.Structures;

namespace Globals
{
    static class Opcode
    {
        private static OpCodeNamer opCodeNamer;

        public static void Init(int protocol)
        {
            string filename = $"data/protocol.{protocol}.map";
            opCodeNamer = new OpCodeNamer(filename);
        }
        public static string GetName(ushort opcode)
        {
            return opCodeNamer?.GetName(opcode) ?? "";
        }
        public static ushort GetCode(string name)
        {
            return opCodeNamer.GetCode(name);
        }
    }
    static class Logs
    {
        public static bool enabled = false;
        public static bool hexy = false;
    }
    static class WebTeraData
    {
        public readonly static TeraDataPools Pools = new();
    }
}