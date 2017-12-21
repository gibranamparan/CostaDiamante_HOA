using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CostaDiamante_HOA.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult NotFound()
        {
            return View();
        }

        // GET: Error
        public ActionResult Forbidden()
        {
            return View();
        }

        // GET: Error
        public ActionResult BadRequest()
        {
            return View();
        }
    }
}