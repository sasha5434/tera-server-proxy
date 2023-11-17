using static Tera.Connection.Dispatcher.PacketStruct;

namespace Tera.Connection.Dispatcher
{
    public static class S_RETURN_TO_LOBBY
    {
        public static void Hook(Dispatch handler, ref Packet packet)
        {
            packet.userData.User.InGame = false;
            Globals.WebTeraData.Pools.Remove(packet.userData.User.Character);
        }
    }
}