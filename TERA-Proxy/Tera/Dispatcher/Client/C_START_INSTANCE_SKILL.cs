using Readers;
using System;
using Tera.Game;
using static Tera.Connection.Dispatcher.PacketStruct;

namespace Tera.Connection.Dispatcher
{
    public static class C_START_INSTANCE_SKILL
    {
        public static void Hook(Dispatch handler, ref Packet packet)
        {
            using var stream = packet.payload.GetStream();
            using var reader = stream.GetReader();
            reader.Skip(12);
            var skillId = reader.ReadUInt16();
            var group = Math.Floor((decimal)(skillId / 10000));
            packet.skip = CheckGroup(packet.userData, group);
            if (packet.skip)
                Console.WriteLine($"Skip C_START_INSTANCE_SKILL for {packet.userData.User.Character.Name} ({packet.userData.User.Character.PlayerId})! Skill: {skillId}");
        }
        private static bool CheckGroup(UserData user, decimal group)
        {
            switch (user.User.Character.Class)
            {
                case PlayerClass.Archer:
                    switch (group)
                    {
                        case 1:
                        case 3:
                        case 4:
                        case 12:
                        case 17:
                        case 19:
                        case 20:
                        case 22:
                        case 24:
                        case 32:
                        case 34:
                            return false;
                    }
                    break;
                case PlayerClass.Ninja:
                    switch (group)
                    {
                        case 20:
                            return false;
                    }
                    break;
                case PlayerClass.Brawler:
                    switch (group)
                    {
                        case 3:
                            return false;
                    }
                    break;
                case PlayerClass.Lancer:
                    switch (group)
                    {
                        case 24:
                            return false;
                    }
                    break;
                case PlayerClass.Slayer:
                    switch (group)
                    {
                        case 21:
                            return false;
                    }
                    break;
                case PlayerClass.Sorcerer:
                    switch (group)
                    {
                        case 1:
                        case 19:
                            return false;
                    }
                    break;
            }
            return true;
        }
    }
}