namespace TaskForge.Core.Execution;

public interface IWorker
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync();
}
