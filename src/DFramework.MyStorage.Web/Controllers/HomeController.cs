using System.Web.Mvc;

namespace DFramework.MyStorage.Web.Controllers
{
    public class HomeController : MyStorageControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}