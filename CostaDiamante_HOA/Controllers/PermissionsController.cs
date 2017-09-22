using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CostaDiamante_HOA.Models;
using static CostaDiamante_HOA.GeneralTools.FiltrosDeSolicitudes;

namespace CostaDiamante_HOA.Controllers
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
        /// <summary>
        /// Check in list creation.
        /// </summary>
        /// <param name="checkedList">Created guests for this list</param>
        /// <param name="period">A new empty list with a period of the year already selected</param>
        /// <param name="ownerID">Owner of the condo where the guests will go</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public JsonResult Create(List<Permissions> checkedList,CheckInList period,string ownerID)
        {
            int numRegChanged = 0;

            if (period != null) {
                //If its a new list
                if (period.checkInListID == 0) { 
                    //If model state is valid, data is saved
                    if(checkedList!=null && checkedList.Count()>0 && !String.IsNullOrEmpty(ownerID))
                    {
                        //The new checkin list is filled
                        CheckInList checkInList = period;
                        checkInList.permissions = checkedList;
                        checkInList.ownerID = ownerID;

                        //List saved
                        db.CheckInLists.Add(checkInList);
                        numRegChanged = db.SaveChanges();
                    }
                }
                else //If the list already exists
                {
                    //Check for wildcards
                    checkedList.Take(4).ToList().ForEach(per => per.checkInListID = period.checkInListID);
                    db.Permissions.AddRange(checkedList);
                    numRegChanged = db.SaveChanges();
                }
            }

            //Generating message error if nothing was not saved
            string errorMsg = string.Empty;
            if (numRegChanged == 0)
                errorMsg = generateErrorMessage(checkedList, ownerID, period);

            return Json(new { numReg = numRegChanged, error = errorMsg });
        }

        /// <summary>
        /// Generate error message based on the validation arguments
        /// </summary>
        /// <param name="checkedList"></param>
        /// <param name="ownerID"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        private string generateErrorMessage(List<Permissions> checkedList, string ownerID, CheckInList period)
        {
            string error = "";
            error += (checkedList == null|| checkedList.Count() > 0) ? "No guest was registered. " : "";
            error += String.IsNullOrEmpty(ownerID) ? "Owner is unknown. " : "";
            error += period == null ? "Period was not determined. " : "";

            return error;
        }
        
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
