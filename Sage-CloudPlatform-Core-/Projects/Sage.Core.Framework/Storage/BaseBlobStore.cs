using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Sage.Core.Framework.Common;
using Sage.Core.Framework.Storage.Exceptions;
using Sage.Core.Utilities.Diagnostics;

namespace Sage.Core.Framework.Storage
{
    /// <summary>
    /// The core component for Blob storage.
    /// </summary>
    public  class BaseBlobStore
    {
        public BaseBlobStore(CloudBlobClient cloudBlobClient)
        {
            _internalBlobClient = cloudBlobClient;
        }

      
      


        #region Public Methods

        /// <summary>
        /// Creates BaseBlobStore object
        /// </summary>
        /// <param name="storageAccountConnectionString">Azure storage connection string.</param>
        /// <returns></returns>
        public static BaseBlobStore Create(string storageAccountConnectionString)
        {
            ArgumentValidator.ValidateNonNullReference(storageAccountConnectionString, "storageAccountConnectionString",
                                                       "BaseBlobStore.Ctor()");

            // Gets the blob service end point.
            return new BaseBlobStore(GetCloudBlobClient(storageAccountConnectionString));
        }

        /// <summary>
        /// Create the blob from the content passed. 
        /// </summary>
        /// <remarks>
        /// This is a simple stream upload.
        /// </remarks>
        /// <typeparam name="T">Type of the content.</typeparam>
        /// <param name="blobData">This will have the container name, blob name and the content.</param>
        public virtual void Put<T>(BaseBlobData<T> blobData)
        {
            Validate(blobData.Path, blobData.Name, "Put");

            try
            {
                var container = GetBlobContainer(blobData.Path, blobData.Name);
                var blob = GetBlockBlobReference(container, blobData.Name);

                blob.UploadFromStream(blobData.ContentStream);

            }
            catch (StorageException storageException)
            {
                throw new BlobStorageException(BlobResource.UploadError, blobData.Name, blobData.Path, storageException);
            }
        }

        /// <summary>
        /// Create a new version for the blob (creates the snapshot).
        /// </summary>
        /// <typeparam name="T">Type of the content.</typeparam>
        /// <param name="versionBlobData">This will have the container name, blob name and the content.</param>
        /// <returns></returns>
        public  void Put<T>(VersionBlobData<T> versionBlobData)
        {
            Validate(versionBlobData.BaseBlobData.Path, versionBlobData.BaseBlobData.Name, "Put");

            try
            {
                var container = GetBlobContainer(versionBlobData.BaseBlobData.Path, versionBlobData.BaseBlobData.Name);
                var blob = GetBlockBlobReference(container, versionBlobData.BaseBlobData.Name);

                blob.CreateSnapshot();

                blob.UploadFromStream(versionBlobData.BaseBlobData.ContentStream);
            }
            catch (StorageException storageException)
            {
                throw new BlobStorageException(BlobResource.BlobCreateSnapshotError, versionBlobData.BaseBlobData.Name,
                                               versionBlobData.BaseBlobData.Path,
                                               storageException);
            }
        }
        
        /// <summary>
        /// Upload individual blocks (chunks).
        /// </summary>
        /// <typeparam name="T">Type of the content.</typeparam>
        /// <param name="chunkBlobData">This will have the container name, blob name, chunk number and total chunk size.</param>
        public  void Put<T>(ChunkBlobData<T> chunkBlobData)
        {
            Validate(chunkBlobData.BaseBlobData.Path, chunkBlobData.BaseBlobData.Name, "Put");
            ArgumentValidator.ValidateMaxContentLength(chunkBlobData.BaseBlobData.ContentStream, "Content", "Put", MaxChunkSize);
            
            try
            {
                var container = GetBlobContainer(chunkBlobData.BaseBlobData.Path, chunkBlobData.BaseBlobData.Name);
                var blob = GetBlockBlobReference(container, chunkBlobData.BaseBlobData.Name);

                //A valid Base64 string value that identifies the block. Base64 string must be URL-encoded
                var blockIdBase64 = EncodeOffset(chunkBlobData.ChunkNumber);

                //An MD5 hash of the block content. This hash is used to verify the integrity of the block during transport.
                var contentMd5 = CalculateMd5(chunkBlobData.BaseBlobData.ContentStream);

                blob.PutBlock(blockIdBase64, chunkBlobData.BaseBlobData.ContentStream, contentMd5);
            }
            catch (StorageException storageException)
            {
                throw new BlobStorageException(BlobResource.PutBlockError, chunkBlobData.BaseBlobData.Name,
                                               chunkBlobData.BaseBlobData.Path, storageException);
            }
        }

        /// <summary>
        /// Method to commit all the uploaded blocks (chunks).
        /// </summary>
        /// <typeparam name="T">Type of the content.</typeparam>
        /// <param name="chunkBlobData">This will have the container name, blob name.</param>
        public  void CommitPut<T>(ChunkBlobData<T> chunkBlobData)
        {
            Validate(chunkBlobData.BaseBlobData.Path, chunkBlobData.BaseBlobData.Name, "Put");

            try
            {
                var container = GetBlobContainer(chunkBlobData.BaseBlobData.Path, chunkBlobData.BaseBlobData.Name);
                var blob = GetBlockBlobReference(container, chunkBlobData.BaseBlobData.Name);

                // Download all blocks (chunks).
                var blockBlobList = GetBlockListFromBlob(blob);
            
                // commit the list of uploaded blocks
                blob.PutBlockList(blockBlobList);
            }
            catch (StorageException storageException)
            {
                throw new BlobStorageException(BlobResource.CommitChunkUploadError, chunkBlobData.BaseBlobData.Name,
                                               chunkBlobData.BaseBlobData.Path, storageException);
            }
        }

        /// <summary>
        /// Download the blob.
        /// </summary>
        /// <remarks>
        /// The blob will be download as stream. Downloaded stream will be copied into the content passed.
        /// </remarks>
        /// <typeparam name="T">Type of the content.</typeparam>
        /// <param name="blobData">This will have the container name, blob name.</param>
        public virtual void Get<T>(BaseBlobData<T> blobData)
        {
            Validate(blobData.Path, blobData.Name, "Get");

            try
            {
                var container = GetBlobContainer(blobData.Path, blobData.Name);
                var blob = GetBlockBlobReference(container, blobData.Name);

              

                blob.DownloadToStream(blobData.ContentStream);

            }
            catch (StorageException storageException)
            {
                throw new BlobStorageException(BlobResource.DownloadError, blobData.Name, blobData.Path,
                                               storageException);
            }
        }

        /// <summary>
        /// Download the blob version (snapshot).
        /// </summary>
        /// <remarks>
        /// The blob snapshot will be download as stream. Downloaded stream will be copied into the content passed.
        /// </remarks>
        /// <typeparam name="T">Type of the content.</typeparam>
        /// <param name="versionBlobData">This will have the container name, blob name, snapshot datetime.</param>
        public void Get<T>(VersionBlobData<T> versionBlobData)
        {
            Validate(versionBlobData.BaseBlobData.Path, versionBlobData.BaseBlobData.Name, "Get");

            try
            {
                var container = GetBlobContainer(versionBlobData.BaseBlobData.Path, versionBlobData.BaseBlobData.Name);
                var blob = GetBlockBlobReference(container, versionBlobData.BaseBlobData.Name,
                                                 versionBlobData.SnapshotDateTime);

                blob.DownloadToStream(versionBlobData.BaseBlobData.ContentStream);

            }
            catch (StorageException storageException)
            {
                throw new BlobStorageException(BlobResource.DownloadError, versionBlobData.BaseBlobData.Name,
                                               versionBlobData.BaseBlobData.Path, storageException);
            }
        }

        /// <summary>
        /// Opens a stream for reading from the blob.
        /// This can be used to download the read the blob content in chunks. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="chunkBlobData">This will have the container name, blob name.</param>
        public Stream Get<T>(ChunkBlobData<T> chunkBlobData)
        {
            Validate(chunkBlobData.BaseBlobData.Path, chunkBlobData.BaseBlobData.Name, "Get");

            try
            {
                var container = GetBlobContainer(chunkBlobData.BaseBlobData.Path, chunkBlobData.BaseBlobData.Name);
                var blob = GetBlockBlobReference(container, chunkBlobData.BaseBlobData.Name);

                var ret = blob.OpenRead();
                return ret;
            }
            catch (StorageException storageException)
            {
                throw new BlobStorageException(BlobResource.DownloadError, chunkBlobData.BaseBlobData.Name,
                                               chunkBlobData.BaseBlobData.Path, storageException);
            }
        }

        /// <summary>
        /// Delete the blob if exists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="blobData">This will have the container name, blob name.</param>
        /// <returns></returns>
        public virtual bool Delete<T>(BaseBlobData<T> blobData)
        {
            Validate(blobData.Path, blobData.Name, "Delete");

            try
            {
                var container = GetBlobContainer(blobData.Path, blobData.Name);
                var blob = GetBlockBlobReference(container, blobData.Name);
                blob.Delete();
                return true;
            }
            catch (StorageException storageException)
            {
                throw new BlobStorageException(BlobResource.DeleteError, blobData.Name, blobData.Path, storageException);
            }
        }

        /// <summary>
        /// Delete the blob if exists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="versionBlobData">This will have the container name, blob name, snapshot datetime.</param>
        /// <returns></returns>
        public bool Delete<T>(VersionBlobData<T> versionBlobData)
        {
            Validate(versionBlobData.BaseBlobData.Path, versionBlobData.BaseBlobData.Name, "Delete");

            try
            {
                var container = GetBlobContainer(versionBlobData.BaseBlobData.Path, versionBlobData.BaseBlobData.Name);
                var blob = GetBlockBlobReference(container, versionBlobData.BaseBlobData.Name,
                                                 versionBlobData.SnapshotDateTime);
                blob.Delete();
                return true;
            }
            catch (StorageException storageException)
            {
                throw new BlobStorageException(BlobResource.DeleteError, versionBlobData.BaseBlobData.Name,
                                               versionBlobData.BaseBlobData.Path, storageException);
            }

        }

        /// <summary>
        /// Get snapshots
        /// </summary>
        /// <typeparam name="T"></typeparam>        
        /// <param name="blobData">This will have the container name, blob name,.</param>
        /// <returns></returns>
        public List<DateTime> GetVersions<T>(BaseBlobData<T> blobData)
        {
            Validate(blobData.Path, blobData.Name, "GetVersions");

            try
            {
                var container = GetBlobContainer(blobData.Path, blobData.Name);
                var blob = GetBlockBlobReference(container, blobData.Name);

                if (!blob.Exists())
                {
                    return null;
                }

                var snapshots = container.ListBlobs(
                    useFlatBlobListing: true,
                    blobListingDetails: BlobListingDetails.Snapshots
                    ).Where(item => ((ICloudBlob) item).SnapshotTime.HasValue && item.Uri.Equals(blob.Uri)).ToList();

                return snapshots.Select(snapshot =>
                    {
                        var dateTimeOffset = ((ICloudBlob) snapshot).SnapshotTime;
                        return dateTimeOffset != null ? dateTimeOffset.Value.UtcDateTime : new DateTime();
                    }).ToList();
            }
            catch (StorageException storageException)
            {
                throw new BlobStorageException(BlobResource.BlobContainerGetSnapshotError, blobData.Name, blobData.Path,
                                               storageException);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets the blob service end point.
        /// </summary>
        /// <param name="storageAccountConnectionString">Connection string from configuration.</param>
        /// <returns>Blob service endpoint</returns>
        protected static CloudBlobClient GetCloudBlobClient(string storageAccountConnectionString)
        {
            try
            {
                // Get storage account from the connection string.
                var azureStorageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);

                if (azureStorageAccount == null)
                {
                    throw new BlobStorageException(BlobResource.BlobConfigurationError);
                }

                var blobClient = azureStorageAccount.CreateCloudBlobClient();
                
                if (blobClient == null)
                {
                    throw new BlobStorageException(BlobResource.CreateCloudBlobClientError);
                }

                // Create the default retry policy for azure storage blob operation.
                blobClient.RetryPolicy = GetDefaultPolicy();

                return blobClient;
            }
            catch (StorageException storageException)
            {
                throw new BlobStorageException(BlobResource.CreateCloudBlobClientError, storageException);
            }
        }

        /// <summary>
        /// Create the default retry policy for azure storage blobs. 
        /// ExponentialRetry with 5 retries, and 2 sec interval between retries.
        /// </summary>
        /// <returns>Retry policy that performs a specified number of retries.</returns>
        private static ExponentialRetry GetDefaultPolicy()
        {
            const int retryCount = 5;
            var increment = TimeSpan.FromSeconds(2);
            var retryPolicy = new ExponentialRetry(increment, retryCount);

            return retryPolicy;
        }

        /// <summary>
        /// Get reference to the blob container.
        /// </summary>
        /// <param name="name">Name of the container.</param>
        /// <param name="blobName">Name of the blob.</param>
        /// <returns>Reference to the container.</returns>
        private CloudBlobContainer GetBlobContainer(string name, string blobName)
        {
            try
            {
                var blobContainer = _internalBlobClient.GetContainerReference(name);
                
                if (blobContainer == null)
                {
                    throw new BlobStorageException(BlobResource.BlobContainerCreationError, blobName, name);
                }

                if (blobContainer.CreateIfNotExists())
                {
                    // New container set permissions so that it is not accessible by anonymous user request.
                    // We will control access using shared access signatures at the blob level.
                    var blobContainerPermissions = new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Off
                        };

                    blobContainer.SetPermissions(blobContainerPermissions);
                }

                return blobContainer;
            }
            catch (StorageException storageException)
            {
                throw new BlobStorageException(BlobResource.BlobContainerCreationError, blobName, name, storageException);
            }
        }

        /// <summary>
        /// Get a reference to the blob with the given name within this container.
        /// Return a reference of the given type.
        /// </summary>
        /// <param name="container">Reference to the blob container.</param>
        /// <param name="name">Name of the blob.</param>
        /// <returns>Reference to the block blob.</returns>
        private CloudBlockBlob GetBlockBlobReference(CloudBlobContainer container, string name)
        {
            try
            {
                var blobReference = container.GetBlockBlobReference(name);
                if (blobReference == null)
                {
                    throw new BlobStorageException(BlobResource.BlobGetBlobReferenceError, name, container.Name);
                }
                return blobReference;
            }
            catch (StorageException storageException)
            {
                throw new BlobStorageException(BlobResource.BlobGetBlobReferenceError, name, container.Name,
                                               storageException);
            }
        }

        /// <summary>
        /// Get a reference to the blob with the given name within this container.
        /// </summary>
        /// <param name="container">Reference to the blob container.</param>
        /// <param name="name">Name of the blob.</param>
        /// <param name="snapshotDateTime">Snapshot date time.</param>
        /// <returns>Reference to the block blob.</returns>
        private CloudBlockBlob GetBlockBlobReference(CloudBlobContainer container, string name,
                                                            DateTime snapshotDateTime)
        {
            try
            {
                // The snapshot timestamp in UTC.
                var snapshot = new DateTime(snapshotDateTime.Ticks, DateTimeKind.Utc);

                var blobReference = container.GetBlockBlobReference(name, snapshot);

                if (blobReference == null)
                {
                    throw new BlobStorageException(BlobResource.BlobGetBlobReferenceError, name, container.Name);
                }

                return blobReference;
            }
            catch (StorageException storageException)
            {
                throw new BlobStorageException(BlobResource.BlobGetBlobReferenceError, name, container.Name,
                                               storageException);
            }
        }

        /// <summary>
        /// Download all blocks(chunks)
        /// </summary>
        /// <param name="blob">BlockBlob reference</param>
        /// <returns></returns>
        private IEnumerable<string> GetBlockListFromBlob(CloudBlockBlob blob)
        {
            // download the list of blocks that were uploaded
            IEnumerable<ListBlockItem> blockList = blob.DownloadBlockList(BlockListingFilter.All);

            
                // sort the blocks by their offset
                var orderedBlockList = blockList.OrderBy(block =>
                    {
                        var currentOffset = DecodeOffset(block.Name);
                        return currentOffset;
                    });

                // ensure that there are no gaps in the sequence
                long offset = 0;
                foreach (var currentOffset in orderedBlockList.Select(block => DecodeOffset(block.Name)))
                {
                    if (currentOffset != offset)
                    {
                        throw new BlobStorageException(BlobResource.MissingBlocksError, blob.Name,
                                                       blob.Container.Name);

                    }
                    offset += 1;
                }
                List<string> blockBlobList = orderedBlockList.Select(p => p.Name).ToList();
            

            return blockBlobList;
        }

        /// <summary>
        /// An MD5 hash of the block content.  This hash is used to verify the integrity of the block during transport. 
        /// When this header is specified, the storage service compares the hash of the content that has arrived with this header value.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string CalculateMd5(Stream input)
        {
            using (var cryptoService = MD5.Create())
            {
                var md5 = cryptoService.ComputeHash(input);
                input.Seek(0, SeekOrigin.Begin);
                return Convert.ToBase64String(md5);
            }
        }

        /// <summary>
        /// Base64 string must be URL-encoded.
        /// A valid Base64 string value that identifies the block. 
        /// Prior to encoding, the string must be less than or equal to 64 bytes in size. 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static string EncodeOffset(long offset)
        {
            return Convert.ToBase64String(BitConverter.GetBytes(offset));
        }

        /// <summary>
        /// Decode Encoded blockId
        /// </summary>
        /// <param name="blockId"></param>
        /// <returns></returns>
        private static long DecodeOffset(string blockId)
        {
            return BitConverter.ToInt64(Convert.FromBase64String(blockId), 0);
        }

        /// <summary>
        /// Validate the container name and blob name.
        /// </summary>
        /// <remarks>
        /// The Blob name can only contain letters in lowercase, numbers, or the following special characters:  . _ + - ! * ` ( ) $.
        /// The Container name  must start with a letter or number, and can contain only letters in lowercase, numbers, and the dash (-) character. 
        /// Every dash (-) character must be immediately preceded and followed by a letter or number; consecutive dashes are not permitted.
        /// </remarks>
        /// <param name="containerName">The container name</param>
        /// <param name="blobName">The blob name</param>
        /// <param name="source"></param>
        private static void Validate(string containerName, string blobName, string source)
        {
            ArgumentValidator.ValidateMinStringLength(blobName, "Name", source, 1);
            ArgumentValidator.ValidateMaxStringLength(blobName, "Name", source, 1024);
            ArgumentValidator.ValidateStringIsMatchForRegularExpression(blobName, "Name", source,
                                                                        BlobUtility.NameRegEx, BlobResource.BlobNameRegExMessage);
            ArgumentValidator.ValidateMinStringLength(containerName, "Path", source, 3);
            ArgumentValidator.ValidateMaxStringLength(containerName, "Path", source, 63);
            ArgumentValidator.ValidateStringIsMatchForRegularExpression(containerName, "Path", source,
                                                                        BlobUtility.PathRegEx, BlobResource.PathRegExMessage);
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Factory method that creates the azure storage account
        /// </summary>
        private readonly CloudBlobClient _internalBlobClient;

        /// <summary>
        /// Maximum allowed chunk size is 4MB in Bytes.
        /// </summary>
        private const long MaxChunkSize = 4194304;


        #endregion



      
    }
}
