namespace Tera.Game.Structures
{
    public class PartyMatchInfo
    {
        public uint LeaderId { get; init; }
        public string LeaderName { get; init; }
        public bool IsRaid { get; init; }
        public int PlayerCount { get; init; }
        public string Message { get; init; }
        public int MaxPlayers => IsRaid ? 30 : 5;

        public PartyMatchInfo(uint id, string name, bool isRaid, int count, string message)
        {
            LeaderId = id;
            LeaderName = name;
            IsRaid = isRaid;
            PlayerCount = count;
            Message = message;
        }

        public override string ToString()
        {
            return $"PL: {LeaderName} | Message: {Message} | Players: {PlayerCount}/{MaxPlayers}";
        }
    }
}
