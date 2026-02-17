namespace TaskForge.Core.Storage.InterfaceContant;

public interface IQueueStorage
{
    void AddToQueue(string queue, string jobId);
    IFetchedJob FetchNextJob(string[] queues);

}
