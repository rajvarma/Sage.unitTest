using System;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.Storage;

namespace Sage.Core.Framework.Storage.Exceptions
{
    /// <summary>
    /// Wrapper exception thrown by the blob.
    /// Inner exceptions will include the specific provider error
    /// </summary>
    [Serializable]
    public class BlobStorageException : BaseStorageException 
    {
        /// <summary>
        /// Name of the blob
        /// </summary>
        private string Name { get; set; }

        /// <summary>
        /// Path (container name) of the blob
        /// </summary>
        private string Path { get; set; }

        public BlobStorageException()
        {
        }

        public BlobStorageException(String messsage)
            : base(messsage)
        {
        }

        public BlobStorageException(String messsage, StorageException innerException)
            : base(messsage, innerException)
        {
        }

        public BlobStorageException(String messsage, String name, String path)
            : base(messsage)
        {
            Name = name;
            Path = path;
        }

        public BlobStorageException(String messsage, String name, String path, StorageException innerException)
            : base(messsage, innerException)
        {
            Name = name;
            Path = path;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,BlobResource.BlobErrorMessageFormat, Message, Name, Path);
        }

        /// <summary>
        /// This is required to suppress the Code Analysis error "CA2240: Implement ISerializable correctly"
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
