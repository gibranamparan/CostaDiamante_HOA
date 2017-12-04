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

            List<Condo.VMOwnerHOAQuartersRow> reporte = new List<Condo.VMOwnerHOAQuartersRow>();
            owners.Where(ow=>ow.Condos.Count()>0).ToList().ForEach(ow => reporte.AddRange(ow.ReportHOAFeeByYear(year.Value)));

            ViewBag.year = year;

            return View(reporte);
        }
    }
}