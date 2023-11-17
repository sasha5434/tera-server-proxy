using System;
using System.Collections.Generic;
using Tera.Game.Structures;

namespace TeraPartyMonitor.DataSender.Models
{
    [Serializable]
    public class BattlegroundMatchingModel : PartyMatchingModel
    {
        public Battleground Battleground { get; set; }

        public BattlegroundMatchingModel(IEnumerable<IList<MatchingProfile>> profiles, Battleground battleground)
            : base(profiles)
        {
            Battleground = battleground;
        }
    }
}
