using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sage.Core.Cache;

namespace AspNetLab.Controllers
{
    public class AzureCacheController : BaseController
    {
        public AzureCacheController(ICache cache) : base(cache)
        {
            
        }

        //
        // GET: /AzureCacheTest/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SetCache(string txtName)
        {
            SageCache.SetCacheItem("SampleCacheValue", txtName);
            ViewBag.IsSaved = true;
            return View("Index");
        }

        public ActionResult GetCache(string txtName)
        {
            ViewBag.IsSaved = true;
            ViewBag.CachedValue = SageCache.GetCacheItem("SampleCacheValue");
            return View("Index");
        }

        public ActionResult RemoveCache()
        {
            SageCache.RemoveCacheItem("SampleCacheValue");
            ViewBag.IsSaved = false;
            ViewBag.CachedValue = null;
            return View("Index");
        }
	}
}