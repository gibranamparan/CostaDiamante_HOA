using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using CostaDiamante_HOA.Models;
using CostaDiamante_HOA.GeneralTools;
using static CostaDiamante_HOA.GeneralTools.FiltrosDeSolicitudes;

namespace CostaDiamante_HOA.Controllers
{
    public class PaymentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Check if the user is allowed to access
        /// </summary>
        /// <param name="userIDToCheck"></param>
        /// <returns></returns>
        public bool isAllowedToAccess(string userIDToCheck)
        {
            bool res = false;
            if (!User.IsInRole(ApplicationUser.RoleNames.ADMIN))
            {
                var userID = User.Identity.GetUserId();
                res = userIDToCheck == userID;
            }
            else
                res = true;
            return res;
        }
        // GET: Payments
        /// <summary>
        /// Returns a HTTP Json response with all the associated impact of rent payments to a visit given its ID
        /// </summary>
        /// <param name="id">Visit ID</param>
        /// <returns>HTTP Json response with numReg, errorMsg and if successful, res as the payments list</returns>
        [HttpGet]
        [ValidateHeaderAntiForgeryToken]
        public JsonResult IndexPaymentsRentImpact(int id)
        {
            int numReg = 0;
            string errorMsg = string.Empty;
            try
            {
                //Find the visit and prove if the user is allowed to access the information
                var visit = db.Visits.Find(id);
                if(!isAllowedToAccess(visit.ownerID))
                    return Json(new { numReg = numReg, errorMsg = GlobalMessages.HTTP_ERROR_FORBIDDEN });

                //Get all the payments related to the visit
                var payments = visit.payments.OrderByDescending(pay => pay.date).ToList()
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
        /// <summary>
        /// Returns a HTTP Json response with all the associated HOA Fee payments to condo given its ID
        /// in a specific quarter of a year.
        /// </summary>
        /// <param name="id">Condo ID</param>
        /// <param name="year">Year of report</param>
        /// <param name="quarter">Quarter number given as an integer number from 1 to 4 indicating the number of the trimester.</param>
        /// <returns>HTTP Json response with numReg, errorMsg and if successful, res as the payments list</returns>
        [HttpGet]
        //[ValidateHeaderAntiForgeryToken]
        public JsonResult IndexPaymentsHOAFee(int id, int year, int quarter)
        {
            int numReg = 0;
            string errorMsg = string.Empty;
            try
            {
                //Find the condo and prove if the user is allowed to access the information
                var condo = db.Condoes.Find(id);
                if (!isAllowedToAccess(condo.ownerID))
                    return Json(new { numReg = numReg, errorMsg = GlobalMessages.HTTP_ERROR_FORBIDDEN });

                //Get all the HOAFee payments related to the condo in the given period of time
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
                //Find the condo and prove if the user is allowed to access the information
                if (!isAllowedToAccess(condo.ownerID))
                    return Json(new { numReg = numReg, errorMsg = GlobalMessages.HTTP_ERROR_FORBIDDEN });

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
                //Find the condo and prove if the user is allowed to access the information
                if (!isAllowedToAccess(condo.ownerID))
                    return Json(new { numReg = numReg, errorMsg = GlobalMessages.HTTP_ERROR_FORBIDDEN });

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
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
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

                    //Load the visit reference to compose the email notification
                    db.Entry(payment).Reference(p => p.visit).Load();
                    payment.sendEmailNotification_NewRentPayment(Request,ControllerContext);

                    return Json(new { numReg = numReg, result = new { payment = new Payment.VMPayment(payment) } });
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
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
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
                    db.Entry(payment).Reference(p => p.condo).Load();
                    if (numReg > 0) {
                        payment.sendNotificationEmail(Request);
                    }
                    return Json(new { numReg = numReg, result = new { payment = new Payment.VMPayment(payment), status = payment.quarter.currentStatus } });
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
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public JsonResult DeleteConfirmed(int id)
        {
            int numReg = 0, quarterNumber = 0;
            string errorMsg = string.Empty;
            Payment payment = db.Payments.Find(id);
            Condo condo = null;
            VMHOAQuarter quarter = null;
            if (payment != null)
                try
                {
                    if (payment.typeOfPayment == Payment.TypeOfPayment.HOA_FEE) { 
                        quarter = ((Payment_HOAFee)payment).quarter;
                        condo = ((Payment_HOAFee)payment).condo;
                        quarterNumber = quarter.quarterNumber;
                    }

                    db.Payments.Remove(payment);
                    numReg = db.SaveChanges();

                    if (payment.typeOfPayment == Payment.TypeOfPayment.HOA_FEE)
                        quarter = new VMHOAQuarter(payment.date.Year, quarterNumber, condo);

                }
                catch (Exception e)
                {
                    errorMsg = String.Format("{0}. Details: {1}", e.Message, e.InnerException.Message);
                }
            else
                errorMsg = GlobalMessages.PAY_NOT_FOUND;

            if (payment.typeOfPayment == Payment.TypeOfPayment.HOA_FEE)
                return Json(new { numReg, errorMsg, status = quarter.currentStatus });
            else
                return Json(new { numReg, errorMsg});
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
