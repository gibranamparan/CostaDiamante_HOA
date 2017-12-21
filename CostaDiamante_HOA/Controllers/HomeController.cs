using CostaDiamante_HOA.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CostaDiamante_HOA.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult UserManual()
        {
            return View();
        }

        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole(ApplicationUser.RoleNames.ADMIN))
                return RedirectToAction("Index", "Visits");
            else
                return RedirectToAction("Details", "Owners");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}