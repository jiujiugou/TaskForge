using TaskForge.Core.Server;
using TaskForge.Core.Storage.InterfaceContant;

namespace TaskForge.Core.Storage;

public interface IStorageConnection : IDisposable, IServerStorage, IQueueStorage, IJobStorage, IJobStateStorage, IKeyValueStorage
{

}

