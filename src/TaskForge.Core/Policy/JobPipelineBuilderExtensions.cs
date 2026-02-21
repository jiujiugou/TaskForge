using System;
using TaskForge.Core.Policy.Policies;

namespace TaskForge.Core.Policy;

public static class JobPipelineBuilderExtensions
{
    public static IJobPipelineBuilder Run(this IJobPipelineBuilder app, JobPipelineDelegate handler)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));
        if (handler == null) throw new ArgumentNullException(nameof(handler));
        var runPolicy = new RunPolicy(handler);
        return app.UsePolicy(runPolicy);
    }
}
