using System;

namespace TaskForge.Core.Policy;

public class TimeoutPolicy : IJobPolicy
{
    private readonly TimeSpan _timeout;
    public TimeoutPolicy(TimeSpan timeout)
    {
        _timeout = timeout;
    }
    public async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken token)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        var task = action(cts.Token);
        if (await Task.WhenAny(task, Task.Delay(_timeout, cts.Token)) == task)
        {
            // Action completed within timeout
            await task; // Ensure any exceptions are observed
        }
        else
        {
            // Timeout occurred
            cts.Cancel(); // Cancel the action
            throw new TimeoutException($"The job execution exceeded the timeout of {_timeout.TotalSeconds} seconds.");
        }
    }
}
