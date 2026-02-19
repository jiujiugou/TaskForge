using TaskForge.Core.Common;

namespace TaskForge.Core.Policy;

public class RetryPolicy : IJobPolicy
{
    private readonly int _Retries;
    private readonly TimeSpan _delay;
    public RetryPolicy(int retries, TimeSpan? delay = null)
    {
        _Retries = retries;
        _delay = delay ?? TimeSpan.FromMilliseconds(500);
    }
    public async Task ExecuteAsync(JobContext context, JobPipelineDelegate action)
    {
        for (int i = 0; i < _Retries; i++)
        {
            try
            {
                await action(context);
                return;
            }
            catch
            {
                if (i == _Retries - 1)
                    throw;
                await Task.Delay(_delay, context.CancellationToken);
            }
        }
    }

}
