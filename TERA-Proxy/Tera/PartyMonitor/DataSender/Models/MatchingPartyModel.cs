using System;
using System.Collections.Generic;
using Tera.Game.Structures;

namespace TeraPartyMonitor.DataSender.Models
{
    [Serializable]
    public class MatchingPartyModel
    {
        //public int PartyId { get; set; }
        public IList<MatchingProfile> Players { get; set; }

    }
}
