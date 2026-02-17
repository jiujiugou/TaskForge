using TaskForge.Core;
using TaskForge.Core.Server;
using TaskForge.Core.States;
using TaskForge.Core.Storage;

namespace TaskForge.Storage.File;

public abstract class StorageConnection : IStorageConnection
{
    public abstract void AddJobState(string jobId, IState state);

    public abstract void AddToQueue(string queue, string jobId);

    public abstract void AnnounceServer(string serverId, ServerContext context);

    public abstract string CreateExpiredJob(InvocationData invocationData, TimeSpan expireIn);


    public abstract IWriteOnlyTransaction CreateWriteOnlyTransaction();



    public abstract IFetchedJob FetchNextJob(string[] queues);

    public abstract Dictionary<string, string> GetAllEntriesFromHash(string key);

    public abstract HashSet<string> GetAllItemsFromSet(string key);

    public abstract string GetFirstByLowestScoreFromSet(string key, double fromScore, double toScore);

    public abstract JobData GetJobData(string jobId);

    public abstract StateData GetStateData(string jobId);

    public abstract void Heartbeat(string serverId);

    public abstract void RemoveServer(string serverId);

    public abstract int RemoveTimedOutServers(TimeSpan timeOut);

    public abstract void SetJobState(string jobId, IState state);

    public abstract void SetRangeInHash(string key, IEnumerable<KeyValuePair<string, string>> keyValuePairs);

    public abstract void Dispose();
}
