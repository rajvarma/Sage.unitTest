using Sage.Core.Framework.Common;   //TODO: Remove this class during merge. Use Domain project's ContextChainLink instead.

namespace Sage.Core.Framework.Storage
{
    /// <summary>
    /// This can be used to store/retrieve content to/from blob storage.
    /// </summary>
    public interface IBlobStore
    {
        /// <summary>
        ///  Upload content to blob storage.
        /// </summary>
        /// <typeparam name="T">Content type (Stream, byte array or text)</typeparam>
        /// <param name="context">Context of the application</param>
        /// <param name="blobData">Data to create blob</param>
        void Put<T>(Context context, BaseBlobData<T> blobData);

        /// <summary>
        /// Download a content from blob storage.
        /// </summary>
        /// <typeparam name="T">Content type (Stream, byte array or text) </typeparam>
        /// <param name="context">Context of the application</param>
        /// <param name="blobData">Data to get the blob</param>
        void Get<T>(Context context, BaseBlobData<T> blobData);

        /// <summary>
        /// Delete content from blob storage.
        /// </summary>
        /// <param name="context">Context of the application</param>
        /// <param name="blobData">Data to delete blob</param>
        bool Delete<T>(Context context, BaseBlobData<T> blobData);

       
    }
}
