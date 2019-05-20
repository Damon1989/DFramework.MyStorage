using System;
using System.Runtime.Serialization;

namespace DFramework.MyStorage
{
    public class StorageException : Exception
    {
        public ErrorCode ErrorCode { get; set; }
        public StorageException() { }
        public StorageException(ErrorCode errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }
        protected StorageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
