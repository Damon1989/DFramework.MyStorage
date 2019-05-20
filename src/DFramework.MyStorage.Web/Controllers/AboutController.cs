using System.Web.Mvc;

namespace DFramework.MyStorage.Web.Controllers
{
    public class AboutController : MyStorageControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}