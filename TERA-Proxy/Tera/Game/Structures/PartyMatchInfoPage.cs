using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Tera.Game.Structures
{
    public class PartyMatchInfoPage
    {
        [JsonIgnore]
        public int Page { get; init; }
        public IEnumerable<PartyMatchInfo> Messages { get; init; }
        public DateTime LastUpdate { get; init; }

        public PartyMatchInfoPage(int page, IEnumerable<PartyMatchInfo> infos, DateTime time)
        {
            Page = page;
            Messages = infos;
            LastUpdate = time;
        }
    }
}
