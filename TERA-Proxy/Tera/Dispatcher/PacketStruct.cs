using System;

namespace Tera.Connection.Dispatcher
{
    public class PacketStruct
    {
        public struct Packet
        {
            public Packet(UserData userData, ushort opcode, byte[] payload)
            {
                this.userData = userData;
                this.opcode = opcode;
                this.name = Globals.Opcode.GetName(opcode);
                this.payload = payload;
            }
            public Packet(UserData userData, string name, byte[] payload)
            {
                this.userData = userData;
                this.opcode = Globals.Opcode.GetCode(name);
                this.name = name;
                this.payload = payload;
                this.crafted = true;
            }
            public Packet(Packet packet)
            {
                this.userData = packet.userData;
                this.opcode = packet.opcode;
                this.name = packet.name;
                this.payload = packet.payload;
                this.modified = true;
                this.crafted = true;
                this.skip = packet.skip;
                this.time = packet.time;
            }
            public readonly UserData userData { get; init; }
            public readonly ushort opcode { get; init; }
            public readonly string name { get; init; }
            public byte[] payload { get; set; }
            public bool modified { get; set; } = false;
            public bool crafted { get; set; } = false;
            public bool skip { get; set; } = false;
            public readonly DateTime time { get; init; } = DateTime.Now;
        }
    }
}