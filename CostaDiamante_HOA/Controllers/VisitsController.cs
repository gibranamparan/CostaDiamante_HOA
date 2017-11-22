using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CostaDiamante_HOA.Models;
using CostaDiamante_HOA.GeneralTools;
using static CostaDiamante_HOA.GeneralTools.FiltrosDeSolicitudes;
using Microsoft.AspNet.Identity;
using System.Web.Script.Serialization;

using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using System.Configuration;

namespace CostaDiamante_HOA.Controllers
{
    [Authorize]
    public class VisitsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Visits
        public ActionResult Index(Visit.VMVisitsFilter visitsFilter)
        {
            TimePeriod periodReported = visitsFilter.TimePeriod;
            bool isInHouse = visitsFilter.isInHouse;

            List<Visit> visits = new List<Visit>();
            //If user is admin, shows every visit in database
            if (User.IsInRole(ApplicationUser.RoleNames.ADMIN))
                visits = db.Visits.ToList();
            //If user is an owner, shows just his visits
            else if (User.IsInRole(ApplicationUser.RoleNames.OWNER))
            {
                string ownerID = User.Identity.GetUserId();
                Owner owner = db.Owners.Find(ownerID);
                visits = owner.visitsHistory.ToList();
            }
            if (periodReported.Equals(new TimePeriod()))
                periodReported = new TimePeriod(DateTime.Today, DateTime.Today.AddDays(7));

            //Se filtran visitas
            if (!isInHouse)
            {
                visits = visits.Where(vis => periodReported
                    .hasInside(vis.timePeriod.startDate))
                    .OrderBy(vis => vis.timePeriod.startDate).ToList();
            }
            else
            {
                visits = visits.Where(vis => vis.isInHouseInPeriod(periodReported))
                    .OrderBy(vis => vis.timePeriod.startDate).ToList();
            }

            ViewBag.result = visits;
            return View(visitsFilter);
        }

        public async Task<JsonResult> SendEmail()
        {
            //var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var apiKey = ConfigurationManager.AppSettings["sendGrindAPIKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("gibranamparand@hotmail.com", "Gibran Amparan");
            var subject = "Sending with SendGrid is Fun for test to Cost Diamante";
            var to = new List<EmailAddress> { new EmailAddress("gibranamparan@netcodesolutions.net", "Netcode") };
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        // GET: Visits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visit visits = db.Visits.Find(id);
            if (visits == null)
            {
                return HttpNotFound();
            }
            return View(visits);
        }

        // GET: Visits/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Condo condo = db.Condoes.Find(id);
            if (condo == null)
                return HttpNotFound();

            Visit visit = new Visit();
            visit.condoID = id.Value;
            visit.ownerID = condo.owner.Id;
            visit.owner = condo.owner;
            return View(visit);
        }

        // POST: Visits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Create(Visit visit)
        {
            int numReg = 0;
            string errorMsg = string.Empty;
            visit.date = DateTime.Now;
            try { 
                if (ModelState.IsValid)
                {
                    db.Visits.Add(visit);
                    numReg = db.SaveChanges();
                    return Json(new { numReg = numReg });
                }
            }catch(Exception e)
            {
                errorMsg = String.Format("{0}. Details: {1}",e.Message,e.InnerException.Message);
            }

            return Json(new { numReg = numReg, errorMsg = errorMsg });
        }

        // GET: Visits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visit visits = db.Visits.Find(id);
            if (visits == null)
            {
                return HttpNotFound();
            }
            ViewBag.condoID = new SelectList(db.Condoes, "condoID", "name", visits.condoID);
            ViewBag.ownerID = new SelectList(db.Users, "Id", "name", visits.ownerID);
            return View(visits);
        }

        // POST: Visits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Edit(Visit visit)
        {
            int numReg = 0;
            string errorMsg = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(visit).State = EntityState.Modified;
                    numReg = db.SaveChanges();
                    return Json(new { numReg = numReg });
                }
            }
            catch (Exception e)
            {
                errorMsg = String.Format("{0}. Details: {1}", e.Message, e.InnerException.Message);
            }

            return Json(new { numReg = numReg, errorMsg = errorMsg });
        }

        // GET: Visits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visit visits = db.Visits.Find(id);
            if (visits == null)
            {
                return HttpNotFound();
            }
            return View(visits);
        }

        // POST: Visits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Visit visits = db.Visits.Find(id);
            db.Visits.Remove(visits);
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
