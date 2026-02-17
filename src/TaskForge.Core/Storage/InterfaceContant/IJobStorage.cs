namespace TaskForge.Core.Storage.InterfaceContant;

public interface IJobStorage
{
    public string CreateExpiredJob(InvocationData invocationData, TimeSpan expireIn);
    //获取任务的详细数据
    public JobData GetJobData(string jobId);
    public StateData GetStateData(string jobId);
}
