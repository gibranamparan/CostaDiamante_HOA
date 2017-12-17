using CostaDiamante_HOA.GeneralTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using System.Configuration;

namespace CostaDiamante_HOA.Models
{
    public class Visit
    {
        public static int MAX_GUESTS_ALLOWED = 8;

        [Key]
        [Display(Name = "Notification #")]
        public int visitID { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Notification Date")]
        public DateTime date { get; set; }

        [Display(Name = "Type of visit")]
        public typeOfVisit typeOfVisit { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Arrival")]
        public DateTime arrivalDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Departure")]
        public DateTime departureDate { get; set; }

        [Display(Name = "Period")]
        public TimePeriod timePeriod { get {
                return new TimePeriod(this.arrivalDate, this.departureDate);
            }
        }

        [Display(Name = "Total Cost")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal totalCost { get; set; }

        [Display(Name = "Payment omitted")]
        public bool paymentOmitted { get; set; }

        //A visits is for one condo
        [Display(Name = "Condo Number")]
        public int condoID { get; set; }
        public virtual Condo condo { get; set; }

        //A visits is for one owner
        [Display(Name = "Owner")]
        public string ownerID { get; set; }
        public virtual Owner owner { get; set; }

        //Every visits has a list of visitors
        public virtual ICollection<Visitor> visitors { get; set; }

        //Every visits has a list of payments
        public virtual ICollection<Payment_RentImpact> payments { get; set; }

        [DisplayName("Paid")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal totalPaid
        {
            get
            {
                decimal res = 0;
                if (this.payments != null && this.payments.Count() > 0)
                    res = this.payments.Sum(p => p.amount);
                return res;
            }
        }

        [DisplayName("Discount")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal totalDiscount
        {
            get
            {
                decimal res = 0;
                if (this.payments != null && this.payments.Count() > 0)
                    res = this.payments.Where(p=>p.isDiscount).Sum(p => p.amount);
                return res;
            }
        }

        public bool isInHouseInPeriod(TimePeriod tp)
        {
            return tp.hasPartInside(this.timePeriod) || this.timePeriod.hasPartInside(tp);
        }

        public class VMVisitsFilter
        {
            private TimePeriod _timePeriod { get; set; }
            public TimePeriod TimePeriod {
                get {
                    if(_timePeriod == null)
                    {
                        _timePeriod = new TimePeriod(DateTime.Now,DateTime.Now.AddDays(7));
                    }
                    return _timePeriod;
                }
                set { _timePeriod = value; } }

            [DisplayName("Include Inhouse Visits")]
            public bool isInHouse { get; set; }
        }

        /// <summary>
        /// A visit model is created to avoid returning all collections that have to do with this model.
        /// </summary>
        public class VMVisits
        {
            public int visitID { get; set; }

            public DateTime arrivalDate { get; set; }

            public DateTime departureDate { get; set; }

            public typeOfVisit typeOfVisit { get; set; }

            public VMVisits(Visit visit)
            {
                this.visitID = visit.visitID;
                this.arrivalDate = visit.arrivalDate;
                this.departureDate = visit.departureDate;
                this.typeOfVisit = visit.typeOfVisit;
            }
        }

        /// <summary>
        /// Sends an email to de administrator notifying that the current visit
        /// was registered in the system.
        /// </summary>
        /// <param name="Request">HTTPRequestBase from the controller to get the URL from the web server
        /// currentrly running the application.</param>
        /// <returns>A string with an error message, if its empty or null, everything worked OK or the async task did't finished before the response was sent to the client.</returns>
        public string sendNotificationEmail(HttpRequestBase Request)
        {
            string errorMessage = string.Empty;

            //Subject
            string subject = "A visit was notified by " + this.owner.fullName;

            //URL To see Details
            string visitDetailsURL = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
            visitDetailsURL += "/Visits/Details/" + this.visitID;

            //Email Body
            string emailMessage = "<h2>Costa Diamante HOA-System</h2>";
            emailMessage += "<h3>New Visit Notified</h3>";
            emailMessage += "<span>A new visit of " + this.visitors.Count() + " guest(s) was notified by ";
            emailMessage += this.owner.fullName + " in condo " + this.condo.name + ". ";
            emailMessage += "<span>Click this link to see the details:</span>";
            emailMessage += " <a href='" + visitDetailsURL + "'>Go to visit notification details.</a>";
            Task.Run(() =>
            {
                //Email is sent just to the admin
                var response = MailerSendGrid.sendEmailToMultipleRecipients(subject, emailMessage, null,null);
                errorMessage = response.Result;
            });

            return errorMessage;
        }
    }

    /// <summary>
    /// Enumeration to list the different types of visits
    /// </summary>
    public enum typeOfVisit
    {
        FRIENDS_AND_FAMILY, BY_RENT
    }

    public class VMVisitreport
    {
        [DisplayName("Arrival Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime arrivalDate { get; set; }

        [DisplayName("Departure Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime departureDate { get; set; }

        public TimePeriod period { get
            {
                return new TimePeriod(this.arrivalDate, this.departureDate);
            }
        }

        [DisplayName("Renters")]
        public int renters { get; set; }

        public bool isRent { get; private set;}

        [DisplayName("Cost")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal cost { get; set; }

        [DisplayName("Discount")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal discount { get; set; }

        [DisplayName("Paid")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal paid { get; set; }

        [DisplayName("Days Rented")]
        public int daysRented {
            get
            {
                int res = 0;
                if (this.period != null)
                    res = this.period.time.Days;
                return res;
            }
        }

        public VMVisitreport(Visit v)
        {
            this.departureDate = v.departureDate;
            this.arrivalDate = v.arrivalDate;
            this.renters = v.visitors.Count();
            this.isRent = v.typeOfVisit == typeOfVisit.BY_RENT;
            this.cost = v.totalCost;
            this.discount = v.totalDiscount;
            this.paid = v.totalPaid;
        }
    }
}