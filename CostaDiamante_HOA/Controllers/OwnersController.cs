using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CostaDiamante_HOA.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using CostaDiamante_HOA.GeneralTools;

namespace CostaDiamante_HOA.Controllers
{
    [Authorize]
    public class OwnersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Owners
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Index()
        {
            var owners = db.Owners.ToList();
            var admins = db.Users.ToList().Where((adm) => owners
                .Find(own => own.Id == adm.Id) == null).ToList();
            ViewBag.admins = admins;

            return View(owners);
        }

        // GET: Owners/Details/5
        public ActionResult Details(string id, int year = 0,bool errorGuest=false)
        {
            year = year == 0 ? DateTime.Today.Year : year;

            Owner owners = null;
            //If owner, show only his details
            if (User.IsInRole(ApplicationUser.RoleNames.OWNER))
                owners = db.Owners.Find(User.Identity.GetUserId());
            else if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //If admin, get the requested owner
            else if (User.IsInRole(ApplicationUser.RoleNames.ADMIN))
                owners = db.Owners.Find(id);

            if (owners == null)
                return HttpNotFound();

            ViewBag.notAssociatedCondos = db.Condoes.Where(con =>
                String.IsNullOrEmpty(con.ownerID)).ToList();

            ViewBag.errorGuest = errorGuest;
            ViewBag.year = year;

            return View(owners);
        }

        [HttpGet]
        public JsonResult GetVisits(string id, DateTime startdate, DateTime enddate) //  DateTime startdate, DateTime enddate
        { 
            TimePeriod periodReported = new TimePeriod(startdate, enddate);
            List<Visit> visits = new List<Visit>();

            Owner owner = db.Owners.Find(id);
            visits = owner.visitsHistory.ToList();

            // Se filtran visitas
            var listVisits = visits.Where(vis => periodReported
                             .hasInside(vis.timePeriod.startDate))
                             .OrderBy(vis => vis.timePeriod.startDate)
                             .Select(vis => new Visit.VMVisits(vis));

            return Json(listVisits, JsonRequestBehavior.AllowGet);
            //return Json(listVisits);
        }

        // GET: Payments
        [HttpGet]
        //[ValidateHeaderAntiForgeryToken]
        public JsonResult GetQuartersStatus(string id, int year)
        {
            int numReg = 0;
            string errorMsg = string.Empty;
            try
            {
                var condos = db.Owners.Find(id).Condos;

                var res = from condo in condos
                          select new { condoID = condo.condoID, condoName = condo.name, status = condo.getHOAStatusByYear(year) };

                return Json(new { res = res, numReg = res.Count() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                errorMsg = String.Format("{0}. Details: {1}", e.Message, e.InnerException.Message);
            }

            return Json(new { numReg = numReg, errorMsg = errorMsg }, JsonRequestBehavior.AllowGet);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
