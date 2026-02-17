namespace TaskForge.Core.Storage.InterfaceContant;

public interface IKeyValueStorage
{    
    HashSet<string> GetAllItemsFromSet(string key);
    string GetFirstByLowestScoreFromSet(string key, double fromScore, double toScore);
    void SetRangeInHash(string key, IEnumerable<KeyValuePair<string,string>> keyValuePairs);
    Dictionary<string, string> GetAllEntriesFromHash(string key);
}
