using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace DFramework.MyStorage
{ 
    public class File:AggregateRoot<string>,IHasCreationTime
    {
        public string Path { get; set; }
        public long Size { get; set; }
        public int ReferenceCount { get; set; }
        public string NodeId { get; set; }
        [ForeignKey("NodeId")]
        public virtual Node  Node{ get; set; }
        public DateTime CreationTime { get; set; }
    }
}
