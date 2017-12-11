using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CostaDiamante_HOA.Models
{
    public class Owner:ApplicationUser
    {
        public Owner() { }
        
        /// <summary>
        /// Given the data introduced in a form saved in a RegisterViewMolde instance, 
        /// a new Owner instance is created.
        /// </summary>
        /// <param name="model"></param>
        public Owner(RegisterViewModel model) {
            if (!String.IsNullOrEmpty(model.userID))
                this.Id = model.userID;
            this.UserName = model.Email;
            this.Email = model.Email;
            this.name = model.name;
            this.lastName = model.lastname;
            this.PhoneNumber = model.phone;
            this.registrationDate = model.registrationDate;
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