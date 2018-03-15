using CostaDiamante_HOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using CostaDiamante_HOA.GeneralTools;

namespace CostaDiamante_HOA.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

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
        // GET: Reports
        /// <summary>
        /// Prepares a global report to show the payment of HOAFee status for every quarter of a year for every condo.
        /// </summary>
        /// <param name="year">Year of the report</param>
        /// <returns>A view to show the report in the template Reports/HOAFeesByYear</returns>
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult HOAFeesByYear(int? year)
        {
            year = (year==null || year == 0) ? DateTime.Now.Year : year;
            var owners = db.Owners.ToList();

            List<VMOwnerHOAQuartersRow> reporte = new List<VMOwnerHOAQuartersRow>();
            owners.Where(ow=>ow.Condos.Count()>0).ToList().ForEach(ow => reporte.AddRange(ow.ReportHOAFeeByYear(year.Value)));

            ViewBag.year = year;

            return View(reporte);
        }

        /// <summary>
        /// Prepares a report of rents and costs caused by impact of rent, given an condoID, year and 
        /// boolean value to indicate PDF Mode.
        /// </summary>
        /// <param name="id">Condo ID to which corresponds the report.</param>
        /// <param name="year">Year which is the period of time</param>
        /// <param name="pdfMode">A boolean flag to indicate if the view is going to be adapted to be rendered in a PDF</param>
        /// <returns>A view to show the report in the template Reports/RentsByYear</returns>
        public ActionResult RentsByYear(int? id, int? year, bool pdfMode = false)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var condo = db.Condoes.Find(id);
            if (condo == null)
                return HttpNotFound();

            if(!isAllowedToAccess(condo.ownerID))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            year = (year == null || year == 0) ? DateTime.Today.Year : year;
            var visits = condo.visitsHistory.Where(v => v.typeOfVisit == typeOfVisit.BY_RENT && v.arrivalDate.Year == year)
                .Select(v => new VMVisitreport(v));

            ViewBag.year = year;
            ViewBag.condo = condo;
            if (pdfMode) { 
                ViewBag.pdfMode = pdfMode;
            }

            return View(visits);
        }

        /// <summary>
        /// Generates a PDF to download of the RentsByYear report.
        /// </summary>
        /// <param name="id">Condo ID</param>
        /// <param name="year">Year to report</param>
        /// <returns>A rotativa PDF generated from RentsByYear action in PDF Mode which is converted to a file to be downloaded.</returns>
        public ActionResult DownloadPDfRentsByYear(int? id, int? year)
        {
            var condo = db.Condoes.Find(id);
            if (condo == null)
                return HttpNotFound();

            if (!isAllowedToAccess(condo.ownerID))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            var fileView = condo.generateRotativaPDF_RentsByYearReport(year, Request);

            //Code to get content
            return fileView;
        }

        [HttpPost]
        [FiltrosDeSolicitudes.ValidateHeaderAntiForgeryToken]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public async Task<JsonResult> SendEmailImpactRentsReport(int condoID, int year = 0)
        {
            var condo = db.Condoes.Find(condoID);
            if (condo == null)
            {
                var error = HttpNotFound();
                return Json(new { count = 0,errorCode = error.StatusCode, errorMessage = error.StatusDescription});
            }

            if (!isAllowedToAccess(condo.ownerID))
            {
                var error = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                return Json(new { count = 0, errorCode = error.StatusCode, errorMessage = error.StatusDescription });
            }

            string errorMessage = await condo.sendEmail_ImpactOfRentReport(Request, ControllerContext, year);

            return Json(errorMessage);
        }
    }
}