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
            public decimal Amount { get; set; }
            public string Date { get; set; }
            public int TypeOfPayment { get; set; }

            public VMPayment() { }
            public VMPayment(int paymentsID, decimal amount, DateTime date, typeOfPayment typeOfPayment)
            {
                id = paymentsID;
                Amount = amount;
                Date = date.ToString("s");
                TypeOfPayment = (int)typeOfPayment;
            }
            public VMPayment(Payments pay)
            {
                id = pay.paymentsID;
                Amount = pay.amount;
                Date = pay.date.ToString("s");
                TypeOfPayment = (int)pay.typeOfPayment;
            }
        }

    }

    public enum typeOfPayment
    {
        HOA_FEE, RENTAL_IMPACT, MIX
    }
}