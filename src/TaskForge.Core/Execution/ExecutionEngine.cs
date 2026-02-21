using Microsoft.Extensions.Logging;

namespace TaskForge.Core.Execution;

public class ExecutionEngine : IExecutionEngine, IAsyncDisposable
{
    private readonly JobChannel _channel;
    private readonly WorkerPool _pool;
    private CancellationTokenSource? _cts;
    private readonly ILogger<ExecutionEngine> _logger;
    private readonly SemaphoreSlim _lifecycleLock = new SemaphoreSlim(1, 1);
    private volatile bool _isStarted = false;
    public ExecutionEngine(ILogger<ExecutionEngine> logger, int workerCount = 4, int queueCapacity = 100)
    {
        _channel = new JobChannel(queueCapacity);
        _pool = new WorkerPool(_channel, workerCount);
        _logger = logger;
    }
    /// <summary>
    /// 入队
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    public async Task EnqueueAsync(Job job)
    {
        if (!_isStarted)
            throw new InvalidOperationException("ExecutionEngine is not started");
        await _channel.EnqueueAsync(job);
    }

    // =============================
    // 启动
    // =============================
    public async Task StartAsync(CancellationToken token)
    {
        await _lifecycleLock.WaitAsync(token);
        try
        {
            if (_isStarted) return;

            _logger.LogInformation("ExecutionEngine starting...");
            _cts = CancellationTokenSource.CreateLinkedTokenSource(token);

            try
            {
                await _pool.StartAsync(_cts.Token);
                _isStarted = true;
                _logger.LogInformation("ExecutionEngine started successfully.");
            }
            catch
            {
                _isStarted = false;
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
                throw;
            }
        }
        finally
        {
            _lifecycleLock.Release();
        }
    }

    // =============================
    // 停止
    // =============================
    public async Task StopAsync()
    {
        CancellationTokenSource? localCts;

        await _lifecycleLock.WaitAsync();
        try
        {
            if (!_isStarted || _cts == null)
            {
                _logger.LogDebug("ExecutionEngine already stopped.");
                return;
            }

            _logger.LogInformation("ExecutionEngine stopping...");
            localCts = _cts;
            _cts = null;
            _isStarted = false;
        }
        finally
        {
            _lifecycleLock.Release();
        }

        try
        {
            localCts.Cancel();
            await _pool.StopAsync();
            _logger.LogInformation("ExecutionEngine stopped successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while stopping ExecutionEngine.");
        }
        finally
        {
            localCts.Dispose();
        }
    }

    // =============================
    // 释放资源
    // =============================
    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_isStarted)
                await StopAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during ExecutionEngine dispose.");
        }
        finally
        {
            _lifecycleLock.Dispose();
            _cts?.Dispose();
        }
    }
}
