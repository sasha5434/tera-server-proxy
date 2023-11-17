using Readers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tera.Game;
using Tera.Game.Structures;
using static Tera.Connection.Dispatcher.PacketStruct;

namespace Tera.Connection.Dispatcher
{
    public static class S_ADD_INTER_PARTY_MATCH_POOL
    {
        public static void Hook(Dispatch handler, ref Packet packet)
        {
            IList<MatchingProfile> Profiles = new List<MatchingProfile>();
            IList<MatchingInstance> Instances = new List<MatchingInstance>();

            var stream = packet.payload.GetStream();
            var reader = stream.GetReader();
            reader.SkipHeader();
            var instanceCount = reader.ReadUInt16();
            var instancePointer = reader.ReadUInt16();
            var playerCount = reader.ReadUInt16();
            var playerPointer = reader.ReadUInt16();

            reader.Skip(4); // estimated wait time - 2 or 4 bytes

            MatchingTypes MatchingType = (MatchingTypes)reader.ReadByte();

            //reader.Skip(2);

            for (var i = 0; i < instanceCount; i++)
            {
                reader.BaseStream.Position = instancePointer;

                var selfPointer = reader.ReadUInt16();
                Debug.Assert(instancePointer == selfPointer);

                instancePointer = reader.ReadUInt16();

                var instanceId = reader.ReadUInt32();
                MatchingInstance instance = MatchingType switch
                {
                    MatchingTypes.Dungeon => InstanceManager.GetDungeon(instanceId),
                    MatchingTypes.Battleground => InstanceManager.GetBattleground(instanceId),
                    _ => throw new MatchingTypesInvalidEnumArgumentException(MatchingType)
                };

                Instances.Add(instance);
            }

            for (var i = 0; i < playerCount; i++)
            {
                reader.BaseStream.Position = playerPointer;

                var selfPointer = reader.ReadUInt16();
                Debug.Assert(playerPointer == selfPointer);

                playerPointer = reader.ReadUInt16();
                var namePointer = reader.ReadUInt16();
                var isLeader = reader.ReadBoolean();
                var role = (PlayerPartyRoles)reader.ReadUInt16();

                //reader.Skip(2);

                reader.BaseStream.Position = namePointer;
                var name = reader.ReadTeraString();

                Profiles.Add(new MatchingProfile(name, isLeader, role));
            }

            if (Profiles.First().Name != packet.userData.User.Character.Name)
                return;
            foreach (var profile in Profiles)
            {
                profile.LinkedPlayer = packet.userData.User.Character;
            }
            var partyMatching = new PartyMatching(Profiles, Instances, MatchingType);
            Globals.WebTeraData.Pools.Add(partyMatching);
            if (Globals.Logs.enabled)
                Console.WriteLine($"{packet.userData.Socket}|Added PartyMatching: {partyMatching}.");
        }
    }
}