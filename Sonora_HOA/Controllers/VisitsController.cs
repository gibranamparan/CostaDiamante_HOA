using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sonora_HOA.GeneralTools;
using Sonora_HOA.Models;
using static Sonora_HOA.GeneralTools.FiltrosDeSolicitudes;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace Sonora_HOA.Controllers
{
    [Authorize]
    public class VisitsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Visits
        public ActionResult Index()
        {
            string ownerID = User.Identity.GetUserId();
            Owner owner = db.Owners.Find(ownerID);
            return View(owner.visitsHistory.ToList());
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
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult Create([Bind(Include = "condoID,arrivalDate,departureDate,date,guestsInVisit, visitors")] Visits visit)
        {
            Condo condo = db.Condoes.Find(visit.condoID);
            if (ModelState.IsValid)
            {
                if (visit.arrivalDate > visit.departureDate)
                {
                    ModelState.AddModelError("INVALID_DATES", "Arrival date is bigger than departure date. Check introduced dates.");
                }else { 
                    visit.date = DateTime.Today;
                    db.Visits.Add(visit);
                    db.SaveChanges();
                    return RedirectToAction("Index","Visits", new { id = condo.ownerID });
                }
            }

            visit = prepareView(condo);

            return View(visit);
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
