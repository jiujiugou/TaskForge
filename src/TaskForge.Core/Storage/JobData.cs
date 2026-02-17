using TaskForge.Core.States;

namespace TaskForge.Core.Storage;

public class JobData
{
    //方法信息
    public string? InvocationData{get;set;}
    //JobId
    public Job? Job{get;set;}
    //当前状态
    public string? CurrentState{get;set;}
    public DateTime CreateAt{get;set;}
    public DateTime? ExpireAt{get;set;}
    public IReadOnlyDictionary<string, string>? ParametersSnapshot { get; set; }
}
