namespace TaskForge.Core.Policy;

public class ConcurrencyPolicy : IJobPolicy
{
    private readonly SemaphoreSlim _semaphore;

    public ConcurrencyPolicy(int maxConcurrent)
    {
        _semaphore = new SemaphoreSlim(maxConcurrent, maxConcurrent);
    }

    public async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken token)
    {
        await _semaphore.WaitAsync(token);
        try
        {
            await action(token);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
