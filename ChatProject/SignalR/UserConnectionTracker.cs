using Domain.Entities;
using System.Collections.Concurrent;

namespace Api.SignalR
{
    // add a class that implements IUserConnectionTracker using a ConcurrentDictionary to store user connections
    //ConcurrentDictionary<string,byte> to store the connection ids for each user
    //ConcurrentDictionary<Guid, ConcurrentDictionary<string, byte>> to store the user connections
    //One ID can have more than one connection Ids
    //GetOrAdd method to add a new user connection if it doesn't exist or get the existing one if it does
    //TryAdd method to add a new connection id to the user's connections
    //TryGetValue(userId, out var connections) Sometimes it returns to two valus (bool(T\F),values of connectionIds of user if is T) 
    public class UserConnectionTracker: IUserConnectionTracker
    {
        private readonly ConcurrentDictionary<Guid, HashSet<string>> _users = new();

        public bool Add(Guid userId, string connectionId)
        {
            var set = _users.GetOrAdd(userId, _ => new HashSet<string>());
            lock (set)
            {
                set.Add(connectionId);
                return set.Count == 1;
            }
        }

        public bool Remove(Guid userId, string connectionId)
        {
            if (!_users.TryGetValue(userId, out var set)) return false;
            lock (set)
            {
                set.Remove(connectionId);
                if (set.Count > 0) return false;
                _users.TryRemove(userId, out _);
                return true;
            }
        }

        public IReadOnlyCollection<string> GetConnections(Guid userId)
        {
            if (!_users.TryGetValue(userId, out var set)) return Array.Empty<string>();
            lock (set) return set.ToArray();
        }

        public bool IsOnline(Guid userId) => _users.ContainsKey(userId);

    }
}
