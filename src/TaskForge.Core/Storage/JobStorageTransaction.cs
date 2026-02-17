using TaskForge.Core.States;

namespace TaskForge.Core.Storage;

public abstract class JobStorageTransaction : IWriteOnlyTransaction
{

    public abstract void AddJobState(string jobId, IState state);


    public abstract void AddToQueue(string queue, string jobId);

    public abstract void Commit();

    public abstract void Dispose();

    public abstract void ExpireJob(string jobId, TimeSpan? expireIn);

    public abstract void PersistJob(string jobId);

    public abstract void SetJobState(string jobId, IState state);

}
