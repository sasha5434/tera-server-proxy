namespace Tera.Game
{
    public enum PlayerRace : byte
    {
        Human = 0,
        Highelf = 1,
        Aman = 2,
        Castanic = 3,
        Popori = 4, // Male=Popori, Female = Elin
        Baraka = 5,

        Common = 99
    }
    public enum PlayerClass : byte
    {
        Warrior = 0,
        Lancer = 1,
        Slayer = 2,
        Berserker = 3,
        Sorcerer = 4,
        Archer = 5,
        Priest = 6,
        Mystic = 7,
        Reaper = 8,
        Gunner = 9,
        Brawler = 10,
        Ninja = 11,
        Valkyrie = 12,

        Common = 99
    }
    public enum PlayerGender : byte
    {
        Female = 0,
        Male = 1,

        Common = 99 // Used in the skill database
    }
    public enum PlayerPartyRoles : byte
    {
        Tank,
        DamageDealer,
        Healer
    }
}