﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CostaDiamante_HOA.Models;
using System.Threading.Tasks;
using static CostaDiamante_HOA.Models.Payment;
using CostaDiamante_HOA.GeneralTools;
using System.Net;
using Microsoft.AspNet.Identity;

namespace CostaDiamante_HOA.Controllers
{
    [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
    public class InvoiceController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Invoice
        public ActionResult Index()
        {
            return View("Invoice_Index");
        }

        /// <summary>
        /// Check if the user is allowed to access
        /// </summary>
        /// <param name="userIDToCheck"></param>
        /// <returns></returns>
        private bool isAllowedToAccess(string userIDToCheck)
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

        /// <summary>
        /// Envia emails a todos los owners y contactos de todos los condominios 
        /// </summary>
        /// <param name="selectedCondos"></param>
        /// <param name="invoiceGeneratorForm"></param>
        /// <returns></returns>
        [HttpPost]
        [GeneralTools.FiltrosDeSolicitudes.ValidateHeaderAntiForgeryToken]
        public async Task<ActionResult> SendInvoices(List<Condo.VMCondo> selectedCondos, InvoiceFormGenerator invoiceGeneratorForm)
        {
            int year = DateTime.Today.Year; //TODO: Temp year
            string errorMessage = string.Empty;
            List<InvoiceSentStatus> invoicesStatus = new List<InvoiceSentStatus>();

            try
            {
                if (selectedCondos.Count() > 0) { 
                    foreach(var vmCondo in selectedCondos) {
                        var condo = db.Condoes.Find(vmCondo.condoID);
                        InvoiceSentStatus res = await condo.sendEmail_ImpactOfRentReport(Request, ControllerContext, year);
                        invoicesStatus.Add(res);
                    }
                }
                return Json(new { invoicesStatus, count = invoicesStatus.Count() });
            }catch(Exception exc)
            {
                return Json(new { errorMessage = $"{exc.Message}." + 
                    (exc.InnerException != null ? exc.InnerException.Message : string.Empty)});
            }
        }

        public ActionResult DownloadInvoice(int? id, int? year, int? quarter, TypeOfPayment typeOfInvoice)
        {
            var condo = db.Condoes.Find(id);
            if (condo == null)
                return HttpNotFound();

            InvoiceFormGenerator ifg = new InvoiceFormGenerator() { quarter = quarter.Value, typeOfInvoice = typeOfInvoice, year = year.Value };

            Invoice inv = null;

            if (typeOfInvoice == TypeOfPayment.HOA_FEE)
                inv = new InvoiceHOA(condo, ifg);
            else if (typeOfInvoice == TypeOfPayment.RENTAL_IMPACT)
                inv = new InvoiceRent(condo, ifg);

            var fileView = inv.generateInvoicePDF(this.Request);

            //Code to get content
            return fileView;
        }

        /// <summary>
        /// Genera una vista de un Invoice de cobro de HOAFee
        /// </summary>
        /// <param name="condoID"></param>
        /// <param name="year"></param>
        /// <param name="quarter"></param>
        /// <param name="sendDate"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Invoice_HOAFee(int id, int year, int quarter, DateTime? sendDate)
        {
            var condo = db.Condoes.Find(id);
            var quarterStatus = new VMHOAQuarter(year, quarter, condo);
            InvoiceFormGenerator ifg = new InvoiceFormGenerator() { quarter = quarter, year = year,
                sendDate = (sendDate == null ? DateTime.Today : sendDate.Value), typeOfInvoice = TypeOfPayment.HOA_FEE };

            var invoice = new InvoiceHOA(condo, ifg) { amount = quarterStatus.RemainToPay };
            return View(invoice);
        }


        [HttpPost]
        [FiltrosDeSolicitudes.ValidateHeaderAntiForgeryToken]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public async Task<JsonResult> SendEmail(int? id, InvoiceFormGenerator ifg)
        {
            if (id == null || ifg == null)//Check request
            {
                var error = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Json(new { count = 0, errorCode = error.StatusCode, errorMessage = error.StatusDescription });
            }

            var condo = db.Condoes.Find(id);
            if (condo == null) //Check if existe
            {
                var error = HttpNotFound();
                return Json(new { count = 0, errorCode = error.StatusCode, errorMessage = error.StatusDescription });
            }

            if (!isAllowedToAccess(condo.ownerID))
            {
                var error = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                return Json(new { count = 0, errorCode = error.StatusCode, errorMessage = error.StatusDescription });
            }

            Invoice inv = null;

            if (ifg.typeOfInvoice == TypeOfPayment.HOA_FEE)
                inv = new InvoiceHOA(condo, ifg);
            else if (ifg.typeOfInvoice == TypeOfPayment.RENTAL_IMPACT)
                inv = new InvoiceRent(condo, ifg);

            Payment.InvoiceSentStatus errorMessage = await inv.sendInvoice(Request, ControllerContext);

            return Json(errorMessage.mailStatus.message);
        }


    }
}