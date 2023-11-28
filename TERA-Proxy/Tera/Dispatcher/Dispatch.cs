using Readers;
using System;

namespace Tera.Connection.Dispatcher
{
    public class Dispatch : PacketStruct
    {
        private TeraConnection connection;

        public Dispatch(TeraConnection connection)
        {
            this.connection = connection;
        }
        public void Route(ref Packet packet)
        {
            try
            {
                switch (packet.name)
                {
                    case "S_LOGIN_ARBITER":
                        S_LOGIN_ARBITER.Hook(this, ref packet);
                        break;
                    case "S_LOGIN_ACCOUNT_INFO":
                        S_LOGIN_ACCOUNT_INFO.Hook(this, ref packet);
                        break;
                    case "S_LOGIN":
                        S_LOGIN.Hook(this, ref packet);
                        break;
                    case "S_USER_LEVELUP":
                        S_USER_LEVELUP.Hook(this, ref packet);
                        break;
                    case "S_USER_CHANGE_NAME":
                        S_USER_CHANGE_NAME.Hook(this, ref packet);
                        break;
                    case "S_RETURN_TO_LOBBY":
                        S_RETURN_TO_LOBBY.Hook(this, ref packet);
                        break;
                    case "S_EXIT":
                        S_EXIT.Hook(this, ref packet);
                        break;
                    case "S_ADD_INTER_PARTY_MATCH_POOL":
                        S_ADD_INTER_PARTY_MATCH_POOL.Hook(this, ref packet);
                        break;
                    case "S_DEL_INTER_PARTY_MATCH_POOL":
                        S_DEL_INTER_PARTY_MATCH_POOL.Hook(this, ref packet);
                        break;
                    case "S_MODIFY_INTER_PARTY_MATCH_POOL":
                        S_MODIFY_INTER_PARTY_MATCH_POOL.Hook(this, ref packet);
                        break;
                    case "S_SHOW_PARTY_MATCH_INFO":
                        S_SHOW_PARTY_MATCH_INFO.Hook(this, ref packet);
                        break;
                    case "C_ADMIN":
                        C_ADMIN.Hook(this, ref packet);
                        break;
                    case "C_REQUEST_GAMESTAT_PING":
                        C_REQUEST_GAMESTAT_PING.Hook(this, ref packet);
                        break;
                    case "C_REQUEST_PVE_RANKING":
                        C_REQUEST_PVE_RANKING.Hook(this, ref packet);
                        break;
                    case "C_REQUEST_PVP_RANKING":
                        C_REQUEST_PVP_RANKING.Hook(this, ref packet);
                        break;
                    case "C_START_INSTANCE_SKILL":
                        C_START_INSTANCE_SKILL.Hook(this, ref packet);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception thrown during {packet.name}.Hook() execution:\n{e.Message}\n{e.StackTrace}");
            }
            if (Globals.Logs.enabled)
                Log(ref packet, Globals.Logs.hexy);
            Write(ref packet);
        }
        public static void Log(ref Packet packet, bool hexy = false)
        {
            string arrow = (packet.name[0] == 'S') ? "<=" : "=>";
            string modified = (packet.modified) ? " modified" : "";
            string crafted = (packet.crafted) ? " crafted" : "";
            string skip = (packet.skip) ? " skip" : "";
            if (hexy)
                Console.WriteLine($"[{packet.time.ToLongTimeString()}:{packet.time.Millisecond}] {packet.userData.Socket}   {arrow}   {packet.name} ({packet.opcode}){modified}{crafted}{skip}\n{packet.payload.ToHex()}\n");
            else
                Console.WriteLine($"[{packet.time.ToLongTimeString()}:{packet.time.Millisecond}] {packet.userData.Socket}   {arrow}   {packet.name} ({packet.opcode}){modified}{crafted}{skip}");
        }
        public void Handle(byte[] data)
        {
            Packet packet = new(connection.userData, opcode: data.ReadUInt16LE(offset: 2), payload: data);
            Route(ref packet);
        }

        public void Write(ref Packet packet)
        {
            if (!packet.skip)
            {
                switch (packet.name[packet.name.IndexOf("TTB_") == 0 ? 4 : 0])
                {
                    case 'S':
                    case 'I':
                        this.connection.sendClient(packet.payload);
                        break;
                    case 'C':
                        this.connection.sendServer(packet.payload);
                        break;
                    default:
                        Console.WriteLine($"Unknown packet direction: {packet.name}");
                        break;
                }
            }
        }
    }
}