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
        public ActionResult Details(string id, bool errorGuest=false)
        {
            Owner owners = null;
            List<Visit> visits = new List<Visit>();

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

            visits = owners.visitsHistory.ToList();

            ViewBag.errorGuest = errorGuest;
            ViewBag.result = visits;

            return View(owners);
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
