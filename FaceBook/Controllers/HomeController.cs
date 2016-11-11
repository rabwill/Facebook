using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FaceBook.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Message = "This is your wall page";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "This is about you";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "This is your contact page.";

            return View();
        }
    }
}