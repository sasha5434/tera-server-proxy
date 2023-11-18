using Readers;
using System;
using System.Linq;
using Tera.Game.Structures;
using static Tera.Connection.Dispatcher.PacketStruct;

namespace Tera.Connection.Dispatcher
{
    public static class S_DEL_INTER_PARTY_MATCH_POOL
    {
        public static void Hook(Dispatch handler, ref Packet packet)
        {
            using var stream = packet.payload.GetStream();
            using var reader = stream.GetReader();
            reader.SkipHeader();
            var MatchingType = (MatchingTypes)reader.ReadByte();

            PartyMatching partyMatching1, partyMatching2 = null;

            switch (MatchingType)
            {
                case MatchingTypes.Dungeon:
                case MatchingTypes.Battleground:
                    partyMatching1 = Globals.WebTeraData.Pools.GetPartyMatchingByPlayer(packet.userData.User.Character, MatchingType);
                    break;
                case MatchingTypes.All:
                    partyMatching1 = Globals.WebTeraData.Pools.GetPartyMatchingByPlayer(packet.userData.User.Character, MatchingTypes.Dungeon);
                    partyMatching2 = Globals.WebTeraData.Pools.GetPartyMatchingByPlayer(packet.userData.User.Character, MatchingTypes.Battleground);
                    break;
                default:
                    throw new MatchingTypesInvalidEnumArgumentException(MatchingType);
            }

            void TryRemove(PartyMatching partyMatching, CharacterInfo player, string socket)
            {
                if (partyMatching == null)
                    return;

                if (!partyMatching.MatchingProfiles.First().LinkedPlayer.Equals(player))
                    return;

                Globals.WebTeraData.Pools.Remove(partyMatching);
                if (Globals.Logs.enabled)
                    Console.WriteLine($"{socket}|Removed PartyMatching: {partyMatching}.");
            }

            TryRemove(partyMatching1, packet.userData.User.Character, packet.userData.Socket);
            TryRemove(partyMatching2, packet.userData.User.Character, packet.userData.Socket);
        }
    }
}