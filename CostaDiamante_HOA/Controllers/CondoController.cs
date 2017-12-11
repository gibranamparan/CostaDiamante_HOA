﻿using System;
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
    [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
    public class CondoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Condo
        public ActionResult Index()
        {
            var condoes = db.Condoes.Include(c => c.owner);
            return View(condoes.ToList());
        }

        // GET: Condo/Create
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
        public JsonResult AssociateCondo(List<Condo> condosToAssociate, string ownerID)
        {
            //Associating condos in the array to owner
            foreach(Condo condo in condosToAssociate)
                db.Entry(condo).State = EntityState.Modified;

            int numRegChanged = db.SaveChanges();

            return Json(numRegChanged);
        }

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
        }

        public ActionResult HOAFees(int? id, int? year)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Condo condo = db.Condoes.Find(id);
            if (condo == null)
                return HttpNotFound();

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
