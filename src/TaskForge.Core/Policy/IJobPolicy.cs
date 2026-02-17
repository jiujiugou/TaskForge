using TaskForge.Core.Common;

namespace TaskForge.Core.Policy;

public interface IJobPolicy
{
    /// <summary>
    /// 包装job执行逻辑
    /// </summary>
    /// <param name="action">执行函数</param>
    /// <param name="token">取消令牌</param>
    /// <returns></returns>
    Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken token);
}
