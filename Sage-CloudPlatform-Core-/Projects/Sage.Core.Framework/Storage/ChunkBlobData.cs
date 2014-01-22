namespace Sage.Core.Framework.Storage
{
    /// <summary>
    /// Data to be passed for chunk upload.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChunkBlobData<T>
    {
        /// <summary>
        /// Basic blob data. This will have name, path and content to be uploaded.
        /// </summary>
        public BaseBlobData<T> BaseBlobData { get; set; }

        /// <summary>
        /// Current chunk number.
        /// </summary>
        public long ChunkNumber { get; set; }

        /// <summary>
        /// Total number of chunks (blocks).
        /// </summary>
        public int TotalChunkSize { get; set; }
    }
}
