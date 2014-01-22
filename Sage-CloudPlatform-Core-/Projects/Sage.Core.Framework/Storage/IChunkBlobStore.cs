using Sage.Core.Framework.Common;     //TODO: Remove this class during merge. Use Domain project's ContextChainLink instead.

namespace Sage.Core.Framework.Storage
{
    /// <summary>
    /// This can be used to upload files in chunks.
    /// Also, it inherits the normal file related operations.
    /// </summary>
    public interface IChunkBlobStore
    {
        /// <summary>
        /// This can be used to upload a large data to the blob in chunks.
        /// </summary>
        /// <typeparam name="T">Type of the content (Either Text blob or Stream blob or Byte blob).</typeparam>
        /// <param name="context">Context of the application.</param>
        /// <param name="blobData">Blob related information.</param>
        /// <remarks>
        /// To upload a file total number chunks must be greater than or equal to one. The chunk number must be greater than zero.
        /// </remarks>
        void Put<T>(Context context, ChunkBlobData<T> blobData);

        /// <summary>
        /// This can be used to download a large data from blob.
        /// </summary>
        /// <typeparam name="T">Type of the content (Either Text blob or Stream blob or Byte blob).</typeparam>
        /// <param name="context">Context of the application.</param>
        /// <param name="blobData">Blob related information.</param>
        /// <remarks>
        /// Content where the file needs to downloaded should be passed as input. 
        /// Chunk download will be done internally and copied to the content passed.
        /// </remarks>
        void Get<T>(Context context, BaseBlobData<T> blobData);
    }
}
