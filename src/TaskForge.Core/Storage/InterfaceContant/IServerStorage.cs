using TaskForge.Core.Server;

namespace TaskForge.Core.Storage.InterfaceContant;

public interface IServerStorage
{    
    public void AnnounceServer(string serverId, ServerContext context);
    public void RemoveServer(string serverId);
    public void Heartbeat(string serverId);
    public int RemoveTimedOutServers(TimeSpan timeOut);
}
