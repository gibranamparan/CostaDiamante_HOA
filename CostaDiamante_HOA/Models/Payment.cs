using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CostaDiamante_HOA.Models
{
    public class Payments
    {
        [Key]
        [Display(Name = "Notification #")]
        public int paymentsID { get; set; }

        [Display(Name = "Amount")]
        public decimal amount { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Payment Date")]
        public DateTime date { get; set; }

        [Display(Name = "Type of payment")]
        public typeOfPayment typeOfPayment { get; set; }

        //A payment has one owner
        [Display(Name = "Owner")]
        public string ownerID { get; set; }
        public virtual Owner owner { get; set; }

        //A payment has one visit
        [Display(Name = "Visit #")]
        public int visitID { get; set;  }
        public virtual Visit visit { get; set; }
        
        public class VMPayment
        {
            /**/
            public int id { get; set; }
            public decimal amount { get; set; }
            public string date { get; set; }
            public int typeOfPayment { get; set; }

            public VMPayment() { }
            public VMPayment(int paymentsID, decimal amount, DateTime date, typeOfPayment typeOfPayment)
            {
                id = paymentsID;
                this.amount = amount;
                this.date = date.ToString("s");
                this.typeOfPayment = (int)typeOfPayment;
            }
            public VMPayment(Payments pay)
            {
                id = pay.paymentsID;
                this.amount = pay.amount;
                this.date = pay.date.ToString("s");
                this.typeOfPayment = (int)pay.typeOfPayment;
            }
        }

    }

    public enum typeOfPayment
    {
        HOA_FEE, RENTAL_IMPACT, MIX
    }
}