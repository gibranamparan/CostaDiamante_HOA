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
    public class PermissionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Permissions
        [Authorize]
        public ActionResult Index(string id, bool openCheckInList = false)
        {
            if (String.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var owner = db.Owners.Find(id);
            if (owner == null)
            {
                return HttpNotFound();
            }

            ViewBag.owner = owner;
            //Time periods list to populate dropdown selection
            List<CheckInList.TimePeriodPermissions> timePeriods = CheckInList.generatePermissionPeriods();
            ViewBag.timePeriods = timePeriods;

            CheckInList currentCheckInList = CheckInList.getCurrentCheckInList(owner.Id, db);
            ViewBag.currentCheckInList = currentCheckInList;

            CheckInList nextCheckInList = CheckInList.findCheckInListByPeriod(owner.Id, timePeriods.ElementAt(1), db);
            if (nextCheckInList.checkInListID == 0)
                nextCheckInList.setPeriod(new CheckInList.TimePeriodPermissions(timePeriods.ElementAt(1)));

            ViewBag.nextCheckInList = nextCheckInList;

            //Indicates if new checkinlist form must be opened by default
            ViewBag.openCheckInList = openCheckInList;
            return View();
        }

        // GET: Permissions/Details/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
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
        [Authorize]
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public JsonResult Create(List<Permissions> checkedList,CheckInList period,string ownerID)
        {
            int numRegChanged = 0;
            CheckInList checkInList = period;
            //Associating condos in the array to owner
            checkInList.permissions = checkedList;
            checkInList.ownerID = ownerID;
            if(checkedList.Count()>0 && !String.IsNullOrEmpty(ownerID) && period!=null)
            { 
                db.CheckInLists.Add(checkInList);
                numRegChanged = db.SaveChanges();
            }

            return Json(numRegChanged);
        }

        /*
        // GET: Permissions/Edit/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
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

            ViewBag.guestID = new SelectList(db.Guests.ToList(), "guestID", "fullName", permissions.guestID);
            return View(permissions);
        }
        // POST: Permissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "permissionsID,startDate,condoID,guestID")] Permissions permissions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(permissions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.guestID = new SelectList(db.Guests.ToList(), "guestID", "fullName", permissions.guestID);
            return View(permissions);
        }

        // GET: Permissions/Delete/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Delete(int? id, string ownerID)
        {
            var Permissions = db.Permissions.Where(p => p.guest.ownerID == ownerID).ToList();

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
        }*/

        // POST: Permissions/Delete/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string ownerID)
        {
            Permissions permissions = db.Permissions.Find(id);
            db.Permissions.Remove(permissions);
            db.SaveChanges();
            return RedirectToAction("Create","Visits", new { id = ownerID });
        }


        /*
        [Authorize]
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public JsonResult Delete(int id = 0)
        {
            Guest guest = db.Guests.Find(id);
            string ownerID = guest.ownerID;
            CheckInList currentCil = CheckInList.getCurrentCheckInList(ownerID, db);

            if (guest == null) 
                return Json(new { regSaved = 0 });
            else
            {
                //Find All the permission related to this guest
                List<Permissions_Visits> guestPermissions = db.Permissions_Visits
                    .Where(per => per.permissions.guestID == id).ToList();
                foreach(var pv in guestPermissions)
                {
                    //To remove permissions, the link to the visitors lists have to be removed
                    pv.permissionsID = null;
                    db.Entry(pv).State = EntityState.Modified;
                }

                db.Guests.Remove(guest);
                int regSaved = db.SaveChanges();

                if (currentCil.permissions.Count() == 0)
                {
                    db.CheckInLists.Remove(currentCil);
                    db.SaveChanges();
                }

                return Json(new { regSaved = regSaved });
            }
        }*/

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
