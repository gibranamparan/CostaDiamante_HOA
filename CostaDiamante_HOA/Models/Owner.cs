using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CostaDiamante_HOA.GeneralTools;
using CampanasDelDesierto_v1.GeneralTools;
using OfficeOpenXml;
using System.ComponentModel;

namespace CostaDiamante_HOA.Models
{
    public class Owner:ApplicationUser
    {
        [DisplayName("Name")]
        public string name { get; set; }

        public string fullName { get {
                return name;
            } }

        public virtual ICollection<OwnersInfoContact> condosInfoContact { get; set; }
        
        public Owner() { }
        
        /// <summary>
        /// Given the data introduced in a form saved in a RegisterViewMolde instance, 
        /// a new Owner instance is created.
        /// </summary>
        /// <param name="model"></param>
        public Owner(RegisterViewModel model) {
            if (!String.IsNullOrEmpty(model.userID))
                this.Id = model.userID;
            this.UserName = model.UserName;
            this.Email = model.Email;
            this.name = model.name;
            this.PhoneNumber = model.phone;
            this.registrationDate = model.registrationDate;
        }

        public Owner(Owners_ExcelClass model, DateTime registrationDate)
        {
            this.UserName = model.username;
            this.Email = model.EMAIL;
            this.name = model.NAME;
            this.PhoneNumber = model.PHONE;
            this.registrationDate = registrationDate;
        }

        /// <summary>
        /// Generates a report to show the status of HOA payments for every quarter in a year
        /// of the current instance of an Owner for its condos.
        /// </summary>
        /// <param name="year">The year of the report</param>
        /// <returns></returns>
        public List<VMOwnerHOAQuartersRow> ReportHOAFeeByYear(int year)
        {
            List<VMOwnerHOAQuartersRow> report = new List<VMOwnerHOAQuartersRow>();
            //For each condo, generates the HOA Fees payments status in a year
            this.Condos.ToList().ForEach(con => report.Add(con.ReportHOAFeeByYear(year)));

            return report;
        }

        //An owner can register many Guests and has many Condos
        public virtual ICollection<Condo> Condos { get; set; }
        public virtual ICollection<Visit> visitsHistory { get; set; }
        

    }
}