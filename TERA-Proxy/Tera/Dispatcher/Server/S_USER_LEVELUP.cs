using Readers;
using Tera.Game;
using static Tera.Connection.Dispatcher.PacketStruct;

namespace Tera.Connection.Dispatcher
{
    public static class S_USER_LEVELUP
    {
        public static void Hook(Dispatch handler, ref Packet packet)
        {
            var stream = packet.payload.GetStream();
            var reader = stream.GetReader();
            reader.SkipHeader();
            var EntityId = reader.ReadEntityId();
            if (packet.userData.User.Character.GameId == EntityId.Id)
                packet.userData.User.Character.Level = (byte)reader.ReadInt16();
        }
    }
}