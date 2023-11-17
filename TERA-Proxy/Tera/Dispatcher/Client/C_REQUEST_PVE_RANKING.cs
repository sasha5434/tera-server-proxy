using Readers;
using static Tera.Connection.Dispatcher.PacketStruct;

namespace Tera.Connection.Dispatcher
{
    public static class C_REQUEST_PVE_RANKING
    {
        public static void Hook(Dispatch handler, ref Packet packet)
        {
            var stream = packet.payload.GetStream();
            var reader = stream.GetReader();
            reader.Skip(12);
            uint playerClass = reader.ReadUInt32();
            if (playerClass > 12 && playerClass != 15)
                packet.skip = true;
        }
    }
}