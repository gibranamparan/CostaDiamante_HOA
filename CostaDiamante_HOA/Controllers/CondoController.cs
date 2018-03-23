using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using CostaDiamante_HOA.Models;
using static CostaDiamante_HOA.GeneralTools.FiltrosDeSolicitudes;

namespace CostaDiamante_HOA.Controllers
{
    [Authorize]
    public class CondoController : Controller
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

        // GET: Condo
        [HttpGet]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Index()
        {
            var condoes = db.Condoes.Include(c => c.owner);
            return View(condoes.ToList());
        }

        // GET: Condo
        [HttpGet]
        [ValidateHeaderAntiForgeryToken]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult List()
        {
            try { 
                var condoes = db.Condoes.ToList().Select(con=>new Condo.VMCondo(con));
                var res = condoes.ToList();
                return Json(new { condos = res, count = res.Count }, JsonRequestBehavior.AllowGet);
            }catch(Exception exc)
            {
                return Json(new { errorMessage = $"{exc.Message}." + 
                    (exc.InnerException != null ? exc.InnerException.Message : string.Empty) }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Condo/Create
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Create()
        {
            ViewBag.ownerID = new SelectList(db.Owners.ToList(), "Id", "fullName");
            return View();
        }

        // POST: Condo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Create([Bind(Include = "num,name,ownerID")] Condo condo, Boolean fromIndex=false)
        {
            if (ModelState.IsValid)
            {
                db.Condoes.Add(condo);
                db.SaveChanges();
                return RedirectToAction("Index", "Condo");
            }

            ViewBag.ownerID = new SelectList(db.Owners, "Id", "fullName", condo.ownerID);
            return View(condo);
        }

        // GET: Condo/Edit/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condo condo = db.Condoes.Find(id);
            if (condo == null)
            {
                return HttpNotFound();
            }
            ViewBag.ownerID = new SelectList(db.Owners, "Id", "fullName", condo.ownerID);
            return View(condo);
        }

        // POST: Condo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Edit([Bind(Include = "condoID,name,ownerID")] Condo condo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(condo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.Owners, "Id", "fullName", condo.ownerID);
            return View(condo);
        }

        // GET: Condo/Delete/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condo condo = db.Condoes.Find(id);
            if (condo == null)
            {
                return HttpNotFound();
            }
            return View(condo);
        }

        // POST: Condo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult DeleteConfirmed(int id)
        {
            Condo condo = db.Condoes.Find(id);
            db.Condoes.Remove(condo);
            db.SaveChanges();
            return RedirectToAction("Index", "Condo");
            //return RedirectToAction("Details","Owners",new { id=ownerID});
        }

        // POST: Condo/Delete/5
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public JsonResult RemoveFromOwner(int id=0)
        {
            int num = 0;
            Condo condo = db.Condoes.Find(id);
            if (condo != null) { 
                condo.ownerID = null;
                var entry = db.Entry(condo);
                entry.Property(con => con.ownerID).IsModified = true;
                num = db.SaveChanges();
            }
            return Json(num);
        }

        private class AreaCondo { public string prefix = ""; public int limit = 0; }
        /// <summary>
        /// Action to generate automatically every condo acording to the standar format for names in the HOA Office
        /// </summary>
        /// <returns>Json HTTP Response reporting how many condos were created</returns>
        public JsonResult generateCondos()
        {
            var registeredCondos = db.Condoes;
            var areas = new AreaCondo[] {
                new AreaCondo { prefix="A",limit = 8 },
                new AreaCondo { prefix="B",limit = 10 },
                new AreaCondo { prefix="C",limit = 9 },
                new AreaCondo { prefix="D",limit = 15 },
                new AreaCondo { prefix="E",limit = 21 },
                new AreaCondo { prefix="F",limit = 49 },
            };

            String condoName = "";
            //Every condo name is generated for every tower in the complex
            areas.ToList().ForEach(area =>
            {
                for (int c = 1; c <= area.limit; c++)
                {
                    condoName = area.prefix + c.ToString("00");
                    //
                    if(condoName != "C02" && condoName != "D07" && condoName != "F11") { 
                        if (condoName == "C01")
                            condoName += "-C02";
                        if (condoName == "D06")
                            condoName += "-D07";
                        if (condoName == "F10")
                            condoName += "-F11";

                        //It's registered if doesn't already exists
                        if (registeredCondos.FirstOrDefault(con => con.name == condoName) == null)
                            db.Condoes.Add(new Condo(condoName));
                    }
                }
            });

            int numberOfGeneratedCondos = db.SaveChanges();
            return Json(new { numberOfGeneratedCondos = numberOfGeneratedCondos }, 
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public JsonResult AssociateCondo(List<Condo> condosToAssociate, string ownerID)
        {
            //Associating condos in the array to owner
            foreach(Condo condo in condosToAssociate)
                db.Entry(condo).State = EntityState.Modified;

            int numRegChanged = db.SaveChanges();

            return Json(numRegChanged);
        }
        /*
        public JsonResult removeAllCondoes()
        {
            int numReg = 0;
            db.Condoes.ToList().ForEach(con =>
            {
                db.Payments.RemoveRange(con.payments);
                db.Condoes.Remove(con);
            });
            numReg = db.SaveChanges();
            return Json(numReg, JsonRequestBehavior.AllowGet);
        }*/
        [HandleError]
        public ActionResult HOAFees(int? id, int? year)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Condo condo = db.Condoes.Find(id);
            if (condo == null)
                return HttpNotFound();

            if(!isAllowedToAccess(condo.ownerID))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            if (year.HasValue) ViewBag.year = year;
            return View(condo);
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
