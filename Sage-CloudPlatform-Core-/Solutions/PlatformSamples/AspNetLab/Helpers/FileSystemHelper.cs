using System;
using System.IO;
using System.Web.Hosting;

namespace AspNetLab.Helpers
{
    public class FileSystemHelper
    {
        private const string StaticContextFolder = "~/App_Data/Static";
        public FileInfo GetFileInfo(string folderName, string fileName, string ifNoneMatch = null)
        {
            if (String.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException("folderName");
            }
            if (String.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            var path = BuildPath(StaticContextFolder, folderName, fileName);

            // Get the last modified date of the file and use that as the ContentID
            return new FileInfo(path);

        }

        private static string BuildPath(string fileStorageDirectory, string folderName, string fileName)
        {
            // Resolve the file storage directory
            fileStorageDirectory = ResolvePath(fileStorageDirectory);

            return Path.Combine(fileStorageDirectory, folderName, fileName);
        }

        private static string ResolvePath(string fileStorageDirectory)
        {
            if (fileStorageDirectory.StartsWith("~/", StringComparison.OrdinalIgnoreCase) && HostingEnvironment.IsHosted)
            {
                fileStorageDirectory = HostingEnvironment.MapPath(fileStorageDirectory);
            }
            return fileStorageDirectory;
        }
    }
}