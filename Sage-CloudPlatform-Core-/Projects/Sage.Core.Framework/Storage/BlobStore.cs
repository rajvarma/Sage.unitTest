using System;
using Microsoft.WindowsAzure.Storage.Blob;
using Sage.Core.Framework.Common;
using Sage.Core.Framework.Configuration;
using Sage.Core.Framework.Storage.Exceptions;
using Sage.Core.Utilities.Diagnostics;

namespace Sage.Core.Framework.Storage
{
    /// <summary>
    /// Basic blob storage implementations.
    /// </summary>
    public class BlobStore : BaseBlobStore ,IBlobStore
    {
        // Constructor
        public BlobStore(IConfigurationManager configurationManager)
            : base(GetBlobClient(configurationManager))
        {
            ArgumentValidator.ValidateNonNullReference(configurationManager, "configurationManager", "AzureBlob.Ctor()");
          
        }


        /// <summary>
        ///  Upload content to blob storage.
        /// </summary>
        /// <typeparam name="T">Content type (Stream, byte array or text)</typeparam>
        /// <param name="context">Context of the application</param>
        /// <param name="blobData">Data to create blob</param>
        public  virtual void Put<T>(Context context, BaseBlobData<T> blobData)
        {
            ArgumentValidator.ValidateNonNullReference(context, "context", "BlobStore.Put");
            ArgumentValidator.ValidateNonNullReference(context.ContextChain, "context.ContextChain", "BlobStore.Put");
            ArgumentValidator.ValidateNonNullReference(blobData, "blobData", "BlobStore.Put");
            ArgumentValidator.ValidateNonNullReference(blobData.Content, "blobData.Content", "BlobStore.Put");
            ArgumentValidator.ValidateNonNullReference(blobData.Name, "blobData.Name", "BlobStore.Put");
            ArgumentValidator.ValidateNonNullReference(blobData.Path, "blobData.Path", "BlobStore.Put");

            try
            {
                base.Put(blobData);
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
        /// Download a content from blob storage.
        /// </summary>
        /// <typeparam name="T">Content type (Stream, byte array or text) </typeparam>
        /// <param name="context">Context of the application</param>
        /// <param name="blobData">Data to get the blob</param>
        public void Get<T>(Context context, BaseBlobData<T> blobData)
        {
            ArgumentValidator.ValidateNonNullReference(context, "context", "BlobStore.Get");
            ArgumentValidator.ValidateNonNullReference(blobData, "blobData", "BlobStore.Get");
            ArgumentValidator.ValidateNonNullReference(blobData.Name, "blobData.Name", "BlobStore.Get");
            ArgumentValidator.ValidateNonNullReference(blobData.Path, "blobData.Path", "BlobStore.Get");

            try
            {
                base.Get(blobData);
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
        /// Delete content from the blob.
        /// </summary>
        /// <param name="context">Context of the application</param>
        /// <param name="blobData">Data to delete blob</param>
        public bool Delete<T>(Context context, BaseBlobData<T> blobData)
        {
            ArgumentValidator.ValidateNonNullReference(context, "context", "BlobStore.Delete");
            ArgumentValidator.ValidateNonNullReference(blobData, "blobData", "BlobStore.Delete");
            ArgumentValidator.ValidateNonNullReference(blobData.Name, "blobData.Name", "BlobStore.Delete");
            ArgumentValidator.ValidateNonNullReference(blobData.Path, "blobData.Path", "BlobStore.Delete");

            try
            {
                return base.Delete(blobData);
            }
            catch (BlobStorageException storageException)
            {
                throw new Exception(storageException.ToString());
            }
            catch (Exception exception)
            {
                throw new Exception(BlobResource.DeleteError, exception);
            }
        }
        

        public static CloudBlobClient GetBlobClient(IConfigurationManager configurationManager)
        {
            var storageAccountConnectionString = BlobUtility.GetConnectionString(configurationManager);
            ArgumentValidator.ValidateNonNullReference(storageAccountConnectionString, "storageAccountConnectionString",
                                                       "BaseBlobStore.Ctor()");

            // Gets the blob service end point.
            return GetCloudBlobClient(storageAccountConnectionString);
        }
    }
}
