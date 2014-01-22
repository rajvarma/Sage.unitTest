
using System.IO;

namespace Sage.Core.Framework.Storage
{
   /// <summary>
   /// Byte blob data (byte array).
   /// </summary>
    public class ByteBlob : BaseBlobData<byte[]>
    {
        protected internal override Stream ContentStream
        {
            get
            {
                var memStream = new MemoryStream(Content);
                return memStream;
            }
            set
            {
                using (var ms = new MemoryStream())
                {
                    value.CopyTo(ms);
                    Content = ms.ToArray();
                }
            }
        }
    }
}
