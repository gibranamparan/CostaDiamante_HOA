﻿using CostaDiamante_HOA.GeneralTools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Web;
using Rotativa;
using System.Web.Mvc;
using System.Threading.Tasks;

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
            private readonly string STRING_FORMAT_QUERY_STRING = "id={0}&year={1}&quarter={2}&type-of-invoice={3}";

            public int condoID { get; set; }
            public DateTime sendDate { get; set; }
            public TypeOfPayment typeOfInvoice { get; set; }
            public int year { get; set; }
            public int quarter { get; set; }


            public InvoiceFormGenerator() { }
            public InvoiceFormGenerator(string queryString)
            {
                System.Collections.Specialized.NameValueCollection res = HttpUtility.ParseQueryString(queryString);

                int condoID = 0;
                int.TryParse(res["id"] ?? string.Empty, out condoID);
                this.condoID = condoID;

                int quarter = 0;
                int.TryParse(res["quarter"] ?? string.Empty, out quarter);
                this.quarter = quarter;

                int year = DateTime.Today.Year;
                int.TryParse(res["year"] ?? string.Empty, out year);
                this.year = year;

                // If valid, assing input value, else assing NONE
                TypeOfPayment typeOfPayment = TypeOfPayment.NONE;
                Enum.TryParse(res["type-of-invoice"] ?? string.Empty, out typeOfPayment);
                this.typeOfInvoice = typeOfPayment;
            }

            /// <summary>
            /// Transforms a InvoiceArguments objecto into its query string format.
            /// </summary>
            /// <param name="args">InvoiceArguments object</param>
            /// <returns>Query string ready to be part of a URL</returns>
            public string QueryString
            {
                get { 
                    string res = String.Format(this.STRING_FORMAT_QUERY_STRING, this.condoID,this.year, this.quarter, this.typeOfInvoice);
                    return res;
                }
            }
        }

        /// <summary>
        /// Contiene toda la informacion basica que un invoice debe contener para generar un formato enviable
        /// </summary>
        public abstract class Invoice
        {
            public DateTime date { get; set; }
            public int year { get; set; }
            public decimal amount { get; set; }
            public Condo condo { get; set; }
            public string queryString { get; set; }
            public string cryptedQueryString { get; set; }

            public TypeOfPayment typeOfInvoice
            {
                get
                {
                    if (this is InvoiceHOA)
                        return TypeOfPayment.HOA_FEE;
                    else if (this is InvoiceRent)
                        return TypeOfPayment.RENTAL_IMPACT;
                    else
                        return TypeOfPayment.NONE;
                }
            }

            public abstract decimal totalDue { get; }

            public Invoice(Condo condo, InvoiceFormGenerator invoiceForm)
            {
                this.condo = condo;
                this.year = invoiceForm.year;
                this.date = invoiceForm.sendDate;
            }

            public abstract Rotativa.ActionAsPdf generateInvoicePDF(HttpRequestBase Request);
            public abstract Task<Payment.InvoiceSentStatus> sendInvoice(HttpRequestBase Request, ControllerContext ControllerContext);

            public static Rotativa.ActionAsPdf generatePDF(Condo condo, HttpRequestBase request, int? id, int? year, int? quarter, TypeOfPayment typeOfInvoice)
            {
                InvoiceFormGenerator ifg = new InvoiceFormGenerator() { quarter = quarter.Value, typeOfInvoice = typeOfInvoice, year = year.Value };

                Invoice inv = null;

                if (typeOfInvoice == TypeOfPayment.HOA_FEE)
                    inv = new InvoiceHOA(condo, ifg);
                else if (typeOfInvoice == TypeOfPayment.RENTAL_IMPACT)
                    inv = new InvoiceRent(condo, ifg);

                var fileView = inv.generateInvoicePDF(request);

                //Code to get content
                return fileView;
            }
        }

        /// <summary>
        /// Modelo de respuesta de invoi
        /// </summary>
        public class InvoiceSentStatus : InvoiceFormGenerator
        {
            public string condoName { get; set; }
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

            public string description
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


            public static string quarterMonths(int quarter)
            {
                string res = string.Empty;
                int startMonth = 1;
                if(quarter>=1 && quarter <= 4)
                {
                    startMonth = 3 * (quarter - 1) + 1;
                    for (int c = startMonth; c <= startMonth + 2; c++) {
                        //var dt = new DateTime(DateTime.Today.Year,quarter + c,DateTime.Today.Day);
                        var dt = new DateTime(DateTime.Today.Year, c , 28);
                        var separator = c == startMonth ? ", " : ( c == startMonth +1 ? " & " : "" );
                        res += dt.ToString("MMMM")+separator;
                    }
                }
                return res;
            }

            /// <summary>
            /// Generates Rotativa PDF
            /// </summary>
            /// <param name="Request"></param>
            /// <returns></returns>
            public override ActionAsPdf generateInvoicePDF(HttpRequestBase Request)
            {
                return this.condo.generateRotativaPDF_HOAFeeInvoice(this.year, this.quarter, Request);
            }

            /// <summary>
            /// Send invoice by email to every related contact
            /// </summary>
            /// <param name="Request"></param>
            /// <param name="ControllerContext"></param>
            /// <returns></returns>
            public override Task<Payment.InvoiceSentStatus> sendInvoice(HttpRequestBase Request, ControllerContext ControllerContext)
            {
                return this.condo.sendEmail_HOAFeeInvoice(Request, ControllerContext, this.quarter, this.year, this.cryptedQueryString);
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

            public override decimal totalDue
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            /// <summary>
            /// Generates Rotativa PDF
            /// </summary>
            /// <param name="Request"></param>
            /// <returns></returns>
            public override ActionAsPdf generateInvoicePDF(HttpRequestBase Request)
            {
                return this.condo.generateRotativaPDF_RentsByYearReport(this.year, Request);
            }

            /// <summary>
            /// Send invoice by email to every related contact
            /// </summary>
            /// <param name="Request"></param>
            /// <param name="ControllerContext"></param>
            /// <returns></returns>
            public override Task<Payment.InvoiceSentStatus> sendInvoice(HttpRequestBase Request, ControllerContext ControllerContext)
            {
                return this.condo.sendEmail_ImpactOfRentReport(Request, ControllerContext, this.year, this.cryptedQueryString);
            }
        }
    }
}