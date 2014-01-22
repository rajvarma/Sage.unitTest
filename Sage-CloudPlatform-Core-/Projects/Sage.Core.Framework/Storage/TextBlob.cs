using System.IO;

namespace Sage.Core.Framework.Storage
{
    /// <summary>
    /// Text (string) blob data.
    /// </summary>
    public class TextBlob : BaseBlobData<string>
    {
        protected internal override Stream ContentStream
        {
            get
            {
                var memStream = new MemoryStream();
                var writer = new StreamWriter(memStream);
                writer.Write(Content);
                writer.Flush();
                memStream.Position = 0;
                return memStream;
            }
            set
            {
                var reader = new StreamReader(value);
                Content = reader.ReadToEnd();
            }
        }
    }
}
