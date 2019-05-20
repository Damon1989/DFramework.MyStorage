using System;
using System.Runtime.Serialization;

namespace DFramework.MyStorage.SDK
{
    public class StorageException : Exception
    {
        public StorageException()
        {
        }

        public StorageException(ErrorCode errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        protected StorageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ErrorCode ErrorCode { get; set; }
    }
}