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
            if (!isInHouse) { 
                visits = visits.Where(vis => periodReported
                    .hasInside(vis.timePeriod.startDate))
                    .OrderBy(vis=>vis.timePeriod.startDate).ToList();
            }else
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
        public ActionResult Create(int id=0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condo condo = null;
            if (User.IsInRole(ApplicationUser.RoleNames.ADMIN)) { 
                condo = db.Condoes.Find(id);
            }else if (User.IsInRole(ApplicationUser.RoleNames.OWNER)){
                var user = db.Owners.Find(User.Identity.GetUserId());
                condo = user.Condos.FirstOrDefault(con => con.condoID == id);
            }
            if (condo == null || condo.condoID == 0)
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
            visit.ownerID = condo.ownerID;

            //Time periods list to populate dropdown selection
            List<CheckInList.TimePeriodPermissions> timePeriods = CheckInList.generatePermissionPeriods();
            ViewBag.timePeriods = timePeriods;

            CheckInList currentCheckInList = CheckInList.getCurrentCheckInList(condo.ownerID, db);
            ViewBag.currentCheckInList = currentCheckInList;

            CheckInList nextCheckInList = CheckInList.findCheckInListByPeriod(condo.ownerID, timePeriods.ElementAt(1), db);
            if (nextCheckInList.checkInListID == 0)
                nextCheckInList.setPeriod(new CheckInList.TimePeriodPermissions(timePeriods.ElementAt(1)));
            ViewBag.nextCheckInList = nextCheckInList;

            return visit;
        }

        // POST: Visits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public JsonResult Create([Bind(Include = "condoID,arrivalDate,departureDate,date,guestsInVisit,"+
            "visitors,ownerID,checkInListID,wildcards")]
            Visits visit, int checkInListID)
        {
            if (ModelState.IsValid)
            {
                //Invalid range
                if (visit.arrivalDate > visit.departureDate) 
                    return Json(new { savedRegs = 0, error = "Introduced time range is not valid. " });
                else {
                    //Checking if visit time is inside current checkinlist
                    CheckInList cil = db.CheckInLists.Find(checkInListID);
                    if (cil.period.hasInside(visit.timePeriod))
                    {
                        visit.date = DateTime.Today;
                        //In case checkin list and permissions are removed, the visitors in the visit list
                        //are keeping the full name to be printed
                        foreach(var pv in visit.visitors) {
                            var permission = db.Permissions.Find(pv.permissionsID);
                            pv.guestFullName = db.Permissions.Find(permission.permissionsID).fullName;
                        }

                        db.Visits.Add(visit);
                        int savedRegs = db.SaveChanges();
                        return Json(new { savedRegs = savedRegs, error = "" });
                    }
                    else
                        return Json(new { savedRegs = 0, error = "Selected dates are out of current Check In List year period. " });
                }
            }

            Condo condo = db.Condoes.Find(visit.condoID);
            visit = prepareView(condo);

            return Json(new {
                savedRegs = 0, error = "Some of the data introduced is wrong, check and try again."+
                "If problem persists, contact administrator." });
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
        [ValidateHeaderAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            Visits visits = db.Visits.Find(id);
            db.Visits.Remove(visits);
            int savedRegs = db.SaveChanges();
            return Json(new { savedRegs = savedRegs });
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
