using Readers;
using System.Diagnostics;
using System.Collections.Generic;
using Tera.Game.Structures;
using static Tera.Connection.Dispatcher.PacketStruct;

namespace Tera.Connection.Dispatcher
{
    public static class S_SHOW_PARTY_MATCH_INFO
    {
        public static void Hook(Dispatch handler, ref Packet packet)
        {
            var lfgList = new List<PartyMatchInfo>();

            using var stream = packet.payload.GetStream();
            using var reader = stream.GetReader();

            reader.SkipHeader();
            var infoCount = reader.ReadInt16();
            var infoPointer = reader.ReadUInt16();
            var currentPage = reader.ReadInt16(); // zero based
            var pageCount = reader.ReadInt16(); // zero based

            for (var i = 0; i < infoCount; i++)
            {
                reader.BaseStream.Position = infoPointer;

                var selfPointer = reader.ReadUInt16();
                Debug.Assert(infoPointer == selfPointer);

                infoPointer = reader.ReadUInt16();
                var msgPointer = reader.ReadUInt16();
                var namePointer = reader.ReadUInt16();

                var leaderId = reader.ReadUInt32();
                var isRaid = reader.ReadBoolean();
                var playerCount = reader.ReadInt16();

                reader.BaseStream.Position = msgPointer;
                var message = reader.ReadTeraString();

                reader.BaseStream.Position = namePointer;
                var leaderName = reader.ReadTeraString();

                lfgList.Add(new PartyMatchInfo(leaderId, leaderName, isRaid, playerCount, message));
            }

            var oldInfoPage = Globals.WebTeraData.Pools.GetPartyMatchInfoPageByPage(currentPage);
            if (oldInfoPage != null)
            {
                Globals.WebTeraData.Pools.Remove(oldInfoPage);
            }
            if (infoCount > 0)
            {
                Globals.WebTeraData.Pools.Add(new PartyMatchInfoPage(currentPage, lfgList, packet.time));
            }
        }
    }
}