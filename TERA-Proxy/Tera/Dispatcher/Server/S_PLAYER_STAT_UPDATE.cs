using Readers;
using static Tera.Connection.Dispatcher.PacketStruct;

namespace Tera.Connection.Dispatcher
{
    public static class S_PLAYER_STAT_UPDATE
    {
        public static void Hook(Dispatch handler, ref Packet packet)
        {
            using var stream = packet.payload.GetStream();
            using var reader = stream.GetReader();

            reader.SkipHeader();
            reader.Skip(271);
            packet.userData.User.Character.ItemLevel = reader.ReadSingle();
        }
    }
}