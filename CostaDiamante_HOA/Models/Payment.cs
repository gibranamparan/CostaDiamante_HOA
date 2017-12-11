using CostaDiamante_HOA.GeneralTools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CostaDiamante_HOA.Models
{
    public class Payment
    {
        [Key]
        [Display(Name = "Number #")]
        public int paymentsID { get; set; }

        [Display(Name = "Amount")]
        public decimal amount { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Payment Date")]
        public DateTime date { get; set; }

        [Display(Name = "Is Discount")]
        public bool isDiscount { get; set; }

        [Display(Name = "Reference")]
        public string reference { get; set; }

        [Display(Name = "Type of payment")]
        public TypeOfPayment typeOfPayment { get
            {
                var res = TypeOfPayment.NONE;
                if (this is Payment_RentImpact)
                    res = TypeOfPayment.RENTAL_IMPACT;
                else if (this is Payment_HOAFee)
                    res = TypeOfPayment.HOA_FEE;
                return res;
            }
        }

        public string TypeOfPaymentName {
            get
            {
                var res = string.Empty;
                if (this is Payment_RentImpact)
                    res = GlobalMessages.PAYMENT_TYPE_RENT_IMPACT;
                else if (this is Payment_HOAFee)
                    res = GlobalMessages.PAYMENT_TYPE_HOA_FEE;
                return res;
            }
        }

        public class VMPayment
        {
            /**/
            public int id { get; set; }
            public decimal amount { get; set; }
            public string date { get; set; }
            public int typeOfPayment { get; set; }
            public bool isDiscount { get; set; } = false;
            public string reference { get; set; }

            public VMPayment() { }
            public VMPayment(int paymentsID, decimal amount, DateTime date, TypeOfPayment typeOfPayment)
            {
                id = paymentsID;
                this.amount = amount;
                this.date = date.ToString("s");
                this.typeOfPayment = (int)typeOfPayment;
            }
            public VMPayment(Payment pay)
            {
                id = pay.paymentsID;
                this.amount = pay.amount;
                this.date = pay.date.ToString("s");
                this.typeOfPayment = (int)pay.typeOfPayment;
                this.isDiscount = pay.isDiscount;
                this.reference = string.IsNullOrEmpty(pay.reference)?string.Empty:pay.reference;
            }
        }

        public enum TypeOfPayment
        {
            HOA_FEE, RENTAL_IMPACT, NONE
        }
        
    }
}