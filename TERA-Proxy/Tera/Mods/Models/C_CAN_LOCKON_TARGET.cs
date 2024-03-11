using static Tera.Connection.Dispatcher.PacketStruct;
using Tera.Connection.Dispatcher;
using Tera.Game;
using Readers;

public class C_CAN_LOCKON_TARGET
{
    public long target;
    public int cylinderId;
    public SkillId skillId;
    public C_CAN_LOCKON_TARGET(ref Packet packet)
    {
        using var stream = packet.payload.GetStream();
        using var reader = stream.GetReader();
        reader.Skip(4); //size + opcode

        target = reader.ReadInt64();
        cylinderId = reader.ReadInt32();
        skillId = new SkillId(reader, 9204);
    }
}