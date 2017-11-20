using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CostaDiamante_HOA.Models;

namespace CostaDiamante_HOA.Controllers
{
    public class VisitorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public JsonResult Index(int id)
        {
            //var payments = db.Payments.Include(p => p.owner).Include(p => p.visit);
            var visitors = db.Visitor.Where(a => a.visitID == id)
            .Select(a => new
            {
                visitorID = a.visitorID,
                name = a.name,
                lastName = a.lastName,
                isYounger = a.isYounger,
                visitID = a.visitID
            });

            return Json(visitors);
            //return Json(payments, JsonRequestBehavior.AllowGet);
        }

        // GET: Visitors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visitor visitor = db.Visitor.Find(id);
            if (visitor == null)
            {
                return HttpNotFound();
            }
            return View(visitor);
        }

        // GET: Visitors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Visitors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Create(Visitor visitor)
        {
            int numReg = 0;
            string errorMsg = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Visitor.Add(visitor);
                    db.SaveChanges();
                    numReg = visitor.visitorID;
                    return Json(new { numReg = numReg });
                }
            }
            catch (Exception e)
            {
                errorMsg = String.Format("{0}. Details: {1}", e.Message, e.InnerException.Message);
            }
            return Json(new { numReg = numReg, errorMsg = errorMsg });
        }

        // GET: Visitors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visitor visitor = db.Visitor.Find(id);
            if (visitor == null)
            {
                return HttpNotFound();
            }
            return View(visitor);
        }

        // POST: Visitors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "visitorID,name,lastName,isYounger,visitID")] Visitor visitor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(visitor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(visitor);
        }

        // GET: Visitors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visitor visitor = db.Visitor.Find(id);
            if (visitor == null)
            {
                return HttpNotFound();
            }
            return View(visitor);
        }

        // POST: Visitors/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            int numReg = 0;
            string errorMsg = string.Empty;
            try
            {
                Visitor visitor = db.Visitor.Find(id);
                db.Visitor.Remove(visitor);
                numReg = db.SaveChanges();
                return Json(new { numReg = numReg });
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
