using TaskForge.Core.Server;
using TaskForge.Core.States;

namespace TaskForge.Core.Storage;

public abstract class StorageConnection : IStorageConnection
{

    public abstract IWriteOnlyTransaction CreateWriteOnlyTransaction();
    //创建一个带过期时间的任务
    public abstract string CreateExpiredJob(InvocationData invocationData, TimeSpan expireIn);
    //获取任务的详细数据
    public abstract JobData GetJobData(string jobId);
    public abstract StateData GetStateData(string jobId);
    public abstract IFetchedJob FetchNextJob(string[] queues,CancellationToken cancellationToken);
    public abstract void AddToQueue(string queue, string jobId);
    public abstract HashSet<string> GetAllItemsFromSet(string key);
    public abstract string GetFirstByLowestScoreFromSet(string key, double fromScore, double toScore);
    public abstract void SetRangeInHash(string key, IEnumerable<KeyValuePair<string,string>> keyValuePairs);
    public abstract Dictionary<string, string> GetAllEntriesFromHash(string key);
    public abstract void AnnounceServer(string serverId, ServerContext context);
    public abstract void RemoveServer(string serverId);
    public abstract void Heartbeat(string serverId);
    public abstract int RemoveTimedOutServers(TimeSpan timeOut);
    public abstract void Dispose();

    public abstract IFetchedJob FetchNextJob(string[] queues);

    public abstract void SetJobState(string jobId, IState state);

    public abstract void AddJobState(string jobId, IState state);

}
