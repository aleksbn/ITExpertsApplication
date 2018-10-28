using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITExpertsApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "About the project";

            return View();
        }

        public ActionResult Project()
        {
            ViewBag.Title = "About the project";

            return View();
        }
    }
}
