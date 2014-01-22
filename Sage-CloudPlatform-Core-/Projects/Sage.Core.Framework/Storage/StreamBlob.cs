using System.IO;

namespace Sage.Core.Framework.Storage
{
   /// <summary>
   /// Stream blob data.
   /// </summary>
    public class StreamBlob : BaseBlobData<Stream>
    {

        protected internal override Stream ContentStream
        {
            get { return Content; }
            set { Content = value; }
        }
    }
}
