using System;
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
        public async Task<ActionResult> SendInvoices(List<Condo.VMCondo> selectedCondos, InvoiceFormGenerator ifg)
        {
            int year = DateTime.Today.Year; //TODO: Temp year
            string errorMessage = string.Empty;
            List<InvoiceSentStatus> invoicesStatus = new List<InvoiceSentStatus>();

            if (ifg == null || ifg == null)//Check if request is valid
            {
                var error = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Json(new { count = 0, errorCode = error.StatusCode, errorMessage = error.StatusDescription });
            }

            try
            {
                if (selectedCondos.Count() > 0) { // Is condos were selected
                    foreach(var vmCondo in selectedCondos) { //For each selected codo

                        var condo = db.Condoes.Find(vmCondo.condoID);
                        ifg.condoID = vmCondo.condoID;

                        /*if (!isAllowedToAccess(condo.ownerID)) // Check if current user has access to information
                        {
                            var error = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                            return Json(new { count = 0, errorCode = error.StatusCode, errorMessage = error.StatusDescription });
                        }*/

                        Invoice inv = null;

                        if (ifg.typeOfInvoice == TypeOfPayment.HOA_FEE)
                            inv = new InvoiceHOA(condo, ifg);
                        else if (ifg.typeOfInvoice == TypeOfPayment.RENTAL_IMPACT)
                            inv = new InvoiceRent(condo, ifg);

                        // Send email to condo owners and associated contacts
                        string criptedQS = GeneralTools.CryptoTools.EnryptString(ifg.QueryString);
                        inv.cryptedQueryString = criptedQS;
                        //var resSend = await inv.sendInvoice(Request, ControllerContext);

                    }
                }
                return Json(new { invoicesStatus, count = invoicesStatus.Count() });
            }catch(Exception exc)
            {
                return Json(new { errorMessage = $"{exc.Message}." + 
                    (exc.InnerException != null ? exc.InnerException.Message : string.Empty)});
            }
        }

        [HttpGet]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult DownloadInvoice(int? id, int? year, int? quarter, TypeOfPayment typeOfInvoice)
        {
            return generatePDF(id, year, quarter, typeOfInvoice);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult DownloadInvoiceByToken(string cryptedStr)
        {
            string decriptedQS = GeneralTools.CryptoTools.DecryptString(cryptedStr);
            InvoiceFormGenerator args = new InvoiceFormGenerator(decriptedQS);

            return generatePDF(args.condoID, args.year, args.quarter, args.typeOfInvoice);
        }

        private ActionResult generatePDF(int? id, int? year, int? quarter, TypeOfPayment typeOfInvoice)
        {
            var condo = db.Condoes.Find(id);
            if (condo == null)
                return HttpNotFound();

            var pdfInvoice = Invoice.generatePDF(condo, this.Request, id, year, quarter, typeOfInvoice);
            return pdfInvoice;
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
            if (id == null || ifg == null)//Check if request is valid
            {
                var error = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Json(new { count = 0, errorCode = error.StatusCode, errorMessage = error.StatusDescription });
            }

            //Find condo
            var condo = db.Condoes.Find(id);
            if (condo == null) //Check if found
            {
                var error = HttpNotFound();
                return Json(new { count = 0, errorCode = error.StatusCode, errorMessage = error.StatusDescription });
            }
            ifg.condoID = id.Value;

            if (!isAllowedToAccess(condo.ownerID)) // Check if current user has access to information
            {
                var error = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                return Json(new { count = 0, errorCode = error.StatusCode, errorMessage = error.StatusDescription });
            }

            Invoice inv = null;

            if (ifg.typeOfInvoice == TypeOfPayment.HOA_FEE)
                inv = new InvoiceHOA(condo, ifg);
            else if (ifg.typeOfInvoice == TypeOfPayment.RENTAL_IMPACT)
                inv = new InvoiceRent(condo, ifg);

            // Send email to condo owners and associated contacts
            string criptedQS = GeneralTools.CryptoTools.EnryptString(ifg.QueryString);
            inv.cryptedQueryString = criptedQS;
            var resSend = await inv.sendInvoice(Request, ControllerContext);

            return Json(resSend);
        }
    }
}