using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Tera.Game.Structures
{
    public class PartyMatching
    {
        private readonly ConcurrentDictionary<string, byte> removeRequestNames = new();
        public IList<MatchingProfile> MatchingProfiles { get; set; }
        public IList<MatchingInstance> Instances { get; init; }
        public MatchingTypes MatchingType { get; init; }
        public bool IsActive => MatchingProfiles.Count - removeRequestNames.Count >= 2;

        public PartyMatching(IList<MatchingProfile> profiles, IList<MatchingInstance> instances, MatchingTypes type)
        {
            MatchingProfiles = profiles;
            Instances = instances;
            MatchingType = type;
        }

        public void RequestRemove(string requestedBy)
        {
            removeRequestNames.TryAdd(requestedBy, 0);
        }
        public bool IsRemoveRequested(string requestedBy)
        {
            return removeRequestNames.ContainsKey(requestedBy);
        }
        public bool IsRemoveRequested()
        {
            return removeRequestNames.Any();
        }

        public override string ToString()
        {
            var names = string.Join(' ', MatchingProfiles.Select(p => p.Name));
            return $"Type: {MatchingType}, Players: {names}, Instances count: {Instances.Count}";
        }
    }
}
