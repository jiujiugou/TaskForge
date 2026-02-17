namespace TaskForge.Core.Server;

public class ServerContext
{
    public ServerContext()
    {
        Queues = [];
        WorkerCount = -1;
    }
    public int WorkerCount { get; set; }
    public string[] Queues { get; set; }
}
