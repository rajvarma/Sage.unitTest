using System;

namespace VSProjectCleaner
{
    using System.IO;

    internal static class PathHelper
    {

        /// <summary>
        /// given a source path, computes a relative path to the target.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        public static string GetRelativePath(string sourcePath, string targetPath)
        {
            if (sourcePath == null) throw new ArgumentNullException("sourcePath");
            if (targetPath == null) throw new ArgumentNullException("targetPath");

            // Ensure the directory separator is at the end of the path
            Func<string, string> getFullName = delegate(string path)
            {

                if (Directory.Exists(path))
                {
                    if (path[path.Length - 1] != Path.DirectorySeparatorChar)
                    {
                        path += Path.DirectorySeparatorChar;
                    }
                }
                return path;
            };

            string path1FullName = getFullName(sourcePath);
            string path2FullName = getFullName(targetPath);

            var uri1 = new Uri(path1FullName);
            var uri2 = new Uri(path2FullName);
            var relativeUri = uri1.MakeRelativeUri(uri2);

            return relativeUri.OriginalString.Replace('/','\\');
        }
    }
}
