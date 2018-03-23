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
                if (this is Payment_RentImpact) { //Payments for rents
                    res = GlobalMessages.PAYMENT_TYPE_RENT_IMPACT; //Default impact rent
                    if(((Payment_RentImpact)this).visit.typeOfVisit == typeOfVisit.FRIENDS_AND_FAMILY){
                        res = GlobalMessages.PAYMENT_TYPE_FAMILY_FRIENDS_RENT; //Family rent
                    }
                }
                else if (this is Payment_HOAFee)
                    res = GlobalMessages.PAYMENT_TYPE_HOA_FEE; //Payment for HOAFee
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

        public class InvoiceFactory
        {
            public Invoice generateInvoice(TypeOfPayment invoiceType, Condo condo, InvoiceFormGenerator invoiceForm)
            {
                if (invoiceType == TypeOfPayment.HOA_FEE)
                    return new InvoiceHOA(condo, invoiceForm);
                else if (invoiceType == TypeOfPayment.RENTAL_IMPACT)
                    return new InvoiceRent(condo, invoiceForm);
                else
                    return null;
            }
        }

        /// <summary>
        /// Modelo para recibir la forma de solicitud de envios de correos masivos
        /// </summary>
        public class InvoiceFormGenerator
        {
            public DateTime sendDate { get; set; }
            public TypeOfPayment typeOfInvoice { get; set; }
            public int year { get; set; }
            public int quarter { get; set; }
        }

        /// <summary>
        /// Contiene toda la informacion basica que un invoice debe contener para generar un formato enviable
        /// </summary>
        public abstract class Invoice
        {
            public DateTime date { get; set; }
            public int year { get; set; }
            public decimal amount { get; set; }
            public TypeOfPayment typeOfInvoice { get; set; }
            public Condo condo { get; set; }

            public abstract string description { get; }
            public abstract decimal totalDue { get; }
            public abstract InvoiceSentStatus sendInvoice();

            public Invoice(Condo condo, InvoiceFormGenerator invoiceForm)
            {
                this.condo = condo;
                this.year = invoiceForm.year;
                this.typeOfInvoice = invoiceForm.typeOfInvoice;
                this.date = invoiceForm.sendDate;
            }
        }

        /// <summary>
        /// Modelo de respuesta de invoi
        /// </summary>
        public class InvoiceSentStatus : InvoiceFormGenerator
        {
            public int condoID { get; set; }
            public MailerSendGrid.MailerResult mailStatus { get; set; }
        }

        /// <summary>
        /// Representa un Invoice de pago de HOAFee.
        /// </summary>
        public class InvoiceHOA : Invoice
        {
            public int quarter { get; set; }

            public InvoiceHOA(Condo condo, InvoiceFormGenerator invoiceForm) 
                : base(condo, invoiceForm)
            {
                this.quarter = invoiceForm.quarter;
            }

            public override string description
            {
                get
                {
                    return $"HOA Dues: {InvoiceHOA.ordinalize(this.quarter)} quarter {year} ({InvoiceHOA.quarterMonths(this.quarter)})";
                }
            }

            public override decimal totalDue
            {
                get
                {
                    return this.amount;
                }
            }

            private static string ordinalize(int num)
            {
                return num == 1 ? "1st" : num == 2 ? "2nd" : num == 3 ? "3th" : "";
            }

            private static string quarterMonths(int quarter)
            {
                string res = string.Empty;
                if(quarter>=1 && quarter <= 4)
                {
                    for (int c = 0; c <= 2; c++) {
                        var dt = new DateTime(DateTime.Today.Year,quarter + c,DateTime.Today.Day);
                        var separator = c == 1 ? ", " : c == 2 ? " & " : "";
                        res += dt.ToString("MMMM")+separator;
                    }
                }
                return res;
            }

            public override InvoiceSentStatus sendInvoice()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Representa los datos de un invoice para impactom de renta
        /// </summary>
        public class InvoiceRent : Invoice
        {
            public InvoiceRent(Condo condo, InvoiceFormGenerator invoiceForm) 
                : base(condo, invoiceForm)
            {
            }

            public override string description
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override decimal totalDue
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override InvoiceSentStatus sendInvoice()
            {
                throw new NotImplementedException();
            }
        }
    }
}