using System;
using System.Diagnostics;
using System.Linq;
using Tera.Connection;
using static Tera.Connection.Dispatcher.PacketStruct;

namespace Tera.Mods.Controlers.AntiCheat.AutoLocks
{
    public class AutoLock
    {
        public Stopwatch Stopwatch = new Stopwatch();

        TimeSpan timeSinceLastLock;

        // Config
        private int minimumDelay = 80;
        private int[] fastLockSkills =
        {
            53000,
            53100,
            193000,
            193100
        };

        public void cCanLockonTarget_handler(C_CAN_LOCKON_TARGET _event, ref Packet packet)
        {
            // in case of the skill advancement the delay is 0ms by default
            if(fastLockSkills.Contains(_event.skillId.Id))
                return;

            if (!Stopwatch.IsRunning)
            {
                Stopwatch.Start();
                timeSinceLastLock = Stopwatch.Elapsed;
                return;
            }

            TimeSpan tempSpan = Stopwatch.Elapsed;
            TimeSpan diff = tempSpan - timeSinceLastLock;

            if (diff.TotalMilliseconds < minimumDelay)
            {
                AntiCheat.Loader.AntiCheatLogger("Auto Lock", packet.userData);
                packet.skip = true;
            }
            timeSinceLastLock = tempSpan;
        }
    }
}