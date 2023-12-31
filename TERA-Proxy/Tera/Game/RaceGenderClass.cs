﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tera.Game
{
    // Tera often uses a this tuple of Race, Gender and Class. For example for looking up skills
    public struct RaceGenderClass
    {
        public PlayerRace Race { get; private set; }
        public PlayerGender Gender { get; private set; }
        public PlayerClass Class { get; private set; }

        public int Raw
        {
            get
            {
                if ((byte)Race >= 50 || (byte)Gender >= 2 || (byte)Class >= 100) return 0;
                //throw new InvalidOperationException();
                return 10200 + 200 * (int)Race - 100 * (int)Gender + (int)Class + 1;
            }
            private set
            {
                if (value / 10000 != 1)
                    throw new ArgumentException($"Unexpected raw value for RaceGenderClass {value}");
                Race = (PlayerRace)((value - 100) / 200 % 50);
                Gender = (PlayerGender)(value / 100 % 2);
                Class = (PlayerClass)(value % 100 - 1);
                Debug.Assert(Raw == value);
            }
        }

        private static T ParseEnum<T>(string s)
        {
            return (T)Enum.Parse(typeof(T), s);
        }

        public string GameRace => Race == PlayerRace.Popori && Gender == PlayerGender.Female ? "Elin" : Race.ToString();

        public RaceGenderClass(string race, string gender, string @class)
            : this()
        {
            Race = ParseEnum<PlayerRace>(race);
            Gender = ParseEnum<PlayerGender>(gender);
            Class = ParseEnum<PlayerClass>(@class);
        }

        public RaceGenderClass(PlayerRace race, PlayerGender gender, PlayerClass @class)
            : this()
        {
            Race = race;
            Gender = gender;
            Class = @class;
        }

        public RaceGenderClass(int raw)
            : this()
        {
            Raw = raw;
        }

        public IEnumerable<RaceGenderClass> Fallbacks()
        {
            yield return this;
            yield return new RaceGenderClass(PlayerRace.Common, PlayerGender.Common, Class);
            yield return new RaceGenderClass(Race, Gender, PlayerClass.Common);
            yield return new RaceGenderClass(Race, PlayerGender.Common, PlayerClass.Common);
            yield return new RaceGenderClass(PlayerRace.Common, PlayerGender.Common, PlayerClass.Common);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RaceGenderClass))
                return false;
            var other = (RaceGenderClass)obj;
            return (Race == other.Race) && (Gender == other.Gender) && (Class == other.Class);
        }

        public override int GetHashCode()
        {
            return (int)Race << 16 | (int)Gender << 8 | (int)Class;
        }

        public override string ToString()
        {
            return $"{GameRace} {Gender} {Class}";
        }
    }
}