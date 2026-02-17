using System;

namespace TaskForge.Core.Scheduler;

public class ScheduleEntry
{
    public Guid Id { get; }
    public Job Job { get; }
    public ITrigger Trigger { get; }

    public DateTimeOffset NextRunTime { get; set; }

    public ScheduleEntry(Job job, ITrigger trigger)
    {
        Id = Guid.NewGuid();
        Job = job;
        Trigger = trigger;
        NextRunTime = trigger.GetNextOccurrence(DateTimeOffset.UtcNow);
    }
}

