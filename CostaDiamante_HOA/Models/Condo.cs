using System;
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
        public List<Payment.VMHOAQuarter> getHOAStatusByYear(int year)
        {
            List<Payment.VMHOAQuarter> quartersStatus = new List<Payment.VMHOAQuarter>();
            int delayed = 0;
            for (int quarter = 1; quarter <= 4; quarter++)
            {
                quartersStatus.Add(new Payment.VMHOAQuarter(year, quarter, this));
            }
            var interes = quartersStatus[1].interest;

            return quartersStatus;
        }

        public VMOwnerHOAQuartersRow ReportHOAFeeByYear(int year)
        {
            return new Condo.VMOwnerHOAQuartersRow(this, year);
        }

        public class VMOwnerHOAQuartersRow
        {
            public int condoID { get; set; }

            [DisplayName("Condo")]
            public string condoName { get; set; }

            public string ownerID { get; set; }
            [DisplayName("Owner")]
            public string ownerFullName { get; set; }

            public List<Payment.VMHOAQuarter> quarters { get; private set; }
            [DisplayName("Year")]
            public int year { get; set; }

            [DisplayName("Total")]
            public decimal total { get; set; }

            public VMOwnerHOAQuartersRow() { }
            public VMOwnerHOAQuartersRow(Condo condo, int year)
            {
                this.condoID = condo.condoID;
                this.condoName = condo.name;
                this.ownerID = condo.ownerID;
                this.ownerFullName = condo.owner.fullName;
                this.year = year;
                this.quarters = condo.getHOAStatusByYear(year);
                this.total = this.quarters.Sum(q => q.TotalPaid);
            }
        }
    }
}