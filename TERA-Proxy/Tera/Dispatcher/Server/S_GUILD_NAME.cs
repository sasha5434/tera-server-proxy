using Readers;
using static Tera.Connection.Dispatcher.PacketStruct;

namespace Tera.Connection.Dispatcher
{
    public static class S_GUILD_NAME
    {
        public static void Hook(Dispatch handler, ref Packet packet)
        {
            using var stream = packet.payload.GetStream();
            using var reader = stream.GetReader();

            reader.SkipHeader();
            reader.Skip(8);
            var gameId = reader.ReadEntityId();
            if (packet.userData.User.Character.GameId == gameId.Id)
            {
                var name = reader.ReadTeraString();
                packet.userData.User.Character.GuildName = name != "" ? name : null;
            }
        }
    }
}