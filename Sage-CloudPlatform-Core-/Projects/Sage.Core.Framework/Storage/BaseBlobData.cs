using System.IO;

namespace Sage.Core.Framework.Storage
{
    public class BaseBlobData<T>
    {
        public BaseBlobData()
        {
            
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public T Content { get; set; }
        
        protected internal virtual Stream ContentStream { get; set; }
    }
}
