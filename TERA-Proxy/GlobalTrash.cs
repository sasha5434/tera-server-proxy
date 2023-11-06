using Tera.Opcodes;

namespace Globals
{
    class Global
    {
        public static OpCodeNamer opCodeNamer;

        public static void initOpCodeNamer (int protocol)
        {
            string filename = $"data/protocol.{protocol}.map";
            opCodeNamer = new OpCodeNamer(filename);
        }
    }
}