using static Tera.Connection.Dispatcher.PacketStruct;

namespace Tera.Connection.Dispatcher
{
    public static class C_ADMIN
    {
        public static void Hook(Dispatch handler, ref Packet packet)
        {
            if (!packet.userData.User.IsAdmin)
                packet.skip = true;
        }
    }
}