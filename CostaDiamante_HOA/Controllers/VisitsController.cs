using System;
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
using System.Web.Script.Serialization;

namespace CostaDiamante_HOA.Controllers
{
    [Authorize]
    public class VisitsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: Visits
        public ActionResult Index(Visit.VMVisitsFilter visitsFilter)
        {
            TimePeriod periodReported = visitsFilter.TimePeriod;
            bool isInHouse = visitsFilter.isInHouse;

            List<Visit> visits = new List<Visit>();
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
            Visit visits = db.Visits.Find(id);
            if (visits == null)
            {
                return HttpNotFound();
            }
            return View(visits);
        }

        // GET: Visits/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Condo condo = db.Condoes.Find(id);
            if (condo == null)
                return HttpNotFound();

            Visit visit = new Visit();
            visit.condoID = id.Value;
            visit.ownerID = condo.owner.Id;
            visit.owner = condo.owner;
            return View(visit);
        }

        // POST: Visits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Create(Visit visit)
        {
            int numReg = 0;
            string errorMsg = string.Empty;
            string errorMailer = string.Empty;
            visit.date = DateTime.Now;
            try { 
                if (ModelState.IsValid)
                {
                    db.Visits.Add(visit);
                    numReg = db.SaveChanges();
                    if (numReg > 0) //If the visit notification was added, an email is sent to admin
                    {
                        db.Entry(visit).Reference(v => v.owner).Load();
                        db.Entry(visit).Reference(v => v.condo).Load();
                        //errorMailer = visit.sendNotificationEmail(Request);
                    }
                }
            }catch(Exception e)
            {   //An error ocurrend, prepare to be reported
                if(e == null)
                    errorMsg = "Error Exception was null, error unknown";
                else
                    errorMsg = String.Format("{0}. Details: {1}",e.Message,e.InnerException.Message);
            }

            return Json(new { numReg = numReg, errorMsg = errorMsg, errorMailer = errorMailer });
        }

        // GET: Visits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visit visits = db.Visits.Find(id);
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
        //[ValidateAntiForgeryToken]
        public JsonResult Edit(Visit visit)
        {
            int numReg = 0;
            string errorMsg = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(visit).State = EntityState.Modified;
                    numReg = db.SaveChanges();
                    return Json(new { numReg = numReg });
                }
            }
            catch (Exception e)
            {
                errorMsg = String.Format("{0}. Details: {1}", e.Message, e.InnerException.Message);
            }

            return Json(new { numReg = numReg, errorMsg = errorMsg });
        }

        // GET: Visits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visit visits = db.Visits.Find(id);
            if (visits == null)
            {
                return HttpNotFound();
            }
            return View(visits);
        }

        // POST: Visits/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            int numReg = 0;
            string errorMsg = string.Empty;

            try
            {
                Visit visits = db.Visits.Find(id);
                db.Visits.Remove(visits);
                numReg = db.SaveChanges();
            }
            catch (Exception e)
            {
                errorMsg = String.Format("{0}. Details: {1}", e.Message, e.InnerException.Message);
            }

            return Json(new { numReg = numReg, errorMsg = errorMsg });
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
