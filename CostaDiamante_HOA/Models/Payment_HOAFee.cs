using CostaDiamante_HOA.GeneralTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CostaDiamante_HOA.Models
{
    public class Payment_HOAFee:Payment
    {
        [DisplayName("Quarter")]
        public int quarterNumber { get; set; }

        [DisplayName("Year")]
        public int year { get; set; }

        //A payment has one owner
        [Display(Name = "Condo")]
        public int? condoID { get; set; }
        public virtual Condo condo { get; set; }

        public VMHOAQuarter quarter { get {
                return new VMHOAQuarter(this.year, this.quarterNumber, this.condo);
            }
        }

        public string sendNotificationEmail(HttpRequestBase Request)
        {
            string errorMessage = string.Empty;

            //Subject
            string subject = $"A HOA Fee payment was registered to condo {this.condo.name}";

            //URL To see Details
            string detailsURL = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
            detailsURL += "/Condo/HOAFees/" + this.condoID;

            //Email Body
            string emailMessage = "<h2>Costa Diamante HOA-System</h2>";
            emailMessage += "<h3>New HOA Fee Payment</h3>";
            emailMessage += $"<span>A new payment of {this.amount.ToString("C")} USD to";
            emailMessage += $" condo {this.condo.name }, property of {this.condo.owner.fullName}.";
            emailMessage += " <span>Click this link to see the details:</span>";
            emailMessage += " <a href='" + detailsURL + "'>Go to visit notification details.</a>";
            Task.Run(() =>
            {
                //Email is sent just to the admin
                var response = MailerSendGrid.sendEmailToMultipleRecipients(subject, emailMessage, null,null);
                errorMessage = response.Result;
            });

            return errorMessage;
        }

    }

    public class VMHOAQuarter
    {
        public int year { get; set; }
        public int quarterNumber { get; set; }
        public DateTime ownerRegistrationDate { get; set; }
        public string ownerID { get; set; }
        public TimePeriod quarterPeriod { get; private set; }
        public decimal HOAFee { get; private set; }
        private List<Payment_HOAFee> payments { get; } = new List<Payment_HOAFee>();


        public VMHOAQuarter() { }

        public VMHOAQuarter(int year, int quarterNumber, Condo condo)
        {
            var owner = condo.owner;
            string strDefaultStartDate = ConfigurationManager.AppSettings["DefaultRegistrationDate"] == null ? string.Empty : ConfigurationManager.AppSettings["DefaultRegistrationDate"];
            this.year = year;
            this.quarterNumber = quarterNumber;
            this.ownerID = condo.ownerID;
            this.ownerRegistrationDate = owner.registrationDate == null ? DateTime.Parse(strDefaultStartDate) : owner.registrationDate.Value;
            if (condo.payments != null && condo.payments.Count() > 0)
            {
                this.payments = condo.payments.Where(p => p.year == year && p.quarterNumber == quarterNumber).OrderByDescending(p => p.date).ToList();
            }
            this.quarterPeriod = this.calculateQuaterPeriod();
            this.HOAFee = VMHOAQuarter.StandarHOAFee;
        }


        /// <summary>
        /// Calculates the Number of months delayed to be payed given a date of reference.
        /// </summary>
        /// <param name="refDate">Date of reference</param>
        /// <param name="ignoreIfDelayed">Ignore if its delayed flag, use it for arbitrary redDate not current one.</param>
        /// <returns>The number of months delayed</returns>
        private int calcNumberOfMonthsDelayed(DateTime refDate, bool ignoreIfDelayed = false)
        {
            int res = 0;
            DateTime date2 = this.quarterPeriod.endDate;
            //DateTime date1 = DateTime.Today;
            DateTime date1 = refDate;

            //If quarter's payment is delayed and reference date is later than end date
            //Note: We can ignore "isDelayed" Flag to use this method as a calculator beetween end date and refDate
            //setting ignoreIfDelayed to true.
            if ((ignoreIfDelayed || this.isDelayed) && date1 >= date2) {
                res = 3 + ((date1.Year - date2.Year) * 12) + date1.Month - date2.Month;
            }


            return res;
        }

        /// <summary>
        /// Calculates the Number of months delayed to be payed given a date of reference.
        /// </summary>
        /// <param name="refDate">Date of reference</param>
        /// <param name="ignoreIfDelayed">Ignore if its delayed flag, use it for arbitrary redDate not current one.</param>
        /// <returns>A amount of interest</returns>
        public decimal calcInterest(DateTime refDate, bool ignoreIfDelayed = false)
        {
            decimal percentInterest = 0;
            string strPercentInterest = ConfigurationManager.AppSettings["HOAFeeInterestPercent"];
            strPercentInterest = String.IsNullOrEmpty(strPercentInterest) ? string.Empty : strPercentInterest;
            decimal.TryParse(strPercentInterest, out percentInterest);
            return (percentInterest * (this.HOAFee / 3) * this.calcNumberOfMonthsDelayed(refDate, ignoreIfDelayed));
        }

        /// <summary>
        /// Calculates the Interests generated until a given date of reference.
        /// <param name="refDate">Date of reference</param>
        /// </summary>
        public int numberOfMonthsDelayed
        {
            get { return this.calcNumberOfMonthsDelayed(DateTime.Today); }
        }

        /// <summary>
        /// Check if quarter is delayed to be paid completely
        /// </summary>
        public Boolean isDelayed
        {
            get
            {
                decimal paidInTime = this.payments.ToList().Where(p => p.date <= this.quarterPeriod.endDate).Sum(p => p.amount);
                if (paidInTime >= this.HOAFee)
                    return false;
                else
                {
                    if (this.payments != null && this.payments.Count() > 0)
                    {
                        var lastPay = this.payments.OrderByDescending(p => p.date).First();
                        var partInteres = this.calcInterest(lastPay.date, true);

                        return this.TotalPaid < (VMHOAQuarter.StandarHOAFee + partInteres);
                    }
                    else
                        return true;
                }
            }
        }

        /// <summary>
        /// Interests generated until current date (server date)
        /// </summary>
        public decimal interest
        {
            get{ return this.calcInterest(DateTime.Today); }
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
        /// The sum of all payments registered to this quarter discarding discount payments.
        /// </summary>
        public decimal TotalRealPaid
        {
            get
            {
                decimal res = 0;
                if (this.payments != null && this.payments.Count() > 0)
                    res = this.payments.Where(p=>!p.isDiscount).Sum(p => p.amount);

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
        /// Get the total amount discounted for this quarter's HOA Fee
        /// </summary>
        public decimal totalDiscounts { get {
                decimal res = 0;
                if (this.payments != null && this.payments.Count() > 0)
                    res = this.payments.Where(p => p.isDiscount).Sum(p => p.amount);
                return res;
            }
        }

        /// <summary>
        /// Period of time for this quarter instance.
        /// </summary>
        public TimePeriod calculateQuaterPeriod()
        {/*
            int quarterDay = int.Parse(ConfigurationManager.AppSettings["QuarterDay"]);
            int startMonth = 1 + (this.quarterNumber - 1) * 3;
            int endMonth = startMonth == 10 ? 1 : startMonth + 3;
            DateTime start = new DateTime(this.year, startMonth, quarterDay);
            DateTime end = new DateTime(startMonth == 10 ? this.year + 1 : this.year, endMonth, quarterDay);
            TimePeriod tpQuarter = new TimePeriod(start, end);

            return tpQuarter;*/
            return VMHOAQuarter.calculateQuarterPeriod(this.quarterNumber, this.year);

        }

        public static TimePeriod calculateQuarterPeriod(int quarterNumber, int year)
        {
            int quarterDay = int.Parse(ConfigurationManager.AppSettings["QuarterDay"]);
            int startMonth = 1 + (quarterNumber - 1) * 3;
            int endMonth = startMonth == 10 ? 1 : startMonth + 3;
            DateTime start = new DateTime(year, startMonth, quarterDay);
            DateTime end = new DateTime(startMonth == 10 ? year + 1 : year, endMonth, quarterDay);
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
                else
                {
                    res = QuarterStatus.INTIME; //Quarter is after owner registration
                    if (this.RemainToPay <= 0)
                        res = QuarterStatus.PAYED; //The HOA Fee and interes was completely paid
                    else
                    {
                        if (this.numberOfMonthsDelayed > 0)
                            res = QuarterStatus.DELAYED; //Delayed one month or more
                        if (this.numberOfMonthsDelayed >= 24)
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

    public class VMOwnerHOAQuartersRow
    {
        public int condoID { get; set; }

        [DisplayName("Condo")]
        public string condoName { get; set; }

        public string ownerID { get; set; }
        [DisplayName("Owner")]
        public string ownerFullName { get; set; }

        public List<VMHOAQuarter> quarters { get; private set; }
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
            this.total = this.quarters.Sum(q => q.TotalRealPaid);
        }
    }
}