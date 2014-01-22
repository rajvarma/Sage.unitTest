using System.Collections.Generic;
using Sage.Core.Framework.Common;     //TODO: Remove this class during merge. Use Domain project's ContextChainLink instead.

namespace Sage.Core.Framework.Storage
{
    /// <summary>
    /// This can be used to store/retrieve different versions.
    /// </summary>
    public interface IVersionBlobStore
    {
        /// <summary>
        /// Upload a new version.
        /// </summary>
        /// <typeparam name="T">Type of the content (Either Text blob or Stream blob or Byte blob).</typeparam>
        /// <param name="context">Context of the application.</param>
        /// <param name="blobData">Blob related information.</param>
        /// <remarks>
        /// Uploads the content to the blob. Creates new version if already exists.
        /// </remarks>
        void Put<T>(Context context, BaseBlobData<T> blobData);

        /// <summary>
        /// Download the version.
        /// </summary>
        /// <typeparam name="T">Type of the content (Either Text blob or Stream blob or Byte blob).</typeparam>
        /// <param name="context">Context of the application.</param>
        /// <param name="blobData">Blob related information.</param>
        void Get<T>(Context context, VersionBlobData<T> blobData);

        /// <summary>
        /// Delete the version
        /// </summary>
        /// <typeparam name="T">Type of the content(Can be empty).</typeparam>
        /// <param name="context">Context of the application.</param>
        /// <param name="blobData">Blob related information.</param>
        /// <returns></returns>
        bool Delete<T>(Context context, VersionBlobData<T> blobData);

        /// <summary>
        /// Get all the version information.
        /// </summary>
        /// <typeparam name="T">Type of the content(Can be empty).</typeparam>
        /// <param name="context">Context of the application.</param>
        /// <param name="blobData">Blob related information.</param>
        /// <returns>Different version information. Returns empty list if there is only one version exists.</returns>
        List<VersionBlobData<T>> GetVersionInfo<T>(Context context, BaseBlobData<T> blobData);
    }
}
