using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CostaDiamante_HOA.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using CostaDiamante_HOA.GeneralTools;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace CostaDiamante_HOA.Controllers
{
    public class OwnersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private Owner filterOwner(string id)
        {
            Owner owner = null;
            //If owner, show only his details
            if (User.IsInRole(ApplicationUser.RoleNames.OWNER)) {
                var iden = User.Identity;
                var userID = iden.GetUserId();
                owner = db.Owners.Find(userID);
            }
            //If admin, get the requested owner
            else if (User.IsInRole(ApplicationUser.RoleNames.ADMIN))
                owner = db.Owners.Find(id);

            return owner;
        }

        // GET: Owners
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Index()
        {
            var ownersTemp = db.Owners.ToList();

            var admins = db.Users.ToList().Where((adm) => ownersTemp
                .Find(own => own.Id == adm.Id) == null).ToList();

            var owners = (from user in ownersTemp
                      select new RegisterViewModel(user, UserManager)).ToList();

            ViewBag.admins = admins;

            return View(owners);
        }

        // GET: Owners/Details/5
        public ActionResult Details(string id, int year = 0,bool errorGuest=false)
        {

            year = year == 0 ? DateTime.Today.Year : year;
            
            Owner owner = null;
            owner = filterOwner(id);

            if (owner == null)
                return HttpNotFound();

            //Select all condoes not associated to be listed as options to associate
            ViewBag.notAssociatedCondos = db.Condoes.Where(con => String.IsNullOrEmpty(con.ownerID))
                .ToList();

            ViewBag.errorGuest = errorGuest;
            ViewBag.year = year;

            return View(owner);
        }

        [HttpGet]
        public JsonResult GetVisits(string id, DateTime startdate, DateTime enddate) //  DateTime startdate, DateTime enddate
        { 
            TimePeriod periodReported = new TimePeriod(startdate, enddate);
            List<Visit> visits = new List<Visit>();
            
            Owner owner = null;
            owner = filterOwner(id);
            if (owner != null)
            {
                visits = owner.visitsHistory.ToList();

                //Visits are filtered and prepared to be returned as JSON array creating a ViewModel List
                var listVisits = visits.Where(vis => periodReported
                                 .hasInside(vis.timePeriod.startDate))
                                 .OrderBy(vis => vis.timePeriod.startDate)
                                 .Select(vis => new Visit.VMVisits(vis));

                return Json(listVisits, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { });
        }

        // GET: Payments
        [HttpGet]
        //[ValidateHeaderAntiForgeryToken]
        public JsonResult GetQuartersStatus(string id, int year)
        {
            int numReg = 0;
            string errorMsg = string.Empty;
            try
            {
                var owner = filterOwner(id);
                if (owner != null)
                {
                    var condos = owner.Condos;

                    var res = from condo in condos
                              select new { condoID = condo.condoID, condoName = condo.name, status = condo.getHOAStatusByYear(year) };

                    return Json(new { res = res, numReg = res.Count() }, JsonRequestBehavior.AllowGet);
                }
                else
                    errorMsg = "Owner not found by given ID";
            }
            catch (Exception e)
            {
                errorMsg = String.Format("{0}. Details: {1}", e.Message, e.InnerException.Message);
            }

            return Json(new { numReg = numReg, errorMsg = errorMsg }, JsonRequestBehavior.AllowGet);

        }
        
        [HttpPost]
        [FiltrosDeSolicitudes.ValidateHeaderAntiForgeryToken]
        public JsonResult ContactInfo(string id)
        {
            try { 
                var ownersInfo = db.Owners.Find(id).condosInfoContact.ToList();
                ownersInfo.ForEach(ow => ow.owner = null);
                return Json(new { data = ownersInfo, count = ownersInfo.Count() });
            }
            catch(Exception exc)
            {
                return Json(
                    new {
                        count = 0,
                        data = new { errorMsg = $"{exc.Message}. {(exc.InnerException != null ? exc.InnerException.Message : string.Empty)}" }
                    });
            }
        }

        [HttpPost]
        [FiltrosDeSolicitudes.ValidateHeaderAntiForgeryToken]
        public JsonResult AddContactInfo(OwnersInfoContact newContact)
        {
            int count = 0;
            try
            {
                db.OwnersInfoContact.Add(newContact);
                count = db.SaveChanges();
                newContact.owner = null;
                return Json(new { data = newContact, count });
            }
            catch (Exception exc)
            {
                return Json(
                    new
                    {
                        count = 0,
                        data = new { errorMsg = $"{exc.Message}. {(exc.InnerException != null ? exc.InnerException.Message : string.Empty)}" }
                    });
            }
        }

        [HttpPost]
        [FiltrosDeSolicitudes.ValidateHeaderAntiForgeryToken]
        public JsonResult RemoveContactInfo(int id)
        {
            int count = 0;
            try
            {
                var contactInfo = db.OwnersInfoContact.Find(id);
                db.OwnersInfoContact.Remove(contactInfo);
                count = db.SaveChanges();
                return Json(new { data = contactInfo, count });
            }
            catch (Exception exc)
            {
                return Json(
                    new
                    {
                        count = 0,
                        data = new { errorMsg = $"{exc.Message}. {(exc.InnerException != null ? exc.InnerException.Message : string.Empty)}" }
                    });
            }
        }

        public async Task<JsonResult> importOwnerFromDB(DateTime registrationDate)
        {

            int numReg = 0;
            //Get all the different usernames in data to import (they are grouped by OwnerName and Condo)
            var usernames = db.OwnersToImport.ToList().GroupBy(ow => ow.username).Select(grp => grp.FirstOrDefault().username).ToList();
            foreach (var username in usernames)
            {
                //var username = "JosephElli";
                //Check for the first owner in a group of usernames that has email
                var modelToimport = db.OwnersToImport.ToList().FirstOrDefault(ow => ow.username == username && !string.IsNullOrEmpty(ow.EMAIL!=null?ow.EMAIL.Trim(): ow.EMAIL));
                if (modelToimport == null) //If not found
                {
                    //Take the firsone and assing a null email
                    modelToimport = db.OwnersToImport.ToList().FirstOrDefault(ow => ow.username == username);
                    modelToimport.EMAIL = ApplicationUser.NULL_EMAIL;
                }

                //Create owner instance
                var owner = new Owner(modelToimport, registrationDate);
                var result = await UserManager.CreateAsync(owner, modelToimport.password);
                if (result.Succeeded)
                {
                    //Assign role
                    string rolName = ApplicationUser.RoleNames.OWNER;
                    result = UserManager.AddToRole(owner.Id, rolName);

                    if (result.Succeeded)
                    {
                        //Look for user by unique generated username in table
                        var regsToImport = db.OwnersToImport
                            .Where(reg => reg.username == modelToimport.username).ToList();

                        //For register to import
                        OwnersInfoContact ownerInfo;
                        List<OwnersInfoContact> ownersInfoToImport = new List<OwnersInfoContact>();
                        foreach (var reg in regsToImport)
                        {
                            //A contact info owner is created and added to a temp list
                            ownerInfo = new OwnersInfoContact(owner.Id, reg);
                            ownersInfoToImport.Add(ownerInfo);
                        }

                        //Make the relationship condo-owner
                        if (regsToImport.Count() > 0)
                        {
                            var condoNames = regsToImport.SelectMany(reg => reg.condoNames).Distinct().ToArray();
                            //var condoNames = regsToImport.First().condoNames;
                            foreach (string condoName in condoNames)
                            {
                                var condo = db.Condoes.FirstOrDefault(c => c.name.Equals(condoName, StringComparison.InvariantCultureIgnoreCase));
                                int condoID = condo == null ? 0 : condo.condoID;

                                if (condoID > 0)
                                { //If condo bame was found in database
                                    condo.ownerID = owner.Id;
                                    db.Entry(condo).State = EntityState.Modified;
                                }
                            }
                        }

                        //Al the list is added to be saved
                        if (ownersInfoToImport.Count() > 0)
                            db.OwnersInfoContact.AddRange(ownersInfoToImport);
                    }
                }
            }
            numReg = db.SaveChanges();
            return Json(new { numReg });
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
