using System.IO;
using System.Text.Json;
using TaskForge.Core;
using TaskForge.Core.Common;
using TaskForge.Core.Server;
using TaskForge.Core.Storage;
using TaskForge.Core.Storage.InterfaceContant;

namespace TaskForge.Storage.File;

public class JsonStorageConnection : IJobStorage
{
    public string CreateExpiredJob(InvocationData invocationData, TimeSpan expireIn)
    {
        var jobId = Guid.NewGuid().ToString();

        var record = new JsonRecord
        {
            Id = jobId,
            InvocationData = invocationData,
            CurrentStateName = "Created",
            CreatedAt = DateTime.UtcNow,
            ExpireAt = DateTime.UtcNow.Add(expireIn)
        };

        var json = JsonSerializer.Serialize(record, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        var folder = "D:\\Code\\TaskForge\\src\\TaskForge.Storage.File";
        var filePath = Path.Combine(folder, $"{jobId}.json");
        System.IO.File.WriteAllTextAsync(filePath, json);

        return jobId;
    }
    public JobData GetJobData(string jobId)
    {
        var json = System.IO.File.ReadAllText(@"D:\Code\TaskForge\src\TaskForge.Storage.File\File");
        var record = JsonSerializer.Deserialize<JsonRecord>(json);
        if (record == null)
        {
            return null!;
        }
        var targetType = Type.GetType(record.InvocationData.Type ?? string.Empty);
        if (targetType == null)
        {
            return null!;
        }
        var targetMethod = targetType.GetMethod(record.InvocationData.Method ?? string.Empty);
        if (targetMethod == null)
        {
            return null!;
        }
        return new JobData
        {
            Job = new Job(
                Guid.Parse(record.Id),
                targetType,
                targetMethod,
                Array.Empty<object>()
            ),
            InvocationData = JsonSerializer.Serialize(record.InvocationData),
            CurrentState = record.CurrentStateName,
            CreateAt = record.CreatedAt,
            ExpireAt = record.ExpireAt
        };
    }

    public StateData GetStateData(string jobId)
    {
        var jobData = GetJobData(jobId);
        if (jobData == null)
        {
            return null!;
        }
        return new StateData
        {
            Name = jobData.CurrentState,
            Reason = "",
            Data = jobData.ParametersSnapshot?.ToDictionary(kv => kv.Key, kv => kv.Value)
        };
    }

}
