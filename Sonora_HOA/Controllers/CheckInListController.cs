using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sonora_HOA.Models;
using System.Net;

namespace Sonora_HOA.Controllers
{
    [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
    public class CheckInListController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CheckInList
        public ActionResult Delete(int id=0)
        {
            if (id==0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var checkInList= db.CheckInLists.Find(id);
            string ownerID = checkInList.ownerID;
            if (checkInList == null)
            {
                return HttpNotFound();
            }

            var visits = db.Visits.Where(vis => vis.visitors.First()
                .permissions.checkInListID == checkInList.checkInListID);
            db.Visits.RemoveRange(visits);
            db.CheckInLists.Remove(checkInList);
            db.SaveChanges();

            return RedirectToAction("Index", "Permissions", new { id = ownerID });
        }
    }
}