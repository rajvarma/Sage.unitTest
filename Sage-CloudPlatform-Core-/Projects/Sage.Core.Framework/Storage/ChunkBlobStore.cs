using System;
using Sage.Core.Framework.Common;
using Sage.Core.Framework.Configuration;
using Sage.Core.Framework.Storage.Exceptions;
using Sage.Core.Utilities.Diagnostics;

namespace Sage.Core.Framework.Storage
{
    /// <summary>
    /// This can be used to upload/download large files. 
    /// This can be used for chunk upload and when the last chunk is sent the upload process will be committed.
    /// Chunk Downloading is implemented internally and the content will be sent after downloading all chunks from the blob.  
    /// </summary>
    public sealed class ChunkBlobStore : IChunkBlobStore
    {
        // Constructor
        public ChunkBlobStore(IConfigurationManager configurationManager)
        {
            ArgumentValidator.ValidateNonNullReference(configurationManager, "configurationManager", "AzureBlob.Ctor()");
            baseBlobStore = CreateBaseBlobStore(configurationManager);
        }

        /// <summary>
        /// This can be used to upload a large data to the blob in chunks.
        /// </summary>
        /// <typeparam name="T">Type of the content (Either Text blob or Stream blob or Byte blob).</typeparam>
        /// <param name="context">Context of the application.</param>
        /// <param name="blobData">Blob related information.</param>
        /// <remarks>
        /// To upload a file total number chunks must be greater than or equal to one. The chunk number must be greater than zero.
        /// </remarks>
        public void Put<T>(Context context, ChunkBlobData<T> blobData)
        {
            ArgumentValidator.ValidateNonNullReference(context, "context", "ChunkBlobStore.Put");
            ArgumentValidator.ValidateNonNullReference(blobData, "blobData", "ChunkBlobStore.Put");
            ArgumentValidator.ValidateNonNullReference(blobData.BaseBlobData, "blobData.BlobData", "ChunkBlobStore.Put");
            ArgumentValidator.ValidateNonNullReference(blobData.BaseBlobData.Name, "blobData.BlobData.Name",
                                                       "ChunkBlobStore.Put");
            ArgumentValidator.ValidateNonNullReference(blobData.BaseBlobData.Path, "blobData.BlobData.Path",
                                                       "ChunkBlobStore.Put");
            ArgumentValidator.ValidateNonNullReference(blobData.BaseBlobData.Content, "blobData.BlobData.Content",
                                                       "ChunkBlobStore.Put");
            ArgumentValidator.ValidateNonNullReference(blobData.ChunkNumber, "blobData.ChunkNumber",
                                                       "ChunkBlobStore.Put");
            ArgumentValidator.ValidateNonNullReference(blobData.TotalChunkSize, "blobData.TotalChunkSize",
                                                       "ChunkBlobStore.Put");
            ArgumentValidator.ValidateMinIntegerValue(blobData.TotalChunkSize, "blobData.TotalChunkSize",
                                                      "ChunkBlobStore.Put", 1);

            try
            {
                // Upload block (chunk).
                baseBlobStore.Put(blobData);

                var doCommit = (blobData.ChunkNumber + 1) == blobData.TotalChunkSize;
                if (!doCommit) return;

                // Commit the upload.
                baseBlobStore.CommitPut(blobData);
            }
            catch (BlobStorageException storageException)
            {
                throw new Exception(storageException.ToString());
            }
            catch (Exception exception)
            {
                throw new Exception(BlobResource.UploadError, exception);
            }
        }

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
        public void Get<T>(Context context, BaseBlobData<T> blobData)
        {
            ArgumentValidator.ValidateNonNullReference(context, "context", "ChunkBlobStore.Get");
            ArgumentValidator.ValidateNonNullReference(blobData, "blobData", "ChunkBlobStore.Get");
            ArgumentValidator.ValidateNonNullReference(blobData.Name, "blobData.Name", "ChunkBlobStore.Get");
            ArgumentValidator.ValidateNonNullReference(blobData.Path, "blobData.Path", "ChunkBlobStore.Get");

            try
            {
                var chunkBlobData = new ChunkBlobData<T>
                    {
                        BaseBlobData = blobData
                    };

                using (var content = baseBlobStore.Get(chunkBlobData))
                {
                    // Read the blob content in chunks.
                    var buffer = new byte[ChunkSize];
                    var count = content.Read(buffer, 0, buffer.Length);
                    while (count > 0)
                    {
                        chunkBlobData.BaseBlobData.ContentStream.Write(buffer, 0, count);
                        count = content.Read(buffer, 0, buffer.Length);
                    }

                    // Set the stream pointer to the beginning.
                    chunkBlobData.BaseBlobData.ContentStream.Position = 0;
                }
            }
            catch (BlobStorageException storageException)
            {
                throw new Exception(storageException.ToString());
            }
            catch (Exception exception)
            {
                throw new Exception(BlobResource.DownloadError, exception);
            }
        }


        /// <summary>
        /// Get BaseBlobStore object.
        /// </summary>
        /// <param name="configurationManager"></param>
        /// <returns></returns>
        private BaseBlobStore CreateBaseBlobStore(IConfigurationManager configurationManager)
        {
            var storageAccountConnectionString = BlobUtility.GetConnectionString(configurationManager);

            return BaseBlobStore.Create(storageAccountConnectionString);
        }

        #region Private Properties

        /// <summary>
        /// BaseBlobStore instance.
        /// </summary>
        private readonly BaseBlobStore baseBlobStore;

        /// <summary>
        /// Download chunk size
        /// </summary>
        private const long ChunkSize = 65536;

        #endregion

    }
}
