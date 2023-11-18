using Readers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tera.Game.Structures;
using static Tera.Connection.Dispatcher.PacketStruct;

namespace Tera.Connection.Dispatcher
{
    public static class S_MODIFY_INTER_PARTY_MATCH_POOL
    {
        public static void Hook(Dispatch handler, ref Packet packet)
        {
            IList<(string, bool)> Modifiers = new List<(string, bool)>();

            using var stream = packet.payload.GetStream();
            using var reader = stream.GetReader();
            reader.SkipHeader();
            var playerCount = reader.ReadUInt16();
            var playerPointer = reader.ReadUInt16();

            //reader.Skip(4); // ??
            //reader.Skip(4); // estimated wait time - 2 or 4 bytes
            //reader.Skip(4); // ??

            for (var i = 0; i < playerCount; i++)
            {
                reader.BaseStream.Position = playerPointer;

                var selfPointer = reader.ReadUInt16();
                Debug.Assert(playerPointer == selfPointer);

                playerPointer = reader.ReadUInt16();
                var namePointer = reader.ReadUInt16();
                var isLeader = reader.ReadBoolean();

                reader.BaseStream.Position = namePointer;
                var name = reader.ReadTeraString();
                Modifiers.Add((name, isLeader));
            }

            var partyMatching1 = Globals.WebTeraData.Pools.GetPartyMatchingByPlayer(packet.userData.User.Character, MatchingTypes.Dungeon);
            var partyMatching2 = Globals.WebTeraData.Pools.GetPartyMatchingByPlayer(packet.userData.User.Character, MatchingTypes.Battleground);

            void TryModify(PartyMatching oldPartyMatching, CharacterInfo player, IList<(string, bool)> modifiers, string socket)
            {
                if (oldPartyMatching == null)
                    return;

                if (!oldPartyMatching.MatchingProfiles.First().LinkedPlayer.Equals(player))
                    return;

                var profiles = new List<MatchingProfile>();

                foreach ((var name, var isLeader) in modifiers)
                {
                    var profile = oldPartyMatching.MatchingProfiles.Single(p => p.Name.Equals(name));
                    var role = profile.Role;
                    var linkedPlayer = profile.LinkedPlayer;

                    profiles.Add(new MatchingProfile(name, isLeader, role));
                }

                var newPartyMatching = new PartyMatching(profiles, oldPartyMatching.Instances, oldPartyMatching.MatchingType);
                Globals.WebTeraData.Pools.Replace(oldPartyMatching, newPartyMatching);
                if (Globals.Logs.enabled)
                    Console.WriteLine($"{socket}|Modified PartyMatching: {oldPartyMatching} -> {newPartyMatching}.");
            }

            TryModify(partyMatching1, packet.userData.User.Character, Modifiers, packet.userData.Socket);
            TryModify(partyMatching2, packet.userData.User.Character, Modifiers, packet.userData.Socket);
        }
    }
}