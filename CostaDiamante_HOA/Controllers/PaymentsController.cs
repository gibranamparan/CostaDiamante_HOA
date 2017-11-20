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
using CostaDiamante_HOA.GeneralTools;
using static CostaDiamante_HOA.GeneralTools.FiltrosDeSolicitudes;

namespace CostaDiamante_HOA.Controllers
{
    public class PaymentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Payments
        [HttpGet]
        //[ValidateHeaderAntiForgeryToken]
        public JsonResult Index(int id)
        {
            var payments = db.Payments.Where(a => a.visitID == id).ToList()
            .Select(pay=> new Payments.VMPayment(pay.paymentsID, pay.amount, pay.date, pay.typeOfPayment));
           
            return Json(payments, JsonRequestBehavior.AllowGet);
        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
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
                    return Json(new { numReg = numReg, result = new Payments.VMPayment(payment) });
                }
            }
            catch (Exception e)
            {
                errorMsg = String.Format("{0}. Details: {1}", e.Message, e.InnerException.Message);
            }

            return Json(new { numReg = numReg, errorMsg = errorMsg });
        }
        
        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateHeaderAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            int numReg = 0;
            string errorMsg = string.Empty;
            Payments payments = db.Payments.Find(id);
            if (payments != null)
                try
                {
                    db.Payments.Remove(payments);
                    numReg = db.SaveChanges();
                }
                catch (Exception e)
                {
                    errorMsg = String.Format("{0}. Details: {1}", e.Message, e.InnerException.Message);
                }
            else
                errorMsg = GlobalMessages.PAY_NOT_FOUND;


            return Json(new { numReg, errorMsg });
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
