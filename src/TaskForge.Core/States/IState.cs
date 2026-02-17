namespace TaskForge.Core.States;

public interface IState
{
        /// 状态名称
        public string Name { get;}
        public string Reason { get; set; }
        public bool IsFinal { get; }
        bool IgnoreJobLoadException { get; }
        public Dictionary<string,string> SerializeData();
}
