using System.Threading.Channels;

namespace TaskForge.Core.Execution;

public class JobChannel
{
    private readonly Channel<Job> _channel;
    public JobChannel(int capacity = 100)
    {
        _channel = Channel.CreateBounded<Job>(capacity);
    }
    /// <summary>
    /// 将 Job 添加到队列中
    /// </summary> <param name="job"></param>
    /// <returns></returns>
    /// <exception cref="ChannelFullException">当队列已满时抛出</exception>
    public async Task EnqueueAsync(Job job)
    {
        await _channel.Writer.WriteAsync(job);
    }
    /// <summary>
    /// 从队列中取出一个 Job 进行执行
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<Job> DequeueAsync(CancellationToken token)
    {
        return await _channel.Reader.ReadAsync(token);
    }
}
