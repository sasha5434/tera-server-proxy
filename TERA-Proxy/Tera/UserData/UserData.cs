using System.Text.Json.Serialization;
using Tera.Game;

namespace Tera.Connection
{
    public class UserData
    {
        public UserInfo User { get; init; } = new();
        public string Socket { get; init; }
        public UserData(string Socket)
        {
            this.Socket = Socket;
        }
    }
    public class UserInfo
    {
        public ulong Id { get; set; }
        public bool InGame { get; set; } = false;
        public bool IsAdmin { get; set; } = false;
        public CharacterInfo Character { get; init; } = new();
    }
    public class CharacterInfo
    {
        [JsonIgnore]
        public ulong GameId { get; set; }
        public uint ServerId { get; set; }
        public uint PlayerId { get; set; }
        public string Name { get; set; }
        public byte Level { get; set; }
        //[JsonConverter(typeof(JsonStringEnumConverter))]
        public PlayerRace Race { get; set; }
        //[JsonConverter(typeof(JsonStringEnumConverter))]
        public PlayerClass Class { get; set; }
        //[JsonConverter(typeof(JsonStringEnumConverter))]
        public PlayerGender Gender { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Class} {Level} lvl)";
        }
    }
}