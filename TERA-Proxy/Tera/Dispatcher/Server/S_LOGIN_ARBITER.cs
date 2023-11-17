using Readers;
using static Tera.Connection.Dispatcher.PacketStruct;

namespace Tera.Connection.Dispatcher
{
    public static class S_LOGIN_ARBITER
    {
        public static void Hook(Dispatch handler, ref Packet packet)
        {
            if (packet.payload.ReadUInt16LE(6) > 32)
                packet.userData.User.IsAdmin = true;
        }
    }
}