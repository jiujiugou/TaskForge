using TaskForge.Core.Execution;
using Microsoft.Extensions.Logging;

namespace TaskForge.Core.Scheduler;

public class JobScheduler : IScheduler
{
    private readonly IScheduleStore _store;
    private readonly JobChannel _channel;
    private CancellationTokenSource? _cts;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private Task? _loopTask;

    private readonly ILogger<JobScheduler> _logger;

    public JobScheduler(IScheduleStore store, JobChannel channel, ILogger<JobScheduler> logger)
    {
        _store = store;
        _channel = channel;
        _logger = logger;
    }

    public void Schedule(Job job, ITrigger trigger)
    {
        var entry = new ScheduleEntry(job, trigger);
        _store.Add(entry);
    }

    public async Task StartAsync(CancellationToken token)
    {
        await _semaphore.WaitAsync(token);
        try
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            _loopTask = LoopAsync(_cts.Token);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task StopAsync()
    {
        CancellationTokenSource? ctsCopy;
        Task? loopCopy;

        await _semaphore.WaitAsync();
        try
        {
            if (_cts == null) return;

            ctsCopy = _cts;
            loopCopy = _loopTask;

            _cts = null;
            _loopTask = null;
        }
        finally
        {
            _semaphore.Release();
        }

        ctsCopy.Cancel();

        if (loopCopy != null)
            await loopCopy;

        ctsCopy.Dispose();
    }


    private async Task LoopAsync(CancellationToken token)
    {
        _logger.LogInformation("Job scheduler started");
        try
        {
            while (!token.IsCancellationRequested)
            {
                var now = DateTimeOffset.UtcNow;
                foreach (var entry in _store.GetAll())
                {
                    try
                    {
                        if (entry.NextRunTime <= now)
                        {
                            _logger.LogDebug("Enqueuing job {JobId} for execution", entry.Job.Id);
                            await _channel.EnqueueAsync(entry.Job);

                            entry.NextRunTime =
                                entry.Trigger.GetNextOccurrence(now);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // 忽略，因为是正常取消
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while processing schedule entry for job {JobId}", entry.Job.Id);
                    }
                }
                await Task.Delay(500, token);
            }
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in job scheduler loop");
        }
        finally
        {
            _logger.LogInformation("Job scheduler stopped");
        }
    }
}

