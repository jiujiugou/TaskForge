using TaskForge.Core.States;

namespace TaskForge.Core.Storage;

public interface IWriteOnlyTransaction:IDisposable
{
    void ExpireJob(string jobId, TimeSpan? expireIn);  // Job 过期（可先空实现）
    void PersistJob(string jobId);                   // Job 永久保存
    void SetJobState(string jobId, IState state);   // 设置 Job 当前状态
    void AddJobState(string jobId, IState state);   // 添加状态记录
    void AddToQueue(string queue, string jobId);    // Job 放入队列
    void Commit();                                   // 提交事务

}
