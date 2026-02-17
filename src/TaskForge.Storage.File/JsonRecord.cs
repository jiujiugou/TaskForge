using TaskForge.Core;

namespace TaskForge.Storage.File;

public class JsonRecord
{
    /// <summary>
    /// Job 唯一标识
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    /// 方法调用信息（序列化后的 InvocationData）
    /// </summary>
    public InvocationData InvocationData { get; set; } = default!;

    /// <summary>
    /// 当前状态名称
    /// </summary>
    public string CurrentStateName { get; set; } = default!;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 过期时间（null 表示不过期）
    /// </summary>
    public DateTime? ExpireAt { get; set; }

}
