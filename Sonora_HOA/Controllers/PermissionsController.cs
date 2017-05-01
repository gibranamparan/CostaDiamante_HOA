using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sonora_HOA.Models;

namespace Sonora_HOA.Controllers
{
    public class PermissionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Permissions
        public ActionResult Index()
        {
            var permissions = db.Permissions.Include(p => p.condo);
            ViewBag.number = new SelectList(db.Condoes, "number", "name");
            ViewBag.guestID = new SelectList(db.Guests.ToList(), "guestID", "fullName");
            return View(permissions.ToList());
        }

        // GET: Permissions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permissions permissions = db.Permissions.Find(id);
            if (permissions == null)
            {
                return HttpNotFound();
            }
            return View(permissions);
        }

        // GET: Permissions/Create
        public ActionResult Create(string id)
        {
            var Permissions = db.Permissions.Where(p => p.condo.ownerID == id).ToList();
            ViewBag.condoID = Permissions.FirstOrDefault();
            ViewBag.number = new SelectList(db.Condoes, "number", "name");
            ViewBag.guestID = new SelectList(db.Guests.ToList(), "guestID", "fullName");
            return View();
        }

        // POST: Permissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "permissionsID,startDate,number,guestID")] Permissions permissions, string id)
        {
            if (ModelState.IsValid)
            {
                db.Permissions.Add(permissions);
                db.SaveChanges();
                return RedirectToAction("Create","Visits", new { id = id });
            }

            ViewBag.number = new SelectList(db.Condoes, "number", "name", permissions.number);
            ViewBag.guestID = new SelectList(db.Guests.ToList(), "guestID", "fullName", permissions.guestID);
            return View(permissions);
        }

        // GET: Permissions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permissions permissions = db.Permissions.Find(id);
            if (permissions == null)
            {
                return HttpNotFound();
            }
            ViewBag.number = new SelectList(db.Condoes, "number", "name", permissions.number);
            ViewBag.guestID = new SelectList(db.Guests.ToList(), "guestID", "fullName", permissions.guestID);
            return View(permissions);
        }

        // POST: Permissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "permissionsID,startDate,number,guestID")] Permissions permissions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(permissions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.number = new SelectList(db.Condoes, "number", "name", permissions.number);
            ViewBag.guestID = new SelectList(db.Guests.ToList(), "guestID", "fullName", permissions.guestID);
            return View(permissions);
        }

        // GET: Permissions/Delete/5
        public ActionResult Delete(int? id, string ownerID)
        {
            var Permissions = db.Permissions.Where(p => p.condo.ownerID == ownerID).ToList();
            ViewBag.condoID = Permissions.FirstOrDefault().condo.ownerID;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permissions permissions = db.Permissions.Find(id);
            if (permissions == null)
            {
                return HttpNotFound();
            }
            return View(permissions);
        }

        // POST: Permissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string ownerID)
        {
            Permissions permissions = db.Permissions.Find(id);
            db.Permissions.Remove(permissions);
            db.SaveChanges();
            return RedirectToAction("Create","Visits", new { id = ownerID });
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
