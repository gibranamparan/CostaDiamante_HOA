﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CostaDiamante_HOA.Models;
using CostaDiamante_HOA.GeneralTools;
using static CostaDiamante_HOA.GeneralTools.FiltrosDeSolicitudes;
using Microsoft.AspNet.Identity;

namespace CostaDiamante_HOA.Controllers
{
    [Authorize]
    public class VisitsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Visits
        public ActionResult Index(Visits.VMVisitsFilter visitsFilter)
        {
            TimePeriod periodReported = visitsFilter.TimePeriod;
            bool isInHouse = visitsFilter.isInHouse;

            List<Visits> visits = new List<Visits>();
            //If user is admin, shows every visit in database
            if (User.IsInRole(ApplicationUser.RoleNames.ADMIN))
                visits = db.Visits.ToList();
            //If user is an owner, shows just his visits
            else if (User.IsInRole(ApplicationUser.RoleNames.OWNER))
            {
                string ownerID = User.Identity.GetUserId();
                Owner owner = db.Owners.Find(ownerID);
                visits = owner.visitsHistory.ToList();
            }
            if (periodReported.Equals(new TimePeriod()))
                periodReported = new TimePeriod(DateTime.Today, DateTime.Today.AddDays(7));

            //Se filtran visitas
            if (!isInHouse)
            {
                visits = visits.Where(vis => periodReported
                    .hasInside(vis.timePeriod.startDate))
                    .OrderBy(vis => vis.timePeriod.startDate).ToList();
            }
            else
            {
                visits = visits.Where(vis => vis.isInHouseInPeriod(periodReported))
                    .OrderBy(vis => vis.timePeriod.startDate).ToList();
            }

            ViewBag.result = visits;
            return View(visitsFilter);
        }

        // GET: Visits/Details/5
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
        public ActionResult Create()
        {
            ViewBag.condoID = new SelectList(db.Condoes, "condoID", "name");
            ViewBag.ownerID = new SelectList(db.Users, "Id", "name");
            return View();
        }

        // POST: Visits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Visits visits, List<Visitor> visitors)
        {
            if (ModelState.IsValid)
            {
                db.Visits.Add(visits);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.condoID = new SelectList(db.Condoes, "condoID", "name", visits.condoID);
            ViewBag.ownerID = new SelectList(db.Users, "Id", "name", visits.ownerID);
            return View(visits);
        }

        // GET: Visits/Edit/5
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
            ViewBag.condoID = new SelectList(db.Condoes, "condoID", "name", visits.condoID);
            ViewBag.ownerID = new SelectList(db.Users, "Id", "name", visits.ownerID);
            return View(visits);
        }

        // POST: Visits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "visitsID,date,arrivalDate,departureDate,condoID,ownerID")] Visits visits)
        {
            if (ModelState.IsValid)
            {
                db.Entry(visits).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.condoID = new SelectList(db.Condoes, "condoID", "name", visits.condoID);
            ViewBag.ownerID = new SelectList(db.Users, "Id", "name", visits.ownerID);
            return View(visits);
        }

        // GET: Visits/Delete/5
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
