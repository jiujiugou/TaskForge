namespace TaskForge.Core.Storage;

public class StateData
{
    //状态名称
    public string? Name{get;set;}
    //状态原因
    public string? Reason{get;set;}
    //状态序列化后的字典
    public Dictionary<string,string>? Data{get;set;}
}
