using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CostaDiamante_HOA.Models;
using static CostaDiamante_HOA.GeneralTools.FiltrosDeSolicitudes;

namespace CostaDiamante_HOA.Controllers
{
    public class VisitorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public JsonResult Index(int id)
        {
            //var payments = db.Payments.Include(p => p.owner).Include(p => p.visit);
            var visitors = db.Visitor.Where(a => a.visitID == id)
                        .Select(a => new
                        {
                            visitorID = a.visitorID,
                            name = a.name,
                            lastName = a.lastName,
                            visitID = a.visitID
                            });
            return Json(visitors);
        }

        // POST: Visitors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN+","+ApplicationUser.RoleNames.LANDLORD)]
        public JsonResult Create(Visitor visitor)
        {
            int numReg = 0;
            string errorMsg = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Visitor.Add(visitor);
                    numReg = db.SaveChanges();
                    numReg = visitor.visitorID;
                    return Json(new { numReg = numReg });
                }
            }
            catch (Exception e)
            {
                errorMsg = String.Format("{0}. Details: {1}", e.Message, e.InnerException.Message);
            }
            return Json(new { numReg = numReg, errorMsg = errorMsg });
        }

        // POST: Visitors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateHeaderAntiForgeryToken]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public JsonResult DeleteConfirmed(int id)
        {
            int numReg = 0;
            string errorMsg = string.Empty;
            try
            {
                Visitor visitor = db.Visitor.Find(id);
                db.Visitor.Remove(visitor);
                numReg = db.SaveChanges();
                return Json(new { numReg = numReg });
            }
            catch (Exception e)
            {
                errorMsg = String.Format("{0}. Details: {1}", e.Message, e.InnerException.Message);
            }

            return Json(new { numReg = numReg, errorMsg = errorMsg });
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
