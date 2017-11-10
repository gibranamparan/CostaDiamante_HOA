using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CostaDiamante_HOA.Models;
using System.Web.Routing;

namespace CostaDiamante_HOA.Controllers
{
    public class PaymentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Payments
        [HttpPost]
        public JsonResult Index(int id)
        {
            //var payments = db.Payments.Include(p => p.owner).Include(p => p.visit);
            var payments = db.Payments.Where(a => a.visitID == id)
            .Select(a => new
            {
                OwnerName = a.owner.name,
                Amount = a.amount,
                Date = a.date.ToString(),
                TypeOfPayment = a.typeOfPayment
            });
           
            return Json(payments);
            //return Json(payments, JsonRequestBehavior.AllowGet);
        }

        // GET: Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payments payments = db.Payments.Find(id);
            if (payments == null)
            {
                return HttpNotFound();
            }
            return View(payments);
        }

        // GET: Payments/Create
        public ActionResult Create(int visitID, string ownerID)
        {
            //ViewBag.ownerID = new SelectList(db.Users, "Id", "name");
            //ViewBag.visitID = new SelectList(db.Visits, "visitID", "ownerID");
            ViewBag.ownerID = ownerID;
            ViewBag.visitID = visitID;
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Create(Payments payment)
        {
            int numReg = 0;
            string errorMsg = string.Empty;

            try
            {
                if (ModelState.IsValid)
                {
                    db.Payments.Add(payment);
                    numReg = db.SaveChanges();
                    return Json(new { numReg = numReg });

                    //Visit visits = db.Visits.Find(payment.visitID);
                    //return RedirectToAction("Details", "Visits", payments.visitID);
                    //return RedirectToAction("Details", new RouteValueDictionary(new { controller = "Visits", action = "Details", Id = payments.visitID }));
                    //return Json(visits);
                    //return Json(new { });
                }
            }
            catch (Exception e)
            {
                errorMsg = String.Format("{0}. Details: {1}", e.Message, e.InnerException.Message);
            }

            //ViewBag.ownerID = new SelectList(db.Users, "Id", "name", payment.ownerID);
            //ViewBag.visitID = new SelectList(db.Visits, "visitID", "ownerID", payment.visitID);
            //return View(payments);
            //return Json(new { });
            return Json(new { numReg = numReg, errorMsg = errorMsg });
        }

        // GET: Payments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payments payments = db.Payments.Find(id);
            if (payments == null)
            {
                return HttpNotFound();
            }
            ViewBag.ownerID = new SelectList(db.Users, "Id", "name", payments.ownerID);
            ViewBag.visitID = new SelectList(db.Visits, "visitID", "ownerID", payments.visitID);
            return View(payments);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "paymentsID,amount,date,typeOfPayment,ownerID,visitID")] Payments payments)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payments).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ownerID = new SelectList(db.Users, "Id", "name", payments.ownerID);
            ViewBag.visitID = new SelectList(db.Visits, "visitID", "ownerID", payments.visitID);
            return View(payments);
        }

        // GET: Payments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payments payments = db.Payments.Find(id);
            if (payments == null)
            {
                return HttpNotFound();
            }
            return View(payments);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Payments payments = db.Payments.Find(id);
            db.Payments.Remove(payments);
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
