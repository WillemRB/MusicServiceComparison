using System.Web.Mvc;
using MusicServiceComparison.Repository;

namespace MusicServiceComparison.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet, Route("")]
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}