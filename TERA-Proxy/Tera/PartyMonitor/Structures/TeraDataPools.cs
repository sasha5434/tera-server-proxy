using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Tera.Connection;
using Tera.Game.Structures;

namespace TeraPartyMonitor.Structures
{
    internal class TeraDataPools
    {
        protected TeraDataPool<CharacterInfo> PlayerCollection { get; init; }
        //protected TeraDataPool<Party> PartyCollection { get; init; }
        protected TeraDataPool<PartyMatching> PartyMatchingCollection { get; init; }
        protected TeraDataPool<PartyMatchInfoPage> PartyMatchInfoPageCollection { get; init; }

        private Dictionary<Type, object> lockers = new()
        {
            { typeof(CharacterInfo), new() },
            //{ typeof(Party), new() },
            { typeof(PartyMatching), new() },
            { typeof(PartyMatchInfoPage), new() }
        };

        public TeraDataPools(int capacity = 64)
        {
            PlayerCollection = new(capacity);
            //PartyCollection = new(capacity);
            PartyMatchingCollection = new(capacity);
            PartyMatchInfoPageCollection = new(capacity);

            PlayerCollection.ItemRemoved += PlayerCollection_ItemRemoved;
        }

        #region Event Handlers

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
        }

        #endregion

        #region Public Methods

        public IReadOnlyCollection<PartyMatching> GetPartyMatchings()
        {
            // remove outdated matchings
            var collection = GetReadOnly(PartyMatchingCollection);
            if (collection.Count != 0)
            {
                var forRemoving = collection.Where(m => m.IsRemoveRequested());

                if (forRemoving.Any())
                {
                    foreach (var r in forRemoving)
                        Remove(PartyMatchingCollection, r);

                    return GetReadOnly(PartyMatchingCollection);
                }
            }

            return collection;
        }
        public IReadOnlyCollection<CharacterInfo> GetPlayers()
        {
            return GetReadOnly(PlayerCollection);
        }
        public IReadOnlyCollection<PartyMatchInfoPage> GetPartyMatchInfoPages()
        {
            return GetReadOnly(PartyMatchInfoPageCollection);
        }

        //public Party GetPartyByPlayer(CharacterInfo player)
        //{
        //    if (player == null)
        //        throw new ArgumentNullException(nameof(player));

        //    try
        //    {
        //        return PartyCollection.SingleOrDefault(p => p.Players.Contains(player));
        //    }
        //    catch
        //    {
        //        throw new Exception($"Player ({player}) is in more than one party");
        //    }
        //}

        //public Party GetOrCreatePartyByPlayer(CharacterInfo player)
        //{
        //    if (player == null)
        //        throw new ArgumentNullException(nameof(player));

        //    try
        //    {
        //        var party = PartyCollection.SingleOrDefault(p => p.Players.Contains(player));
        //        if (party == null)
        //        {
        //            party = new Party(player);
        //            Add(party);
        //        }
        //        return party;
        //    }
        //    catch
        //    {
        //        throw new Exception($"Player ({player}) is in more than one party");
        //    }
        //}

        public PartyMatching GetPartyMatchingByPlayer(CharacterInfo player, MatchingTypes type)
        {
            ArgumentNullException.ThrowIfNull(player);

            try
            {
                var collection = GetReadOnly(PartyMatchingCollection);
                return collection.SingleOrDefault(pm => pm.MatchingType == type &&
                        pm.MatchingProfiles.Any(prof => prof.Name.Equals(player.Name)));
            }
            catch
            {
                throw new Exception($"Player ({player}) is in more than one PartyMatching ({type})");
            }
        }

        public CharacterInfo GetPlayerByName(string name)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);

            try
            {
                var collection = GetReadOnly(PlayerCollection);
                return collection.SingleOrDefault(p => p.Name.Equals(name));
            }
            catch
            {
                throw new Exception($"There are more than one player with the same name: {name}");
            }
        }

        public PartyMatchInfoPage GetPartyMatchInfoPageByPage(int page)
        {
            try
            {
                lock (lockers[typeof(PartyMatchInfoPage)])
                {
                    return PartyMatchInfoPageCollection.SingleOrDefault(i => i.Page == page);
                }
            }
            catch
            {
                throw new Exception($"There are more than one PartyMatchInfo with the same page: {page}");
            }
        }

        public void Add(CharacterInfo player)
        {
            //var old = GetPlayerByName(player.Name);
            //if (old != null)
            //    Replace(old, player);

            Add(PlayerCollection, player);
        }

        //public void Add(Party party)
        //{
        //    Add(PartyCollection, party);
        //}

        public void Add(PartyMatching partyMatching)
        {
            // check for duplicates before adding
            var names = partyMatching.MatchingProfiles.Select(p => p.Name);

            var collection = GetReadOnly(PartyMatchingCollection);
            var forRemoving = collection.Where(m => 
                m.MatchingType == partyMatching.MatchingType && 
                m.MatchingProfiles.Any(p => names.Contains(p.Name)));
            
            foreach (var r in forRemoving)
                Remove(PartyMatchingCollection, r);

            Add(PartyMatchingCollection, partyMatching);
        }

        public void Add(PartyMatchInfoPage infoPage)
        {
            var old = GetPartyMatchInfoPageByPage(infoPage.Page);
            if (old != null)
                Remove(PartyMatchInfoPageCollection, old);

            Add(PartyMatchInfoPageCollection, infoPage);
        }

        public void Remove(CharacterInfo player)
        {
            Remove(PlayerCollection, player);
        }

        //public void Remove(Party party)
        //{
        //    Remove(PartyCollection, party);
        //}

        public void Remove(PartyMatching partyMatching, string removeBy)
        {
            partyMatching.RequestRemove(removeBy);

            if (partyMatching.IsRemovable)
                Remove(PartyMatchingCollection, partyMatching);
        }

        public void Remove(PartyMatchInfoPage infoPage)
        {
            Remove(PartyMatchInfoPageCollection, infoPage);
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
        private ReadOnlyCollection<T> GetReadOnly<T>(TeraDataPool<T> pool)
        {
            lock (lockers[typeof(T)])
            {
                return pool.AsReadOnly();
            }
        }
    }
}
