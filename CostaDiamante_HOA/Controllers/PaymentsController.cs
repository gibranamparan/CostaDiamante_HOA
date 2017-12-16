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
        public JsonResult IndexPaymentsRentImpact(int id)
        {
            /*var payments = db.Payment_RentImpact.Where(a => a.visitID == id).ToList().OrderByDescending(pay => pay.date)
            .Select(pay => new Payment.VMPayment(pay.paymentsID, pay.amount, pay.date, pay.typeOfPayment));*/
            int numReg = 0;
            string errorMsg = string.Empty;
            try { 
                var payments = db.Visits.Find(id).payments.OrderByDescending(pay => pay.date).ToList()
                    .Select(pay => new Payment.VMPayment(pay));
                numReg = payments != null ? payments.Count() : 0;
                return Json( new { res = payments, numReg = payments.Count() }, JsonRequestBehavior.AllowGet);
            }catch(Exception e)
            {
                errorMsg = String.Format("{0}. Details: {1}", e.Message, e.InnerException.Message);
            }
            return Json(new { numReg = numReg, errorMsg = errorMsg });
        }

        // GET: Payments
        [HttpGet]
        //[ValidateHeaderAntiForgeryToken]
        public JsonResult IndexPaymentsHOAFee(int id, int year, int quarter)
        {
            int numReg = 0;
            string errorMsg = string.Empty;
            try
            {
                var condo = db.Condoes.Find(id);
                var payments = condo.payments.Where(p => p.year == year && p.quarterNumber == quarter).OrderByDescending(pay => pay.date).ToList()
                    .Select(pay => new Payment.VMPayment(pay));
                numReg = payments != null ? payments.Count() : 0;

                return Json(new { res = payments, numReg = payments.Count() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                errorMsg = String.Format("{0}. Details: {1}", e.Message, e.InnerException.Message);
            }
            return Json(new { numReg = numReg, errorMsg = errorMsg }, JsonRequestBehavior.AllowGet);

        }


        // GET: Payments
        [HttpGet]
        //[ValidateHeaderAntiForgeryToken]
        public JsonResult GetInterestByReferenceDate(int id, int year, int quarter, DateTime refDate)
        {
            int numReg = 0;
            string errorMsg = string.Empty;
            try
            {
                var condo = db.Condoes.Find(id);
                var vmQuarter = new VMHOAQuarter(year, quarter, condo);
                decimal interest = vmQuarter.calcInterest(refDate, true);

                return Json(new { res = interest, numReg = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                errorMsg = String.Format("{0}. Details: {1}", e.Message, e.InnerException.Message);
            }
            return Json(new { numReg = numReg, errorMsg = errorMsg }, JsonRequestBehavior.AllowGet);

        }



        // GET: Payments
        [HttpGet]
        //[ValidateHeaderAntiForgeryToken]
        public JsonResult GetQuarterStatus(int id, int year)
        {
            int numReg = 0;
            string errorMsg = string.Empty;
            try
            {
                var condo = db.Condoes.Find(id);
                var res = new { condoID = condo.condoID, condoName = condo.name, status = condo.getHOAStatusByYear(year) };
                return Json(new { res = res, numReg = res.status.Count() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                errorMsg = String.Format("{0}. Details: {1}", e.Message, e.InnerException.Message);
            }

            return Json(new { numReg = numReg, errorMsg = errorMsg }, JsonRequestBehavior.AllowGet);

        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public JsonResult CreatePaymentsRentImpact(Payment_RentImpact payment)
        {
            int numReg = 0;
            string errorMsg = string.Empty;

            try
            {
                if (ModelState.IsValid)
                {
                    db.Payment_RentImpact.Add(payment);
                    numReg = db.SaveChanges();
                    db.Entry(payment).Reference(p => p.visit).Load();
                    payment.sendEmailNotification_NewRentPayment(Request,ControllerContext);
                    return Json(new { numReg = numReg, result = new Payment.VMPayment(payment) });
                }
            }
            catch (Exception e)
            {
                errorMsg = String.Format("{0}. Details: {1}", e.Message, e.InnerException.Message);
            }

            return Json(new { numReg = numReg, errorMsg = errorMsg });
        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public JsonResult CreatePaymentsHOAFee(Payment_HOAFee payment)
        {
            int numReg = 0;
            string errorMsg = string.Empty;

            try
            {
                if (ModelState.IsValid)
                {
                    db.Payment_HOAFee.Add(payment);
                    numReg = db.SaveChanges();
                    if (numReg > 0) {
                        db.Entry(payment).Reference(p => p.condo).Load();
                        payment.sendNotificationEmail(Request);
                    }
                    return Json(new { numReg = numReg, result = new Payment.VMPayment(payment) });
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
            Payment payments = db.Payments.Find(id);
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
