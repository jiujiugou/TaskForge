using TaskForge.Core.Common;

namespace TaskForge.Core.Policy;

public class ConcurrencyPolicy : IJobPolicy
{
    private readonly SemaphoreSlim _semaphore;

    public ConcurrencyPolicy(int maxConcurrent)
    {
        _semaphore = new SemaphoreSlim(maxConcurrent, maxConcurrent);
    }

    public async Task ExecuteAsync(JobContext context, JobPipelineDelegate action)
    {
        await _semaphore.WaitAsync(context.CancellationToken);
        try
        {
            await action(context);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
