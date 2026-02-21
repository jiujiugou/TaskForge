using TaskForge.Core.Common;
using TaskForge.Core.Policy;

namespace TaskForge.Core.Execution;

public class JobWrapper
{
    public Job Job { get; }
    public JobContext context { get; }
    public JobPipelineDelegate pipeline { get; }
    public JobWrapper(Job job, JobPipelineDelegate pipeline)
    {
        Job = job;
        this.pipeline = pipeline;
        this.context = new JobContext(Job, CancellationToken.None);
    }
}
