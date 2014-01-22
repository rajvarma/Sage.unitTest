using System.Web.Mvc;
using Sage.Core.Cache;

namespace AspNetLab.Controllers
{
    public class BaseController : Controller
    {
        private static ICache _azureCache;

        public BaseController(ICache cache)
        {
            _azureCache = cache;
        }
        public ICache SageCache 
        {
            get
            {
                if (_azureCache == null)
                {
                    //_azureCache = new AzureCache();
                }
                return _azureCache;
            }
        }
    }
}