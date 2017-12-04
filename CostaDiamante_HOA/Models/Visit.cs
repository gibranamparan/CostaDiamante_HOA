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
        public virtual ICollection<Payment_RentImpact> payments { get; set;  } 

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
            emailMessage += "<span>A new visit of " + this.visitors.Count() + "guest(s) was notified by ";
            emailMessage += this.owner.fullName + " in condo " + this.condo.name + ". ";
            emailMessage += "<span>Click this link to see the details:</span>";
            emailMessage += " <a href='" + visitDetailsURL + "'>Go to visit notification details.</a>";
            Task.Run(() =>
            {
                //Email is sent just to the admin
                var response = MailerSendGrid.sendEmailToMultipleRecipients(subject, emailMessage, null);
                errorMessage = response.Result;
            });

            return errorMessage;
        }
    }
    public enum typeOfVisit
    {
        FRIENDS_AND_FAMILY, BY_RENT
    }
}