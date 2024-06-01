using System;
using System.Web.Mvc;

namespace Project_3.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        // GET: Home/About
        public ActionResult About()
        {
            return View();
        }

        [HttpGet]
        // GET: Home/Contact
        public ActionResult Contact()
        {
            return View();
        }
    }
}