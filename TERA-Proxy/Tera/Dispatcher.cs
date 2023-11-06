using System;
using System.Text;
using System.Buffers.Binary;
using Tera.Opcodes;
using Tools;

namespace Tera.Connection
{
    public class Dispatcher
    {
        public void Dispatch(byte[] data, string socket, bool incoming)
        {
            string opcode = Globals.Global.opCodeNamer.GetName(BinaryPrimitives.ReadUInt16LittleEndian(data));
            byte[] payload = data.Payload(data.Length);
            string arrow = (incoming) ? "<=" : "=>";
            Console.WriteLine($"{socket}   {arrow}   {opcode}\n{payload.ToHex()}\n");
        }
    }
}