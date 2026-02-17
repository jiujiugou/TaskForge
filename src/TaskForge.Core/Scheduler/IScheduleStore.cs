namespace TaskForge.Core.Scheduler;

public interface IScheduleStore
{
    void Add(ScheduleEntry entry);

    IReadOnlyCollection<ScheduleEntry> GetAll();
}

