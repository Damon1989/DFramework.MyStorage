namespace DFramework.MyStorage
{
    public class Node:AggregateRoot<string>
    {
        public string Name { get; set; }
        public string UrlHost { get; set; }
        public string Config { get; set; }
        /// <summary>
        /// 容量
        /// </summary>
        public long Capacity { get; set; }

        public long FileSize { get; set; }
        public long FileCount { get; set; }
        public string FullType { get; set; }
        public NodeStatus Status { get; set; }
    }
}
