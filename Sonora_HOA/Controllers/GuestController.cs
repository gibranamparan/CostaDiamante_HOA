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
    public class GuestController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Guest
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Index(string id)
        {
            ViewBag.Ownerid = id;
            return View(db.Guests.ToList());
        }

        // GET: Guest/Details/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Guest guest = db.Guests.Find(id);
            if (guest == null)
            {
                return HttpNotFound();
            }
            return View(guest);
        }

        // GET: Guest/Create
        [Authorize(Roles = ApplicationUser.RoleNames.OWNER+","+ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Create()
        {
            
            return View();
        }

        // POST: Guest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = ApplicationUser.RoleNames.OWNER+","+ApplicationUser.RoleNames.ADMIN)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ownerID,name,lastName")] Guest guest)
        {
            if (ModelState.IsValid)
            {
                db.Guests.Add(guest);
                db.SaveChanges();
                return RedirectToAction("Details","Owners",new { Id = guest.ownerID});
            }

            return View();
        }

        // GET: Guest/Edit/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN + "," + ApplicationUser.RoleNames.OWNER)]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Guest guest = db.Guests.Find(id);
            if (guest == null)
            {
                return HttpNotFound();
            }
            return View(guest);
        }

        // POST: Guest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN + "," + ApplicationUser.RoleNames.OWNER)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,guestID,name,lastName")] Guest guest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(guest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(guest);
        }

        // GET: Guest/Delete/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN + "," + ApplicationUser.RoleNames.OWNER)]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Guest guest = db.Guests.Find(id);
            if (guest == null)
            {
                return HttpNotFound();
            }
            return View(guest);
        }

        // POST: Guest/Delete/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN + "," + ApplicationUser.RoleNames.OWNER)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Guest guest = db.Guests.Find(id);
            db.Guests.Remove(guest);
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
