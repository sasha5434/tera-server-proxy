using System;
using System.Collections.Generic;
using Tera.Game.Structures;

namespace TeraPartyMonitor.DataSender.Models
{
    [Serializable]
    public class DungeonMatchingModel : PartyMatchingModel
    {
        public Dungeon Dungeon { get; set; }

        public DungeonMatchingModel(IEnumerable<IList<MatchingProfile>> profiles, Dungeon dungeon)
            : base(profiles)
        {
            Dungeon = dungeon;
        }
    }
}
