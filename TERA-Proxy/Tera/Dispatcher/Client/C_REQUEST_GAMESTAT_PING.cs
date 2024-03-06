using Readers;
using static Tera.Connection.Dispatcher.PacketStruct;

namespace Tera.Connection.Dispatcher
{
    public static class C_REQUEST_GAMESTAT_PING
    {
        public static void Hook(Dispatch handler, ref Packet packet)
        {
            packet.skip = true;

            byte[] bytes = new byte[4];
            bytes.WriteUInt16LE(4, 0);
            bytes.WriteUInt16LE(Globals.Opcode.GetCode("S_RESPONSE_GAMESTAT_PONG"), 2);
            var newPacket = new Packet(packet.userData, "S_RESPONSE_GAMESTAT_PONG", payload: bytes);

            handler.Route(ref newPacket, false);
        }
    }
}