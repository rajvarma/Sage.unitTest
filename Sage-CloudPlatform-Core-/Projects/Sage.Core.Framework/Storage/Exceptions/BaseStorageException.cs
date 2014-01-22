using System;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.Storage;

namespace Sage.Core.Framework.Storage.Exceptions
{
    /// <summary>
    /// Base class for Azure Storage exceptions
    /// </summary>
    [Serializable]
    public class BaseStorageException : StorageException
    {
        public BaseStorageException()
        {
        }

        public BaseStorageException(String msg)
            : base(msg)
        {
        }

        public BaseStorageException(String msg, Exception innerException)
            : base(msg, innerException)
        {
        }

        protected BaseStorageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        
    }
}
