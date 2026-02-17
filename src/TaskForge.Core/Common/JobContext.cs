using Microsoft.Extensions.Logging;

namespace TaskForge.Core.Common;

/// <summary>
/// 任务执行上下文，包含任务本身、状态、取消令牌和执行信息
/// </summary>
public class JobContext
{
    /// <summary>任务 ID</summary>
    public Guid JobId { get; }

    /// <summary>任务对象</summary>
    public Job Job { get; }

    /// <summary>取消令牌，用于任务停止或超时控制</summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>当前执行尝试次数（可用于重试策略）</summary>
    public int Attempt { get; internal set; } = 0;

    /// <summary>任务开始时间</summary>
    public DateTimeOffset? StartTime { get; internal set; }

    /// <summary>任务结束时间</summary>
    public DateTimeOffset? EndTime { get; internal set; }

    /// <summary>异常信息（如果执行失败）</summary>
    public Exception? Exception { get; internal set; }

    /// <summary>日志记录器，可在策略或任务中使用</summary>
    public ILogger? Logger { get; }

    public JobContext(Job job, CancellationToken cancellationToken, ILogger? logger = null)
    {
        Job = job ?? throw new ArgumentNullException(nameof(job));
        JobId = job.Id;
        CancellationToken = cancellationToken;
        Logger = logger;
    }

    /// <summary>
    /// 执行任务核心方法
    /// </summary>
    public async Task<object?> ExecuteAsync()
    {
        StartTime = DateTimeOffset.UtcNow;
        try
        {
            var result = await Job.Execute(CancellationToken);
            EndTime = DateTimeOffset.UtcNow;
            return result;
        }
        catch (Exception ex)
        {
            EndTime = DateTimeOffset.UtcNow;
            Exception = ex;
            throw;
        }
    }
}


