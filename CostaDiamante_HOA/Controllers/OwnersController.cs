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
using Microsoft.AspNet.Identity.Owin;

namespace CostaDiamante_HOA.Controllers
{
    public class OwnersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private Owner filterOwner(string id)
        {
            Owner owner = null;
            //If owner, show only his details
            if (User.IsInRole(ApplicationUser.RoleNames.OWNER)) {
                var iden = User.Identity;
                var userID = iden.GetUserId();
                owner = db.Owners.Find(userID);
            }
            //If admin, get the requested owner
            else if (User.IsInRole(ApplicationUser.RoleNames.ADMIN))
                owner = db.Owners.Find(id);

            return owner;
        }

        // GET: Owners
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Index()
        {
            var ownersTemp = db.Owners.ToList();

            var admins = db.Users.ToList().Where((adm) => ownersTemp
                .Find(own => own.Id == adm.Id) == null).ToList();

            var owners = (from user in ownersTemp
                      select new RegisterViewModel(user, UserManager)).ToList();

            ViewBag.admins = admins;

            return View(owners);
        }

        // GET: Owners/Details/5
        public ActionResult Details(string id, int year = 0,bool errorGuest=false)
        {

            year = year == 0 ? DateTime.Today.Year : year;
            
            Owner owner = null;
            owner = filterOwner(id);

            if (owner == null)
                return HttpNotFound();

            //Select all condoes not associated to be listed as options to associate
            ViewBag.notAssociatedCondos = db.Condoes.Where(con => String.IsNullOrEmpty(con.ownerID))
                .ToList();

            ViewBag.errorGuest = errorGuest;
            ViewBag.year = year;

            return View(owner);
        }

        [HttpGet]
        public JsonResult GetVisits(string id, DateTime startdate, DateTime enddate) //  DateTime startdate, DateTime enddate
        { 
            TimePeriod periodReported = new TimePeriod(startdate, enddate);
            List<Visit> visits = new List<Visit>();
            
            Owner owner = null;
            owner = filterOwner(id);
            if (owner != null)
            {
                visits = owner.visitsHistory.ToList();

                //Visits are filtered and prepared to be returned as JSON array creating a ViewModel List
                var listVisits = visits.Where(vis => periodReported
                                 .hasInside(vis.timePeriod.startDate))
                                 .OrderBy(vis => vis.timePeriod.startDate)
                                 .Select(vis => new Visit.VMVisits(vis));

                return Json(listVisits, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { });
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
                var owner = filterOwner(id);
                if (owner != null)
                {
                    var condos = owner.Condos;

                    var res = from condo in condos
                              select new { condoID = condo.condoID, condoName = condo.name, status = condo.getHOAStatusByYear(year) };

                    return Json(new { res = res, numReg = res.Count() }, JsonRequestBehavior.AllowGet);
                }
                else
                    errorMsg = "Owner not found by given ID";
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
