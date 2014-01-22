using System;
using System.Collections.Generic;
using System.Linq;
using Sage.Core.Framework.Common;
using Sage.Core.Framework.Configuration;
using Sage.Core.Framework.Storage.Exceptions;
using Sage.Core.Utilities.Diagnostics;

namespace Sage.Core.Framework.Storage
{
    /// <summary>
    /// This can be used to create and maintain different versions.
    /// </summary>
    public class VersionBlobStore : IVersionBlobStore
    {
        // Constructor
        public VersionBlobStore(IConfigurationManager configurationManager)
        {
            ArgumentValidator.ValidateNonNullReference(configurationManager, "configurationManager",
                                                       "VersionBlobStore.Ctor()");
            baseBlobStore = CreateBaseBlobStore(configurationManager);
        }

        /// <summary>
        /// Upload a new version.
        /// </summary>
        /// <typeparam name="T">Type of the content (Either Text blob or Stream blob or Byte blob).</typeparam>
        /// <param name="context">Context of the application.</param>
        /// <param name="blobData">Blob related information.</param>
        /// <remarks>
        /// Uploads the content to the blob. Creates new version if already exists.
        /// </remarks>
        public void Put<T>(Context context, BaseBlobData<T> blobData)
        {
            ArgumentValidator.ValidateNonNullReference(context, "context", "VersionBlobStore.Put");
            ArgumentValidator.ValidateNonNullReference(blobData, "blobData", "VersionBlobStore.Put");
            ArgumentValidator.ValidateNonNullReference(blobData.Name, "blobData.Name", "VersionBlobStore.Put");
            ArgumentValidator.ValidateNonNullReference(blobData.Path, "blobData.Path", "VersionBlobStore.Put");
            ArgumentValidator.ValidateNonNullReference(blobData.Content, "blobData.Content", "VersionBlobStore.Put");

            try
            {

                // Create new version if there is already any version present
                if (baseBlobStore.GetVersions(blobData) != null)
                {
                    var versionBlobData = new VersionBlobData<T> {BaseBlobData = blobData};
                    baseBlobStore.Put(versionBlobData);
                }
                else
                {
                    // Upload first version.
                    baseBlobStore.Put(blobData);
                }
            }
            catch (BlobStorageException storageException)
            {
                throw new Exception(BlobResource.UploadError, storageException);
            }
            catch (Exception exception)
            {
                throw new Exception(BlobResource.UploadError, exception);
            }
        }

        /// <summary>
        /// Download the version.
        /// </summary>
        /// <typeparam name="T">Type of the content (Either Text blob or Stream blob or Byte blob).</typeparam>
        /// <param name="context">Context of the application.</param>
        /// <param name="blobData">Blob related information.</param>
        public void Get<T>(Context context, VersionBlobData<T> blobData)
        {
            ArgumentValidator.ValidateNonNullReference(context, "context", "VersionBlobStore.Get");
            ArgumentValidator.ValidateNonNullReference(blobData, "blobData", "VersionBlobStore.Get");
            ArgumentValidator.ValidateNonNullReference(blobData.BaseBlobData, "blobData.BlobData", "VersionBlobStore.Get");
            ArgumentValidator.ValidateNonNullReference(blobData.SnapshotDateTime, "blobData.SnapshotDateTime",
                                                       "VersionBlobStore.Get");
            ArgumentValidator.ValidateNonNullReference(blobData.BaseBlobData.Name, "blobData.BlobData.Name",
                                                       "VersionBlobStore.Get");
            ArgumentValidator.ValidateNonNullReference(blobData.BaseBlobData.Path, "blobData.BlobData.Path",
                                                       "VersionBlobStore.Get");

            try
            {
                baseBlobStore.Get(blobData);
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
        /// Delete the version
        /// </summary>
        /// <typeparam name="T">Type of the content(Can be empty).</typeparam>
        /// <param name="context">Context of the application.</param>
        /// <param name="blobData">Blob related information.</param>
        /// <returns></returns>
        public bool Delete<T>(Context context, VersionBlobData<T> blobData)
        {
            ArgumentValidator.ValidateNonNullReference(context, "context", "VersionBlobStore.Delete");
            ArgumentValidator.ValidateNonNullReference(blobData, "blobData", "VersionBlobStore.Delete");
            ArgumentValidator.ValidateNonNullReference(blobData.BaseBlobData, "blobData.BlobData", "VersionBlobStore.Delete");
            ArgumentValidator.ValidateNonNullReference(blobData.SnapshotDateTime, "blobData.SnapshotDateTime",
                                                       "VersionBlobStore.Get");
            ArgumentValidator.ValidateNonNullReference(blobData.BaseBlobData.Name, "blobData.BlobData.Name",
                                                       "VersionBlobStore.Delete");
            ArgumentValidator.ValidateNonNullReference(blobData.BaseBlobData.Path, "blobData.BlobData.Path",
                                                       "VersionBlobStore.Delete");
         


            try
            {
                return baseBlobStore.Delete(blobData);
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

        /// <summary>
        /// Get all the version information.
        /// </summary>
        /// <typeparam name="T">Type of the content(Can be empty).</typeparam>
        /// <param name="context">Context of the application.</param>
        /// <param name="blobData">Blob related information.</param>
        /// <returns>Different version information. Returns empty list if there is only one version exists.</returns>
        public List<VersionBlobData<T>> GetVersionInfo<T>(Context context, BaseBlobData<T> blobData)
        {
            ArgumentValidator.ValidateNonNullReference(context, "context", "VersionBlobStore.GetVersionInfo");
            ArgumentValidator.ValidateNonNullReference(blobData, "blobData", "VersionBlobStore.GetVersionInfo");
            ArgumentValidator.ValidateNonNullReference(blobData.Name, "blobData.Name", "VersionBlobStore.Put");
            ArgumentValidator.ValidateNonNullReference(blobData.Path, "blobData.Path", "VersionBlobStore.Put");

            try
            {
                var versionInformations = new List<VersionBlobData<T>>();
                var snapshots = baseBlobStore.GetVersions(blobData);

                if (snapshots == null)
                {
                    throw new BlobStorageException(BlobResource.BlobDoesntExists, blobData.Name, blobData.Path);
                }

                if (snapshots.Count > 0)
                {
                    versionInformations.AddRange(
                        snapshots.Select(
                            snapshot => new VersionBlobData<T> {BaseBlobData = blobData, SnapshotDateTime = snapshot}));
                }

                return versionInformations;
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

        /// <summary>
        /// BaseBlobStore instance.
        /// </summary>
        private readonly BaseBlobStore baseBlobStore;
    }
}
