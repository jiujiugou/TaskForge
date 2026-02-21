using System.Threading.Channels;

namespace TaskForge.Core.Execution;

public class JobChannel
{
    private readonly Channel<JobWrapper> _channel;
    public JobChannel(int capacity = 100)
    {
        _channel = Channel.CreateBounded<JobWrapper>(capacity);
    }
    /// <summary>
    /// 将 Job 添加到队列中
    /// </summary> <param name="job"></param>
    /// <returns></returns>
    /// <exception cref="ChannelFullException">当队列已满时抛出</exception>
    public async Task EnqueueAsync(JobWrapper jobWrapper)
    {
        await _channel.Writer.WriteAsync(jobWrapper);
    }
    /// <summary>
    /// 从队列中取出一个 Job 进行执行
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<JobWrapper> DequeueAsync(CancellationToken token)
    {
        return await _channel.Reader.ReadAsync(token);
    }
}
