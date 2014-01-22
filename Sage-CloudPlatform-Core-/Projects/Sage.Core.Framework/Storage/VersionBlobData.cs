using System;

namespace Sage.Core.Framework.Storage
{
    /// <summary>
    /// Data to be passed for version upload/download/delete.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VersionBlobData<T>
    {
        /// <summary>
        /// Basic blob data. This will have name, path and content to be uploaded.
        /// </summary>
        public BaseBlobData<T> BaseBlobData { get; set; }

        /// <summary>
        /// Snapshot datetime to download/delete.
        /// </summary>
        public DateTime SnapshotDateTime { get; set; }
    }
}
