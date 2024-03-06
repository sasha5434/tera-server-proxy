using Microsoft.AspNetCore.Http.HttpResults;
using Readers;
using System;
using Tera.Game;

namespace Tera.Connection.Dispatcher
{
    public class Dispatch : PacketStruct
    {
        private TeraConnection connection;

        public Dispatch(TeraConnection connection)
        {
            this.connection = connection;
        }
        public void Route(ref Packet packet, bool fromServer)
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
            Write(ref packet, fromServer);
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
        public void Handle(byte[] data, bool fromServer)
        {
            Packet packet = new(connection.userData, opcode: data.ReadUInt16LE(offset: 2), payload: data);
            Route(ref packet, fromServer);
        }

        public void Write(ref Packet packet, bool fromServer)
        {
            if (!packet.skip)
            {
                switch (packet.name[packet.name.IndexOf("TTB_") == 0 ? 4 : 0])
                {
                    case 'S':
                    case 'I': // This case should never happen server will only send S_/TTB_ packets
                        this.connection.sendClient(packet.payload);
                        if (!fromServer)
                        {
                            // Decide how to handle client sending invalid packets ( ie. a user sending a fake S_LOGIN_ARBITER packet to force admin permissions )
                            // Decided to disconnect the user

                            // Log the packet the user tried to fake
                            Log(ref packet, Globals.Logs.hexy);
                            Console.WriteLine($"Close connection for {packet.userData.User.Character.Name} ({packet.userData.User.Character.PlayerId}) - invalid packet!");
                            // Write info about the user in case we didn't manage to get the Name/PlayerId
                            this.connection.logUser();
                            // Close the connection to the user to prevent the exploit
                            this.connection.Close();
                        }
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