using System.Collections.Generic;
using Tera.Connection;
namespace Tera.Game.Structures
{
    public class Party
    {
        public IList<CharacterInfo> Players { get; private set; }
        public bool IsRaid { get; private set; }
        public CharacterInfo Leader { get; private set; }
        public int MaxPlayers => IsRaid ? 30 : 5;

        public Party(CharacterInfo player1, CharacterInfo player2, bool isRaid = false)
        {
            IsRaid = isRaid;
            Players = new List<CharacterInfo>(MaxPlayers) { player1, player2 };
            Leader = player1;
        }
        public Party(CharacterInfo player, bool isRaid = false)
        {
            IsRaid = isRaid;
            Players = new List<CharacterInfo>(MaxPlayers) { player };
            Leader = player;
        }

        public override string ToString()
        {
            return $"Leader: {Leader}, Players count: {Players.Count}";
        }

        //public bool Equals(Party party)
        //{
        //    return Leader.Equals(party.Leader);
        //}
    }
}
