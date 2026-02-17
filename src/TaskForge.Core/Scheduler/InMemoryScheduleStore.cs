namespace TaskForge.Core.Scheduler;

public class InMemoryScheduleStore : IScheduleStore
{
    private readonly List<ScheduleEntry> _entries = new();

    public void Add(ScheduleEntry entry)
    {
        _entries.Add(entry);
    }

    public IReadOnlyCollection<ScheduleEntry> GetAll()
    {
        return _entries;
    }
}

