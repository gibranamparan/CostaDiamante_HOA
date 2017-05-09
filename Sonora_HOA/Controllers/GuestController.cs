using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sonora_HOA.Models;
using static Sonora_HOA.GeneralTools.FiltrosDeSolicitudes;

namespace Sonora_HOA.Controllers
{
    [Authorize]
    public class GuestController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Guest
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Index(string id)
        {
            ViewBag.Ownerid = id;
            return View(db.Guests.ToList());
        }

        // GET: Guest/Details/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Guest guest = db.Guests.Find(id);
            if (guest == null)
            {
                return HttpNotFound();
            }
            return View(guest);
        }

        // GET: Guest/Create
        [Authorize(Roles = ApplicationUser.RoleNames.OWNER+","+ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Create()
        {
            
            return View();
        }

        // POST: Guest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = ApplicationUser.RoleNames.OWNER+","+ApplicationUser.RoleNames.ADMIN)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ownerID,name,lastName")] Guest guest)
        {
            if (ModelState.IsValid)
            {
                db.Guests.Add(guest);
                db.SaveChanges();
                return RedirectToAction("Details","Owners",new { Id = guest.ownerID});
            }

            return View();
        }

        // GET: Guest/Edit/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN + "," + ApplicationUser.RoleNames.OWNER)]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Guest guest = db.Guests.Find(id);
            if (guest == null)
            {
                return HttpNotFound();
            }
            return View(guest);
        }

        // POST: Guest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN + "," + ApplicationUser.RoleNames.OWNER)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,guestID,name,lastName")] Guest guest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(guest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(guest);
        }

        // POST: Guest/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public JsonResult Delete(int id = 0)
        {
            Guest guest = db.Guests.Find(id);
            if (guest == null) 
                return Json(new { regSaved = 0 });
            else
            {
                db.Guests.Remove(guest);
                int regSaved = db.SaveChanges();
                return Json(new { regSaved = regSaved });
            }
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
