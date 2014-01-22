using System;
using System.Collections.Generic;
using Sage.Core.Framework.Storage;

namespace AspNetLab.Models
{
    public class VersionModel
    {
        public VersionModel(List<VersionBlobData<System.IO.Stream>> versionList, string fileName, string uniqueFileName)
        {
            VersionInfos = new List<VersionInfo>();
            UniqueFileName = uniqueFileName;
            FileName = fileName;
            int count = 0;
            versionList.ForEach(x => VersionInfos.Add(new VersionInfo { VersionNumber = ++count, SnapshotDateTime = x.SnapshotDateTime.ToUniversalTime() }));
        }

        public string UniqueFileName { get; set; }

        public string FileName { get; set; }

        public List<VersionInfo> VersionInfos { get; set; }
    }

    public class VersionInfo
    {
        public int VersionNumber { get; set; }
        public DateTime SnapshotDateTime { get; set; }
    }
}