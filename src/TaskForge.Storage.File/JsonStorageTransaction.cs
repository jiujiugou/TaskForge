using TaskForge.Core.States;
using TaskForge.Core.Storage;

namespace TaskForge.StorageMedia;

public class JsonStorageTransaction : JobStorageTransaction
{
    Dictionary<string, JobData> jobDataCache = new Dictionary<string, JobData>();
    Dictionary<string, List<IState>> jobStateCache = new Dictionary<string, List<IState>>();
    Dictionary<string, List<string>> queueCache = new Dictionary<string, List<string>>();
    /// <summary>
    /// 添加 Job 状态记录
    /// </summary> <param name="jobId"></param> <param name="state"></param>
    public override void AddJobState(string jobId, IState state)
    {
        if(jobStateCache.TryGetValue(jobId, out var states))
        {
            states.Add(state);
        }
        else
        {
            jobStateCache[jobId] = new List<IState> { state };
        }
    }
    /// <summary>
    /// 设置 Job 过期时间
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="expireIn"></param>
    /// <exception cref="NotImplementedException"></exception>
     public override void ExpireJob(string jobId, TimeSpan? expireIn)
    {
        if (!jobDataCache.TryGetValue(jobId, out var job))
            throw new ArgumentException("Job not found", nameof(jobId));

        job.ExpireAt = expireIn.HasValue ? DateTime.UtcNow + expireIn.Value : null;
    }
    /// <summary>
    /// 改变 Job 状态（添加状态记录并根据状态是否为最终状态设置过期时间）
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="state"></param>
    public void ChangeState(string jobId,IState state)
    {
        if(state.IsFinal)
        {
            ExpireJob(jobId,TimeSpan.FromDays(7));
        }   
    }
    /// <summary>
    /// 将 Job 标记为永久保存（不再过期）
    /// </summary>
    /// <param name="jobId"></param>
    /// <exception cref="NotImplementedException"></exception>
    public override void PersistJob(string jobId)
    {
        if(!jobDataCache.TryGetValue(jobId, out var jobData))
        {
            throw new KeyNotFoundException($"Job {jobId} not found.");
        }
        else
        {
            jobData.ExpireAt=null;
        }
    }
    /// <summary>
    /// 设置 Job 当前状态
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="state"></param>
    /// <exception cref="NotImplementedException"></exception>
    public override void SetJobState(string jobId, IState state)
    {
        if(!jobDataCache.TryGetValue(jobId, out var jobData))
        {
            throw new KeyNotFoundException($"Job {jobId} not found.");
        }
        /// 设置当前状态
        jobData.CurrentState=state.Name;
        AddJobState(jobId,state);
        if(state.IsFinal)
        {
            ExpireJob(jobId,null);
        }
    }
    /// <summary>
    /// 将 Job 放入队列
    /// </summary>
    /// <param name="queue"></param>
    /// <param name="jobId"></param>
    /// <exception cref="NotImplementedException"></exception>
    public override void AddToQueue(string queue, string jobId)
    {
        if(queueCache.TryGetValue(queue, out var jobIds))
        {
            jobIds.Add(jobId);
        }
        else
        {
            queueCache[queue] = new List<string> { jobId };
        }
    }
    /// <summary>
    /// 提交事务
    /// </summary> <exception cref="NotImplementedException"></exception>
    /// <remarks>可以在此处实现批量提交逻辑</remarks>
    public override void Commit()
    {
        
    }
    /// <summary>
    /// 释放资源
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}
