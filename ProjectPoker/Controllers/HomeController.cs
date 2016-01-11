
using System.Web.Mvc;

namespace ProjectPoker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Session["state"] = "Home";
            return View();
        }

    }
}