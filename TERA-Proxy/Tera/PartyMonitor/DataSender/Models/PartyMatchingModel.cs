using System;
using System.Collections.Generic;
using Tera.Game.Structures;

namespace TeraPartyMonitor.DataSender.Models
{
    [Serializable]
    public class PartyMatchingModel
    {
        public IList<MatchingPartyModel> Parties { get; set; }

        public PartyMatchingModel(IEnumerable<IList<MatchingProfile>> profiles)
        {
            Parties = new List<MatchingPartyModel>();
            foreach (var p in profiles)
            {
                Parties.Add(new MatchingPartyModel()
                {
                    Players = p
                });
            }
        }
    }
}