namespace TaskForge.Core.Execution;

public interface IExecutionEngine
{
    /// <summary>
    /// 启动执行引擎
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 停止执行引擎
    /// </summary>
    Task StopAsync();
    /// <summary>
    /// 将 Job 添加到执行队列中
    /// </summary>
    /// <param name="job"></param>
    Task EnqueueAsync(Job job);
}
