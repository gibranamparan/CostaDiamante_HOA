using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CostaDiamante_HOA.Models;
using System.Threading.Tasks;
using static CostaDiamante_HOA.Models.Payment;

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
    }
}