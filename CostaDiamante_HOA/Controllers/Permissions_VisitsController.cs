using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CostaDiamante_HOA.Models;

namespace CostaDiamante_HOA.Controllers
{
    [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
    public class Permissions_VisitsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Permissions_Visits
        public ActionResult Index()
        {
            var permissions_Visits = db.Permissions_Visits.Include(p => p.permissions).Include(p => p.visits);
            return View(permissions_Visits.ToList());
        }

        // GET: Permissions_Visits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permissions_Visits permissions_Visits = db.Permissions_Visits.Find(id);
            if (permissions_Visits == null)
            {
                return HttpNotFound();
            }
            return View(permissions_Visits);
        }

        // GET: Permissions_Visits/Create
        public ActionResult Create()
        {
            ViewBag.permissionsID = new SelectList(db.Permissions, "permissionsID", "permissionsID");
            ViewBag.visitsID = new SelectList(db.Visits, "visitsID", "visitsID");
            return View();
        }

        // POST: Permissions_Visits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "permissions_visitsID,permissionsID,visitsID")] Permissions_Visits permissions_Visits)
        {
            if (ModelState.IsValid)
            {
                db.Permissions_Visits.Add(permissions_Visits);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.permissionsID = new SelectList(db.Permissions, "permissionsID", "permissionsID", permissions_Visits.permissionsID);
            ViewBag.visitsID = new SelectList(db.Visits, "visitsID", "visitsID", permissions_Visits.visitsID);
            return View(permissions_Visits);
        }

        // GET: Permissions_Visits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permissions_Visits permissions_Visits = db.Permissions_Visits.Find(id);
            if (permissions_Visits == null)
            {
                return HttpNotFound();
            }
            ViewBag.permissionsID = new SelectList(db.Permissions, "permissionsID", "permissionsID", permissions_Visits.permissionsID);
            ViewBag.visitsID = new SelectList(db.Visits, "visitsID", "visitsID", permissions_Visits.visitsID);
            return View(permissions_Visits);
        }

        // POST: Permissions_Visits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "permissions_visitsID,permissionsID,visitsID")] Permissions_Visits permissions_Visits)
        {
            if (ModelState.IsValid)
            {
                db.Entry(permissions_Visits).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.permissionsID = new SelectList(db.Permissions, "permissionsID", "permissionsID", permissions_Visits.permissionsID);
            ViewBag.visitsID = new SelectList(db.Visits, "visitsID", "visitsID", permissions_Visits.visitsID);
            return View(permissions_Visits);
        }

        // GET: Permissions_Visits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permissions_Visits permissions_Visits = db.Permissions_Visits.Find(id);
            int visitID = permissions_Visits.visitsID;
            if (permissions_Visits == null)
            {
                return HttpNotFound();
            }
            db.Permissions_Visits.Remove(permissions_Visits);
            db.SaveChanges();
            return RedirectToAction("Details", "Visits", new { id = visitID });
        }

        // POST: Permissions_Visits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Permissions_Visits permissions_Visits = db.Permissions_Visits.Find(id);
            db.Permissions_Visits.Remove(permissions_Visits);
            db.SaveChanges();
            return RedirectToAction("Index");
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
