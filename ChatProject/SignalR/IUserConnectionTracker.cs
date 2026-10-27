namespace Api.SignalR
{
    public interface IUserConnectionTracker
    {
        bool Add(Guid userId, string connectionId);    
        bool Remove(Guid userId, string connectionId);  
        IReadOnlyCollection<string> GetConnections(Guid userId);
        bool IsOnline(Guid userId);
    }
}
