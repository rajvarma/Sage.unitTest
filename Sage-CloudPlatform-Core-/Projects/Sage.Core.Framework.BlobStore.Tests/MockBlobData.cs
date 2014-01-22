using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sage.Core.Framework.Storage;

namespace Sage.Core.Framework.BlobStore.Tests
{
    internal class MockBlobData : BaseBlobData<Stream>
    {
        public MockBlobData()
        {
            Name = Guid.NewGuid().ToString();
            Path = "sage";
            Content = new MemoryStream();

        }
    }
}
