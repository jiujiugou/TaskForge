using TaskForge.Core.Common;

namespace TaskForge.Core.Policy;

public class JobPipelineBuilder : IJobPipelineBuilder
{
    private readonly IList<IJobPolicy>? _policies;
    private readonly Func<JobContext, Task>? _job;
    public JobPipelineDelegate Build()
    {
        JobPipelineDelegate pipeline = context => _job(context);
        foreach (var policy in _policies.Reverse())
        {
            var next = pipeline;
            pipeline = context => policy.ExecuteAsync(context, next);
        }
        return pipeline;
    }

    public IJobPipelineBuilder UsePolicy(IJobPolicy next)
    {
        _policies?.Add(next);
        return this;
    }

}
