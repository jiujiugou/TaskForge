using TaskForge.Core.States;

namespace TaskForge.Core.Storage.InterfaceContant;

public interface IJobStateStorage
{
    public IWriteOnlyTransaction CreateWriteOnlyTransaction();
    void SetJobState(string jobId, IState state);
    void AddJobState(string jobId, IState state);
}
