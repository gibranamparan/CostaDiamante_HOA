using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sonora_HOA.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Sonora_HOA.Controllers
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
        public ActionResult Details(string id)
        {
            Owner owners = null;

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //If admin, get the requested owner
            else if (User.IsInRole(ApplicationUser.RoleNames.ADMIN))
                owners = db.Owners.Find(id);
            //If owner, show only his details
            else if (User.IsInRole(ApplicationUser.RoleNames.OWNER))
                owners = db.Owners.Find(User.Identity.GetUserId());

            if (owners == null)
                return HttpNotFound();

            ViewBag.notAssociatedCondos = db.Condoes.Where(con =>
                String.IsNullOrEmpty(con.ownerID)).ToList();

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
