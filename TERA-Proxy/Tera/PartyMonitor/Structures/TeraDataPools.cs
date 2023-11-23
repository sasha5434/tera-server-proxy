using System;
using System.Collections.Generic;
using System.Linq;
using Tera.Connection;
using Tera.Game.Structures;

namespace TeraPartyMonitor.Structures
{
    internal class TeraDataPools
    {
        protected TeraDataPool<CharacterInfo> PlayerCollection { get; init; }
        protected TeraDataPool<Party> PartyCollection { get; init; }
        protected TeraDataPool<PartyMatching> PartyMatchingCollection { get; init; }
        //protected TeraDataPool<Player> CachedPlayers { get; init; }

        private Dictionary<Type, object> lockers = new()
        {
            { typeof(CharacterInfo), new() },
            { typeof(Party), new() },
            { typeof(PartyMatching), new() }
        };

        public TeraDataPools(int capacity = 64)
        {
            PlayerCollection = new(capacity);
            PartyCollection = new(capacity);
            PartyMatchingCollection = new(capacity);

            PlayerCollection.ItemRemoved += PlayerCollection_ItemRemoved;
        }

        #region Event Handlers

        //private void PartyMatchingCollection_ItemChanged(PartyMatching matching1, PartyMatching matching2)
        //{
        //    PartyMatchingCollectionChanged?.Invoke(PartyMatchingCollection.AsReadOnly(), matching1.MatchingType);
        //}

        //private void PartyMatchingCollection_ItemAdded(PartyMatching matching)
        //{
        //    PartyMatchingCollectionChanged?.Invoke(PartyMatchingCollection.AsReadOnly(), matching.MatchingType);
        //}

        //private void PartyMatchingCollection_ItemRemoved(PartyMatching matching)
        //{
        //    PartyMatchingCollectionChanged?.Invoke(PartyMatchingCollection.AsReadOnly(), matching.MatchingType);
        //}

        private void PlayerCollection_ItemRemoved(CharacterInfo player)
        {
            var dgMatching = GetPartyMatchingByPlayer(player, MatchingTypes.Dungeon);
            var bgMatching = GetPartyMatchingByPlayer(player, MatchingTypes.Battleground);

            if (dgMatching != null)
                Remove(dgMatching, player.Name);

            if (bgMatching != null)
                Remove(bgMatching, player.Name);

            //var party = GetPartyByPlayer(player);
            //if (party == null)
            //    return;

            //if (party.Players.Count > 1)
            //    return;

            //Remove(party);

            //CachedPlayers.Add(player);
        }

        #endregion

        #region Public Methods

        public IReadOnlyCollection<PartyMatching> GetPartyMatchings()
        {
            // remove outdated matchings
            if (PartyMatchingCollection.Any())
            {
                var forRemoving = PartyMatchingCollection.Where(m => m.IsRemoveRequested());

                foreach (var r in forRemoving)
                    Remove(PartyMatchingCollection, r);
            }
            return PartyMatchingCollection.AsReadOnly();
        }
        public IReadOnlyCollection<CharacterInfo> GetPlayers()
        {
            return PlayerCollection.AsReadOnly();
        }

        public Party GetPartyByPlayer(CharacterInfo player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            try
            {
                return PartyCollection.SingleOrDefault(p => p.Players.Contains(player));
            }
            catch
            {
                throw new Exception($"Player ({player}) is in more than one party");
            }
        }

        public Party GetOrCreatePartyByPlayer(CharacterInfo player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            try
            {
                var party = PartyCollection.SingleOrDefault(p => p.Players.Contains(player));
                if (party == null)
                {
                    party = new Party(player);
                    Add(party);
                }
                return party;
            }
            catch
            {
                throw new Exception($"Player ({player}) is in more than one party");
            }
        }

        public PartyMatching GetPartyMatchingByPlayer(CharacterInfo player, MatchingTypes type)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            try
            {
                return PartyMatchingCollection.SingleOrDefault(pm => pm.MatchingType == type &&
                    pm.MatchingProfiles.Any(prof => prof.Name.Equals(player.Name)));
            }
            catch
            {
                throw new Exception($"Player ({player}) is in more than one PartyMatching ({type})");
            }
        }

        public CharacterInfo GetPlayerByName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            try
            {
                return PlayerCollection.SingleOrDefault(p => p.Name.Equals(name));
            }
            catch
            {
                throw new Exception($"There are more than one player with the same name: {name}");
            }
        }

        public void Add(CharacterInfo player)
        {
            //var old = GetPlayerByName(player.Name);
            //if (old != null)
            //    Replace(old, player);

            Add(PlayerCollection, player);
        }

        public void Add(Party party)
        {
            Add(PartyCollection, party);
        }

        public void Add(PartyMatching partyMatching)
        {
            // check for duplicates before adding
            var names = partyMatching.MatchingProfiles.Select(p => p.Name);
            var forRemoving = PartyMatchingCollection
                .Where(m => m.MatchingType == partyMatching.MatchingType && m.MatchingProfiles.Any(p => names.Contains(p.Name)));

            foreach (var r in forRemoving)
                Remove(PartyMatchingCollection, r);

            Add(PartyMatchingCollection, partyMatching);
        }

        public void Remove(CharacterInfo player)
        {
            Remove(PlayerCollection, player);
        }

        public void Remove(Party party)
        {
            Remove(PartyCollection, party);
        }

        public void Remove(PartyMatching partyMatching, string removeBy)
        {
            partyMatching.RequestRemove(removeBy);

            if (partyMatching.IsRemovable)
                Remove(PartyMatchingCollection, partyMatching);
        }

        public void Replace(PartyMatching oldPartyMatching, PartyMatching newPartyMatching)
        {
            Replace(PartyMatchingCollection, oldPartyMatching, newPartyMatching);
        }

        #endregion

        private void Add<T>(TeraDataPool<T> pool, T item)
        {
            lock (lockers[typeof(T)])
            {
                pool.Add(item);
            }
        }
        private void Remove<T>(TeraDataPool<T> pool, T item)
        {
            lock (lockers[typeof(T)])
            {
                pool.Remove(item);
            }
        }
        private void Replace<T>(TeraDataPool<T> pool, T oldItem, T newItem)
        {
            lock (lockers[typeof(T)])
            {
                var id = pool.IndexOf(oldItem);
                pool[id] = newItem;
            }
        }
    }
}
