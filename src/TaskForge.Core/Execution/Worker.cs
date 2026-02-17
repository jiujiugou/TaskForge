using Microsoft.Extensions.Logging;

namespace TaskForge.Core.Execution;

public class Worker : IWorker
{
    private readonly JobChannel _channel;
    private Task? _runningTask;
    private CancellationTokenSource? _cts;
    private readonly ILogger<Worker>? _logger;
    public Worker(JobChannel channel, ILogger<Worker>? logger=null)
    {
        _channel = channel;
        _logger = logger;
    }
    /// <summary>
    /// 启动 Worker，持续从 JobChannel 中获取 Job 并执行，直到取消令牌被触发
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task StartAsync(CancellationToken token)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        _runningTask = RunAsync(_cts.Token);
        return _runningTask;
    }
    /// <summary>
    /// 停止 Worker，等待当前正在执行的 Job 完成后退出
    /// </summary>
    /// <returns></returns>
    public async Task StopAsync()
    {
        if(_cts != null)
        {
            _cts.Cancel();
        }
        if (_runningTask != null)
            await _runningTask;
    }
    
    private async Task RunAsync(CancellationToken token)
    {
        _logger?.LogInformation("Run started");
        try
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var job = await _channel.DequeueAsync(token);
                    _logger?.LogDebug("Executing job {JobId}", job.Id);
                    await RetryPolicyExecutor.ExecuteAsync(job.Execute, token: token);
                    _logger?.LogDebug("Finished job {JobId}", job.Id);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    // 可以加入日志或事件处理
                    _logger?.LogError(ex, "Error executing job");
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in worker loop");
        }
        finally
        {
            _logger?.LogInformation("Run stopped");
        }
    }
}

