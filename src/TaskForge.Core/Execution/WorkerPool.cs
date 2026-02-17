namespace TaskForge.Core.Execution;

public class WorkerPool
{
    private readonly List<IWorker> _workers;
    private readonly JobChannel _channel;
    private readonly int _workerCount;
    public WorkerPool(JobChannel channel, int workerCount)
    {
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        _workerCount = workerCount > 0 ? workerCount : throw new ArgumentOutOfRangeException(nameof(workerCount));
        // 初始化 Worker 列表
        _workers = Enumerable.Range(0, _workerCount)
                             .Select(_ => new Worker(_channel))
                             .Cast<IWorker>()
                             .ToList();
    }
    /// <summary>
    /// 启动所有 Worker 开始处理 Job
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException">当取消令牌被触发时抛出</exception>
    public async Task StartAsync(CancellationToken token)
    {
        var tasks = _workers.Select(worker => worker.StartAsync(token));
        await Task.WhenAll(tasks);
    }
    /// <summary>
    /// 停止所有 Worker，等待当前正在执行的 Job 完成后退出
    /// </summary>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException">当取消令牌被触发时抛出</exception>
    public async Task StopAsync()
    {
        var tasks = _workers.Select(worker => worker.StopAsync());
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
