using System;
using System.Text.Json.Serialization;
using Tera.Connection;

namespace Tera.Game.Structures
{
    [Serializable]
    public class MatchingProfile
    {
        private readonly string _name;
        public string Name => LinkedPlayer?.Name ?? _name;
        public bool IsLeaderRequired { get; init; }

        //[JsonConverter(typeof(JsonStringEnumConverter))]
        public PlayerPartyRoles Role { get; init; }

        [JsonIgnore]
        public CharacterInfo LinkedPlayer { get; set; }

        //[JsonConverter(typeof(JsonStringEnumConverter))]
        public PlayerClass? Class => LinkedPlayer?.Class;
        public int? Level => LinkedPlayer?.Level;
        public uint? PlayerId => LinkedPlayer?.PlayerId;

        public MatchingProfile(string name, bool isLeaderRequired, PlayerPartyRoles role)
        {
            _name = name;
            IsLeaderRequired = isLeaderRequired;
            Role = role;
        }

        public override string ToString()
        {
            var pl = IsLeaderRequired ? "[PL]" : "";
            return $"{Name} ({Role}) {pl}";
        }
        public string ToString(bool withoutName)
        {
            if (!withoutName)
                return ToString();

            var pl = IsLeaderRequired ? "[PL]" : "";
            return $"{Role} {pl}";
        }
    }
}