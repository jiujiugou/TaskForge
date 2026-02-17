namespace TaskForge.Core.Execution;

public class ExecutionEngine : IExecutionEngine
{
    private readonly JobChannel _channel;
    private readonly WorkerPool _pool;
    private CancellationTokenSource? _cts;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public ExecutionEngine(int workerCount = 4, int queueCapacity = 100)
    {
        _channel = new JobChannel(queueCapacity);
        _pool = new WorkerPool(_channel, workerCount);
    }

    public async Task EnqueueAsync(Job job)
    {
        await _channel.EnqueueAsync(job);
    }

    public async Task StartAsync(CancellationToken token)
    {
        await _semaphore.WaitAsync(token);
        try
        {
            if (_cts != null) return;
            _cts?.Dispose();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        }
        finally
        {
            _semaphore.Release();
        }
        await _pool.StartAsync(_cts.Token);
    }

    public async Task StopAsync()
    {
        CancellationTokenSource? ctsCopy;

        await _semaphore.WaitAsync();
        try
        {
            if (_cts == null) return; // 未启动，直接返回
            ctsCopy = _cts;
            _cts = null;
        }
        finally
        {
            _semaphore.Release();
        }

        ctsCopy.Cancel();
        await _pool.StopAsync();
        ctsCopy.Dispose();
    }
    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
