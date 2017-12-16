using CostaDiamante_HOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CostaDiamante_HOA.Controllers
{
    public class ReportsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Reports
        public ActionResult HOAFeesByYear(int? year)
        {
            year = (year==null || year == 0) ? DateTime.Now.Year : year;
            var owners = db.Owners.ToList();

            List<VMOwnerHOAQuartersRow> reporte = new List<VMOwnerHOAQuartersRow>();
            owners.Where(ow=>ow.Condos.Count()>0).ToList().ForEach(ow => reporte.AddRange(ow.ReportHOAFeeByYear(year.Value)));

            ViewBag.year = year;

            return View(reporte);
        }

        public ActionResult RentsByYear(int? id, int? year, bool pdfMode = false)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var condo = db.Condoes.Find(id);
            if (condo == null)
                return HttpNotFound();

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
        /// <returns>A rotativa PDF generated from RentsByYear action in PDF Mode.</returns>
        //[Authorize(Roles = ApplicationUser.UserRoles.ADMIN + "," + ApplicationUser.UserRoles.ASISTENTE)]
        public ActionResult DownloadPDfRentsByYear(int? id, int? year)
        {
            var condo = db.Condoes.Find(id);
            if (condo == null)
                return HttpNotFound();
            /*
            System.Web.Routing.RouteValueDictionary rvd = new System.Web.Routing.RouteValueDictionary();
            rvd.Add("id", id);
            rvd.Add("year", year);
            rvd.Add("pdfMode", true);
            var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
            var fileView = new Rotativa.ActionAsPdf("RentsByYear", rvd)
            {
                FileName = $"Rent Imp {condo.name}_{year}" + ".pdf",
                FormsAuthenticationCookieName = System.Web.Security.FormsAuthentication.FormsCookieName,
                Cookies = cookies
            };*/
            var fileView = condo.generateRotativaPDF_RentsByYearReport(year, Request);

            //Code to get content
            return fileView;
        }
    }
}