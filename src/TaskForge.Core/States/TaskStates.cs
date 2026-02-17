namespace TaskForge.Core.States;

public enum TaskStates
{
    //等待执行
    Pending,
    //正在执行
    Running,
    //执行完成
    Completed,
    //执行失败
    Failed
}
