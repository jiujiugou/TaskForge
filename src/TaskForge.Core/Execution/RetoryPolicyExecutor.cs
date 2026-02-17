namespace TaskForge.Core.Execution;

public static class RetryPolicyExecutor
{
    public static async Task ExecuteAsync(Func<CancellationToken, Task> action, int retries = 3, TimeSpan? delay = null, CancellationToken token = default)
    {
        delay ??= TimeSpan.FromMilliseconds(500);
        for (int i = 0; i < retries; i++)
        {
            try
            {
                await action(token);
                return;
            }
            catch
            {
                if (i == retries - 1) throw;
                await Task.Delay(delay.Value, token);
            }
        }
    }
}
