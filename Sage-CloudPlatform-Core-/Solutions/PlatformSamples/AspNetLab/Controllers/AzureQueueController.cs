using Sage.Core.Cache;
using Sage.Core.Framework.Storage;
using System;
using System.Web.Mvc;

namespace AspNetLab.Controllers
{
    public class AzureQueueController : BaseController
    {
        public IQueue Queue { get; set; }
        public AzureQueueController(ICache cache, IQueue queue): base(cache)
        {
            Queue = queue;
        }
        //
        // GET: /AzureQueue/
        public ActionResult Index()
        {
            Queue.Enqueue(new AzureQueueMessage("Simple Message in queue"));
            Queue.Enqueue(new AzureQueueMessage("Simple Message timed in queue"), DateTime.Now.AddMinutes(5));

            IQueueMessage message = Queue.Dequeue();

            if (message != null)
            {
                Queue.ExtendLease(message);
                Queue.Delete(message);
            }
            Queue.Clear();
                
            return View();
        }
	}
}