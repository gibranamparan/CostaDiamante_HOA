﻿using System;
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
    [Authorize]
    public class VisitsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Visits
        [Authorize(Roles = ApplicationUser.RoleNames.OWNER+","+ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Index()
        {
            return View(db.Visits.ToList());
        }

        // GET: Visits/Details/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visits visits = db.Visits.Find(id);
            if (visits == null)
            {
                return HttpNotFound();
            }
            return View(visits);
        }

        // GET: Visits/Create
        [Authorize(Roles = ApplicationUser.RoleNames.OWNER + "," + ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Create(int id=0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condo condo= db.Condoes.Find(id);
            if (condo == null)
            {
                return HttpNotFound();
            }

            Visits visit = prepareView(condo);

            return View(visit);
        }

        private Visits prepareView(Condo condo)
        {
            ViewBag.condo = condo;
            ViewBag.checkInList = CheckInList.getCurrentCheckInList(condo.ownerID, db);

            Visits visit = new Visits();
            visit.date = DateTime.Today;
            visit.arrivalDate = DateTime.Today.AddDays(1);
            visit.departureDate = visit.arrivalDate.AddDays(7);
            visit.condoID = condo.condoID;

            return visit;
        }

        // POST: Visits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = ApplicationUser.RoleNames.OWNER + "," + ApplicationUser.RoleNames.ADMIN)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "visitsID,arrivalDate,departureDate")] Visits visits,string id)
        {
            if (ModelState.IsValid)
            {
                visits.date = DateTime.Today;
                db.Visits.Add(visits);
                db.SaveChanges();
                return RedirectToAction("Create","Visits", new { id = id });
            }

            return View(visits);
        }

        // GET: Visits/Edit/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visits visits = db.Visits.Find(id);
            if (visits == null)
            {
                return HttpNotFound();
            }
            return View(visits);
        }

        // POST: Visits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "visitsID,date,arrivalDate,departureDate")] Visits visits)
        {
            if (ModelState.IsValid)
            {
                db.Entry(visits).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(visits);
        }

        // GET: Visits/Delete/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visits visits = db.Visits.Find(id);
            if (visits == null)
            {
                return HttpNotFound();
            }
            return View(visits);
        }

        // POST: Visits/Delete/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Visits visits = db.Visits.Find(id);
            db.Visits.Remove(visits);
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
