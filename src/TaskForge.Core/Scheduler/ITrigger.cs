using System;

namespace TaskForge.Core.Scheduler;

public interface ITrigger
{
    DateTimeOffset GetNextOccurrence(DateTimeOffset now);
}

