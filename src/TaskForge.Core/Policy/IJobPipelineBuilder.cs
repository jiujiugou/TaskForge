using TaskForge.Core.Common;

namespace TaskForge.Core.Policy;

public interface IJobPipelineBuilder
{
    /// <summary>
    /// 添加一个策略到管道中
    /// </summary>
    /// <param name="next">要添加的策略</param>
    /// <returns>当前管道构建器实例，支持链式调用</returns>
    IJobPipelineBuilder UsePolicy(IJobPolicy next);
    /// <summary>
    /// 构建最终的执行管道
    /// </summary>
    /// <returns>一个可执行的管道委托</returns>
    JobPipelineDelegate Build();
}
