using System.Diagnostics.Eventing.Reader;
using MarkdownSharp;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace AspNetLab.Helpers
{
    public class MarkdownHelper
    {
        private const string MDFileExtension = ".md";
        public const string ContentFolderName = "content";
        private readonly FileSystemHelper _fileSystemHelper;
        public MarkdownHelper()
        {
            _fileSystemHelper = new FileSystemHelper();
        }
        private readonly ConcurrentDictionary<string, string> _mdCache = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public string GetMarkdownContent(string fileName)
        {
            string item;
            if (_mdCache.TryGetValue(fileName, out item))
            {
                return item;
            }
            var fileNameWithExtension = fileName + MDFileExtension;

            var fileInfo = _fileSystemHelper.GetFileInfo(ContentFolderName, fileNameWithExtension);

            var result = string.Empty;
            if (fileInfo != null)
            {
                using (var stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var textToTransfor = reader.ReadToEnd();
                        result = (new Markdown()).Transform(textToTransfor).Trim();
                    }
                }
            }
    #if !DEBUG
            _mdCache.AddOrUpdate(fileName, result, (_, __) => result);
    #endif

            return result;
        }
    }

}