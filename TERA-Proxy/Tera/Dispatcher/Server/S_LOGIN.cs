using Readers;
using Tera.Game;
using static Tera.Connection.Dispatcher.PacketStruct;

namespace Tera.Connection.Dispatcher
{
    public static class S_LOGIN
    {
        public static void Hook(Dispatch handler, ref Packet packet)
        {
            packet.userData.User.InGame = true;

            using var stream = packet.payload.GetStream();
            using var reader = stream.GetReader();
            reader.Skip(8); //size + opcode + 4
            int nameOffset = reader.ReadInt16();
            reader.Skip(8);
            var RaceGenderClass = new RaceGenderClass(reader.ReadInt32());
            packet.userData.User.Character.Class = RaceGenderClass.Class;
            packet.userData.User.Character.Race = RaceGenderClass.Race;
            packet.userData.User.Character.Gender = RaceGenderClass.Gender;
            var EntityId = reader.ReadEntityId();
            packet.userData.User.Character.GameId = EntityId.Id;
            packet.userData.User.Character.ServerId = reader.ReadUInt32();
            packet.userData.User.Character.PlayerId = reader.ReadUInt32();
            reader.Skip(27);
            packet.userData.User.Character.Level = (byte)reader.ReadInt16();
            reader.BaseStream.Position = nameOffset;
            packet.userData.User.Character.Name = reader.ReadTeraString();

            var player = Globals.WebTeraData.Pools.GetPlayerByName(packet.userData.User.Character.Name);
            if (player != null)
                Globals.WebTeraData.Pools.Remove(player);
            Globals.WebTeraData.Pools.Add(packet.userData.User.Character);
        }
    }
}