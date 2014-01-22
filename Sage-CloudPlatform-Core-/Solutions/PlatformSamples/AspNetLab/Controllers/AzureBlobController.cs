using System;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using AspNetLab.Models;
using Sage.Core.Cache;
using Sage.Core.Framework.Storage;
using CoreCommon = Sage.Core.Framework.Common;

namespace AspNetLab.Controllers
{
    public class AzureBlobController : BaseController
    {
        public AzureBlobController(ICache cache, IBlobStore blob, IVersionBlobStore versionBlob,
                                   IChunkBlobStore chunkBlob)
            : base(cache)
        {
            Blob = blob;
            VersionBlob = versionBlob;
            ChunkBlob = chunkBlob;
            DomainContext = GetDomainContext();
        }

        #region BlobStore usage sample

        /// <summary>
        /// Simple upload landing page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Normal file upload
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload()
        {

            if (!ImageIsSelected(Request.Files))
            {
                ViewBag.HasUploadError = true;
                return View("Index");
            }

            string uniqueFileName = Request.Files[0].FileName.ToLower();

            Blob.Put(DomainContext, new StreamBlob { Content = Request.Files[0].InputStream, Name = uniqueFileName, Path = "sagelabs" });

            ViewBag.IsUploaded = true;
            ViewBag.HasUploadError = false;
            ViewBag.FileName = Request.Files[0].FileName;
            ViewBag.UniqueFileName = uniqueFileName;

            return View("Index");
        }

        /// <summary>
        /// Normal file download
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="uniqueFileName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Download(string fileName, string uniqueFileName)
        {
            ViewBag.FileName = fileName;
            ViewBag.UniqueFileName = uniqueFileName;
            ViewBag.IsUploaded = true;
            ControllerContext.HttpContext.Response.CacheControl = "private";
            ControllerContext.HttpContext.Response.AppendHeader("content-disposition", "attachment; filename=\"" + fileName + "\"");
            ControllerContext.HttpContext.Response.ContentType = "application/force-download";
            ControllerContext.HttpContext.Response.BufferOutput = false;

            Stream ret = new MemoryStream();

            Blob.Get(DomainContext, new StreamBlob { Content = Response.OutputStream, Name = uniqueFileName, Path = "sagelabs" });

            ret.Position = 0;
            ret.CopyTo(Response.OutputStream);
            Response.Flush();

            return null;
        }

        /// <summary>
        /// Simple Delete
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="uniqueFileName"></param>
        /// <param name="isChunk"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string fileName, string uniqueFileName, bool isChunk)
        {
            ViewBag.FileName = fileName;
            ViewBag.IsUploaded = false;

            ViewBag.IsDeleted = Blob.Delete(DomainContext, new StreamBlob { Name = uniqueFileName, Path = "sagelabs" });

            return isChunk ? View("ChunkIndex") : View("Index");
        }


        #endregion

        #region VersionBlobStore usage sample

        /// <summary>
        /// Version upload landing page
        /// </summary>
        /// <returns></returns>
        public ActionResult VersionIndex()
        {
            return View();
        }

        /// <summary>
        /// Upload new version
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadVersion()
        {

            if (!ImageIsSelected(Request.Files))
            {
                ViewBag.HasUploadError = true;
                return View("Index");
            }

            string uniqueFileName = Request.Files[0].FileName.ToLower();

            VersionBlob.Put(DomainContext, new StreamBlob { Content = Request.Files[0].InputStream, Name = uniqueFileName, Path = "sagelabs" });

            ViewBag.IsUploaded = true;
            ViewBag.HasUploadError = false;
            ViewBag.FileName = Request.Files[0].FileName;
            ViewBag.UniqueFileName = uniqueFileName;

            return View("VersionIndex");
        }

        /// <summary>
        /// Download a specific version
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="uniqueFileName"></param>
        /// <param name="versionNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DownloadVersion(string fileName, string uniqueFileName, int versionNumber)
        {
            ViewBag.FileName = fileName;
            ViewBag.UniqueFileName = uniqueFileName;
            ViewBag.IsUploaded = true;



            ControllerContext.HttpContext.Response.CacheControl = "private";
            ControllerContext.HttpContext.Response.AppendHeader("content-disposition",
                                                                "attachment; filename=\"" + fileName + "\"");
            ControllerContext.HttpContext.Response.ContentType = "application/force-download";
            ControllerContext.HttpContext.Response.BufferOutput = false;

            Stream ret = new MemoryStream();
            var versionList = VersionBlob.GetVersionInfo(DomainContext, new StreamBlob { Name = uniqueFileName, Path = "sagelabs" });
            var model = new VersionModel(versionList, fileName, uniqueFileName);
            var snapshot = model.VersionInfos.SingleOrDefault(x => x.VersionNumber == versionNumber).SnapshotDateTime;

            VersionBlob.Get(DomainContext,
                            new VersionBlobData<Stream>
                            {
                                BaseBlobData =
                                    new StreamBlob
                                    {
                                        Content = Response.OutputStream,
                                        Name = uniqueFileName,
                                        Path = "sagelabs"
                                    },
                                SnapshotDateTime = snapshot
                            });

            ret.Position = 0;
            ret.CopyTo(Response.OutputStream);
            Response.Flush();

            return null;
        }

        /// <summary>
        /// Get all versions of a file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="uniqueFileName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAllVersions(string fileName, string uniqueFileName)
        {
            ViewBag.FileName = fileName;

            var versionList = VersionBlob.GetVersionInfo(DomainContext, new StreamBlob { Name = uniqueFileName, Path = "sagelabs" });

            var model = new VersionModel(versionList, fileName, uniqueFileName);
            ViewBag.IsUploaded = true;
            ViewBag.HasUploadError = false;
            ViewBag.VersionListed = true;
            ViewBag.UniqueFileName = uniqueFileName;

            return View("VersionIndex", model);
        }

        /// <summary>
        /// Delete a specific version
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="uniqueFileName"></param>
        /// <param name="versionNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteVersion(string fileName, string uniqueFileName, int versionNumber)
        {
            ViewBag.FileName = fileName;
            ViewBag.UniqueFileName = uniqueFileName;
            ViewBag.IsUploaded = true;
            ViewBag.HasUploadError = false;
            ViewBag.VersionListed = true;

            var list = VersionBlob.GetVersionInfo(DomainContext, new StreamBlob { Name = uniqueFileName, Path = "sagelabs" });
            var model = new VersionModel(list, fileName, uniqueFileName);
            var snapshot = model.VersionInfos.SingleOrDefault(x => x.VersionNumber == versionNumber).SnapshotDateTime;

            VersionBlob.Delete(DomainContext,
                               new VersionBlobData<Stream>
                               {
                                   BaseBlobData =
                                       new StreamBlob
                                       {
                                           Content = Response.OutputStream,
                                           Name = uniqueFileName,
                                           Path = "sagelabs"
                                       },
                                   SnapshotDateTime = snapshot
                               });

            list = VersionBlob.GetVersionInfo(DomainContext, new StreamBlob { Name = uniqueFileName, Path = "sagelabs" });
            model = new VersionModel(list, fileName, uniqueFileName);
            return View("VersionIndex", model);
        }


        #endregion

        #region ChunkBlobStore usage sample

        /// <summary>
        /// Chunk upload landing page
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult ChunkIndex(string fileName = null)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                ViewBag.IsUploaded = true;
                ViewBag.HasUploadError = false;
                ViewBag.IsChunkUpload = true;
                ViewBag.FileName = fileName.ToLower();
                ViewBag.UniqueFileName = fileName.ToLower();
            }

            return View();
        }

        /// <summary>
        /// Chunk upload for large files.
        /// </summary>
        /// <param name="chunks"></param>
        /// <param name="chunk"></param>
        /// <param name="name"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadChunk(int? chunks, int? chunk, string name, string fileName)
        {
            ViewBag.FileName = fileName.ToLower();

            if (chunks.HasValue && chunk.HasValue)
            {
                var blobData = new ChunkBlobData<byte[]>
                {
                    BaseBlobData =
                        new ByteBlob { Content = GetUploadedFileContent(), Name = fileName.ToLower(), Path = "sagelabs" },
                    ChunkNumber = chunk.Value,
                    TotalChunkSize = chunks.Value
                };

                ChunkBlob.Put(DomainContext, blobData);
            }
            else
            {
                //normal upload
                fileName = Request.Files[0].FileName;
                string uniqueFileName = fileName.ToLower();
                Blob.Put(DomainContext, new StreamBlob { Content = Request.Files[0].InputStream, Name = uniqueFileName, Path = "sagelabs" });
            }

            ViewBag.IsUploaded = true;
            ViewBag.HasUploadError = false;
            ViewBag.UniqueFileName = fileName.ToLower();

            if (chunks == chunk + 1)
            {
                string redirectUrl = string.Format("{0}://{1}/{2}", Request.Url.Scheme,
                                                           Request.Headers["Host"],
                                                          string.Format(ActionUrl, fileName.ToLower()));
                return Json(new { isSuccess = true, redirectUrl });

            }

            return Json(new { isSuccess = true, redirectUrl = string.Empty });
        }

        /// <summary>
        /// Chunk download for large file download.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="uniqueFileName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DownloadChunk(string fileName, string uniqueFileName)
        {
            ViewBag.FileName = fileName;
            ViewBag.UniqueFileName = uniqueFileName;
            ViewBag.IsUploaded = true;
            ControllerContext.HttpContext.Response.CacheControl = "private";
            ControllerContext.HttpContext.Response.AppendHeader("content-disposition", "attachment; filename=\"" + fileName + "\"");
            ControllerContext.HttpContext.Response.ContentType = "application/force-download";
            ControllerContext.HttpContext.Response.BufferOutput = false;

            Stream ret = new MemoryStream();

            ChunkBlob.Get(DomainContext, new StreamBlob { Content = Response.OutputStream, Name = uniqueFileName, Path = "sagelabs" });

            ret.Position = 0;
            ret.CopyTo(Response.OutputStream);
            Response.Flush();

            return null;
        }


        #endregion
      
        private static bool ImageIsSelected(HttpFileCollectionBase files)
        {
            return !(files == null || files.Count == 0 || files[0].ContentLength == 0);
        }

        private static CoreCommon.Context GetDomainContext()
        {
            var domainContext = new CoreCommon.Context();
            var tenantContextChain = new CoreCommon.ContextChainLink
            {
                Id = new Guid("8ad8f0ef-b50c-4385-953e-cc05a9a83099")
            };


            domainContext.ContextChain = tenantContextChain;
            return domainContext;
        }

        private byte[] GetUploadedFileContent()
        {
            byte[] fileByteArray = null;
            if (Request.Files.Count > 0)
            {
                // Assume that each time this controller is executed there is only one file is getting uploaded
                Stream fileStream = Request.Files[0].InputStream;
                using (var binaryReader = new BinaryReader(fileStream))
                {
                    fileByteArray = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                }
            }
            return fileByteArray;
        }

        private readonly IBlobStore Blob;
        private readonly IVersionBlobStore VersionBlob;
        private readonly IChunkBlobStore ChunkBlob;
        private readonly CoreCommon.Context DomainContext;
        private const string ActionUrl = "AzureBlob/ChunkIndex?fileName={0}";

    }
}