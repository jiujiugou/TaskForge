namespace TaskForge.Core.Scheduler;

public interface IScheduler
{
    void Schedule(Job job,ITrigger trigger);

    public Task StartAsync(CancellationToken token);
    public Task StopAsync();
}
