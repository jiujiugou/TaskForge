namespace TaskForge.Core.Scheduler;

public class IntervalTrigger : ITrigger
{
    private readonly TimeSpan _interval;

    public IntervalTrigger(TimeSpan interval)
    {
        _interval = interval;
    }

    public DateTimeOffset GetNextOccurrence(DateTimeOffset now)
    {
        return now.Add(_interval);
    }
}

