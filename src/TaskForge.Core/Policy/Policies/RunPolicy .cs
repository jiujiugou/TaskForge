using TaskForge.Core.Common;

namespace TaskForge.Core.Policy.Policies;

public class RunPolicy : IJobPolicy
{
    private readonly JobPipelineDelegate _handler;

    public RunPolicy(JobPipelineDelegate handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public Task ExecuteAsync(JobContext context, JobPipelineDelegate next)
    {
        // 这里忽略 next，直接执行终结 Job
        return _handler(context);
    }
}
