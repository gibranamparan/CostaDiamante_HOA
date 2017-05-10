using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sonora_HOA.Models;
using Microsoft.AspNet.Identity;

namespace Sonora_HOA.Controllers
{
    [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
    public class OwnersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Owners
        public ActionResult Index()
        {
            return View(db.Owners.ToList());
        }

        // GET: Owners/Details/5
        [Authorize]
        public ActionResult Details(string id)
        {
            Owner owners = null;

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //If admin, get the requested owner
            else if (User.IsInRole(ApplicationUser.RoleNames.ADMIN))
                owners = db.Owners.Find(id);
            //If owner, show only his details
            else if (User.IsInRole(ApplicationUser.RoleNames.OWNER))
                owners = db.Owners.Find(User.Identity.GetUserId()); 
            
            if (owners == null)
                return HttpNotFound();

            ViewBag.notAssociatedCondos = db.Condoes.Where(con => 
                String.IsNullOrEmpty(con.ownerID)).ToList();

            return View(owners);
        }
        
        // GET: Owners/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Owner owner = db.Owners.Find(id);
            if (owner == null)
            {
                return HttpNotFound();
            }
            RegisterViewModel vmOwner = new RegisterViewModel(owner);
            return View(vmOwner);
        }

        // POST: Owners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,name,lastName")] Owner owners)
        {
            if (ModelState.IsValid)
            {
                db.Entry(owners).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(owners);
        }

        // GET: Owners/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Owner owners = db.Owners.Find(id);
            if (owners == null)
            {
                return HttpNotFound();
            }
            return View(owners);
        }

        // POST: Owners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Owner owners = db.Owners.Find(id);
            var condos = owners.Condos.ToList();
            var cils = owners.checkInListHistory.ToList();
            var visits = owners.visitsHistory.ToList();
            foreach (var condo in condos)
            {
                condo.ownerID = null;
                db.Entry(condo).State = EntityState.Modified;
            }
            foreach (var cil in cils)
                db.CheckInLists.Remove(cil);
            foreach (var visit in visits)
                db.Visits.Remove(visit);

            db.Owners.Remove(owners);
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
