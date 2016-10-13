using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleWebStore.Controllers
{

    
    public class HomeController : Controller
    {
        [NonAction]
        public ActionResult Index()
        {
            return View();
        }

        [NonAction]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [NonAction]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}