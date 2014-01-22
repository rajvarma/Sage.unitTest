using System;

namespace VSProjectCleaner
{
    using System.IO;

    internal class Settings
    {
        public Settings(string rootCodeDirectory, string targetDirectory)
        {
            if (!Directory.Exists(rootCodeDirectory)) throw new ArgumentException("Root directory does not exist");
            if (!Directory.Exists(targetDirectory)) throw new ArgumentException("Target directory does not exist");

            this.RootDirectory = rootCodeDirectory;
            this.TargetDirectory = targetDirectory;
            this.Recursive = true;
        }

        public string RootDirectory { get; private set; }

        public string TargetDirectory { get; private set; }

        public bool Recursive { get; private set;  }

        public string SnkFilePath {
            get
            {
                return Path.Combine(RootDirectory, @"Build\SageCommonPlatform.snk");
            }
        } 
    }
}
