namespace Api.SignalR
{
    public interface IUserConnectionTracker
    {
        //void Add(Guid userId, string connectionId);
        //void Remove(Guid userId, string connectionId);
        //IReadOnlyCollection<string> GetConnections(Guid userId);
        //bool IsOnline(Guid userId);

        bool Add(Guid userId, string connectionId);     // true لو دي أول connection
        bool Remove(Guid userId, string connectionId);  // true لو دي آخر connection
        IReadOnlyCollection<string> GetConnections(Guid userId);
        bool IsOnline(Guid userId);
    }
}
