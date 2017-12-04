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
            }
        }

        public class VMHOAQuarter{
            public int year { get; set; }
            public int quarterNumber { get; set; }
            public DateTime ownerRegistrationDate { get; set; }
            public string ownerID { get; set; }
            public TimePeriod quarterPeriod { get; private set; }
            public decimal HOAFee { get; private set; }
            private List<Payment_HOAFee> payments { get;} = new List<Payment_HOAFee>();


            public VMHOAQuarter(){ }

            public VMHOAQuarter(int year, int quarterNumber, Condo condo)
            {
                var owner = condo.owner;
                string strDefaultStartDate = ConfigurationManager.AppSettings["DefaultRegistrationDate"] == null ? string.Empty : ConfigurationManager.AppSettings["DefaultRegistrationDate"];
                this.year = year;
                this.quarterNumber = quarterNumber;
                this.ownerID = condo.ownerID;
                this.ownerRegistrationDate = owner.registrationDate == null ? DateTime.Parse(strDefaultStartDate) : owner.registrationDate.Value;
                if (condo.payments!=null && condo.payments.Count() > 0) {
                    this.payments = condo.payments.Where(p => p.year == year && p.quarterNumber == quarterNumber).OrderByDescending(p => p.date).ToList();
                }
                this.quarterPeriod = this.calculateQuaterPeriod();
                this.HOAFee = VMHOAQuarter.StandarHOAFee;
            }

            /// <summary>
            /// Number of months delayed to be payed.
            /// </summary>
            public int numberOfMonthsDelayed
            {
                get { 
                    int res = 0;
                    DateTime date2 = this.quarterPeriod.endDate;
                    DateTime date1 = DateTime.Today;

                    if (this.isDelayed && date1 > date2)
                        res = ((date1.Year - date2.Year) * 12) + date1.Month - date2.Month;
                    

                    return res;
                }
            }

            /// <summary>
            /// Check if quarter is delayed to be paid completely
            /// </summary>
            public Boolean isDelayed
            {
                get
                {
                    decimal paidInTime = this.payments.ToList().Where(p => p.date <= this.quarterPeriod.endDate).Sum(p => p.amount);
                    return paidInTime < this.HOAFee;
                }
            }

            public decimal interest
            {
                get
                {
                    decimal percentInterest = 0;
                    string strPercentInterest = ConfigurationManager.AppSettings["HOAFeeInterestPercent"];
                    strPercentInterest = String.IsNullOrEmpty(strPercentInterest) ? string.Empty : strPercentInterest;
                    decimal.TryParse(strPercentInterest, out percentInterest);
                    return (percentInterest * (this.HOAFee/3) * this.numberOfMonthsDelayed);
                }
            }

            /// <summary>
            /// HOA Fee standar in web.config to be applied to every owner
            /// </summary>
            public static decimal StandarHOAFee
            {
                get
                {
                    string strToPay = ConfigurationManager.AppSettings["HOAStandarFee"];
                    decimal toPay = 0;
                    decimal.TryParse(strToPay, out toPay);
                    return toPay;
                }
            }

            /// <summary>
            /// The sum of all payments registered to this quarter.
            /// </summary>
            public decimal TotalPaid
            {
                get
                {
                    decimal res = 0;
                    if (this.payments != null && this.payments.Count() > 0)
                        res = this.payments.Sum(p => p.amount);

                    return res;
                }
            }

            /// <summary>
            /// The total to be paid to fullfill quarter HOA Fee
            /// </summary>
            public decimal RemainToPay
            {
                get { return (this.HOAFee + this.interest) - this.TotalPaid; }
            }

            /// <summary>
            /// Period of time for this quarter instance.
            /// </summary>
            public TimePeriod calculateQuaterPeriod()
            {
                int quarterDay = int.Parse(ConfigurationManager.AppSettings["QuarterDay"]);
                int startMonth = 1 + (this.quarterNumber - 1) * 3;
                int endMonth = startMonth==10 ? 1 : startMonth + 3;
                DateTime start = new DateTime(this.year, startMonth , quarterDay);
                DateTime end = new DateTime(startMonth == 10 ? this.year+1: this.year, endMonth , quarterDay);
                TimePeriod tpQuarter = new TimePeriod(start, end);

                return tpQuarter;
                
            }

            /// <summary>
            /// Returns the CurrentStatus as an enum taking determining the conditions for each state.
            /// See QuarterStatus enum Summary for more information for every state.
            /// </summary>
            public QuarterStatus currentStatus
            {
                get
                {
                    QuarterStatus res = QuarterStatus.NONE;
                    if (this.quarterPeriod.startDate <= this.ownerRegistrationDate)
                        res = QuarterStatus.OUT_OF_TIME; //If quartes is before owner registration
                    else { 
                        res = QuarterStatus.INTIME; //Quarter is after owner registration
                        if (this.RemainToPay <= 0)
                            res = QuarterStatus.PAYED; //The HOA Fee and interes was completely paid
                        else
                        {
                            if (this.numberOfMonthsDelayed > 0)
                                res = QuarterStatus.DELAYED; //Delayed one month or more
                            if(this.numberOfMonthsDelayed >= 24)
                                res = QuarterStatus.DELINQUENCY; //Delayed 2 years or more
                        }
                    }

                    return res;
                }
            }

            /// <summary>
            /// Enumeration that categorize all the possibles states a quarter may have.
            /// OUT OF TIME for quarters which startdate its before the owner's registration date.
            /// INTIME for quarters which startdate its after the owner's registration date.
            /// PAYED for quarters that the HOA Fee and interes was completely paid
            /// DELAYED for quarters that have not been completely paid and have more than 1 month delayed
            /// DELINQUENCY for quartes that are delayed 2 years or more
            /// </summary>
            public enum QuarterStatus
            {
                NONE, OUT_OF_TIME, INTIME, PAYED, DELAYED, DELINQUENCY
            }

        }

        public enum TypeOfPayment
        {
            HOA_FEE, RENTAL_IMPACT, NONE
        }
    }
}