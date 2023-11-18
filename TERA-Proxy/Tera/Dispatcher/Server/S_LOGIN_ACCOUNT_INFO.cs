using Readers;
using static Tera.Connection.Dispatcher.PacketStruct;

namespace Tera.Connection.Dispatcher
{
    public static class S_LOGIN_ACCOUNT_INFO
    {
        public static void Hook(Dispatch handler, ref Packet packet)
        {
            using var stream = packet.payload.GetStream();
            using var reader = stream.GetReader();
            reader.Skip(6);
            packet.userData.User.Id = reader.ReadUInt64LE();
        }
    }
}