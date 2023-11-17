using Readers;
using Tera.Game;
using static Tera.Connection.Dispatcher.PacketStruct;

namespace Tera.Connection.Dispatcher
{
    public static class S_USER_CHANGE_NAME
    {
        public static void Hook(Dispatch handler, ref Packet packet)
        {
            var stream = packet.payload.GetStream();
            var reader = stream.GetReader();
            reader.SkipHeader();
            int nameOffset = reader.ReadInt16();
            var EntityId = reader.ReadEntityId();
            if (packet.userData.User.Character.GameId == EntityId.Id)
            {
                reader.BaseStream.Position = nameOffset;
                packet.userData.User.Character.Name = reader.ReadTeraString();
            }
        }
    }
}