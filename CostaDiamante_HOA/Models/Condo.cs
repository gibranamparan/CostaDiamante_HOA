﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CostaDiamante_HOA.Models
{
    public class Condo
    {
        [Key]
        public int condoID { get; set; }
        [Display(Name = "Condo Number")]
        public string name { get; set; }

        //A condo has one owner
        [ForeignKey("owner")]
        [Display(Name = "Owner")]
        public string ownerID { get; set; }
        public virtual Owner owner { get; set; }

        public Condo() { }
        public Condo(string name)
        {
            this.name = name;
        }

        //A condo can be visited with many permissions
        public virtual ICollection<Visit> visitsHistory { get; set; }

        //Every condo has a HOAFees history
        public virtual ICollection<Payment_HOAFee> payments { get; set; }

        /// <summary>
        /// Generates the HOA Fees payment status for every quarter of a year of owner's current instance.
        /// </summary>
        /// <param name="year">Year for the report.</param>
        /// <returns>Return a List of VMHOAQuartes, ordered from 1sts quarter of January-April to 
        /// last quarter from October to January of next year.</returns>
        public List<VMHOAQuarter> getHOAStatusByYear(int year)
        {
            List<VMHOAQuarter> quartersStatus = new List<VMHOAQuarter>();
            int delayed = 0;
            for (int quarter = 1; quarter <= 4; quarter++)
            {
                quartersStatus.Add(new VMHOAQuarter(year, quarter, this));
            }
            var interes = quartersStatus[1].interest;

            return quartersStatus;
        }

        public Rotativa.ActionAsPdf generateRotativaPDF_RentsByYearReport(int? year, HttpRequestBase Request)
        {
            System.Web.Routing.RouteValueDictionary rvd = new System.Web.Routing.RouteValueDictionary();
            rvd.Add("id", this.condoID);
            rvd.Add("year", year);
            rvd.Add("pdfMode", true);
            var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
            var fileView = new Rotativa.ActionAsPdf("../Reports/RentsByYear", rvd)
            {
                FileName = $"Rent Imp {this.name}_{year}" + ".pdf",
                FormsAuthenticationCookieName = System.Web.Security.FormsAuthentication.FormsCookieName,
                Cookies = cookies
            };

            return fileView;
        }

        public VMOwnerHOAQuartersRow ReportHOAFeeByYear(int year)
        {
            return new VMOwnerHOAQuartersRow(this, year);
        }
    }
}