using CostaDiamante_HOA.GeneralTools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CostaDiamante_HOA.Models
{
    public class Payment_RentImpact : Payment
    {
        //A payment has one visit
        [Display(Name = "Visit #")]
        [ForeignKey("visit")]
        public int visitID { get; set; }
        public virtual Visit visit { get; set; }

        public string sendEmailNotification(HttpRequestBase Request)
        {
            string errorMessage = string.Empty;

            //Subject
            string subject = $"A impact of rent payment was registered to visit from {this.visit.timePeriod} in condo {this.visit.condo.name}";

            //URL To see Details
            int year = this.visit.arrivalDate.Year;
            string detailsURL = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
            detailsURL += $"/Reports/DownloadPDfRentsByYear/{this.visit.condoID}?year={year}";

            //Email Body
            string emailMessage = "<h2>Costa Diamante HOA-System</h2>";
            emailMessage += "<h3>New Rent Impact Payment</h3>";
            emailMessage += $"<span>A new impact rent payment of {this.amount.ToString("C")} USD to";
            emailMessage += $" condo {this.visit.condo.name }, property of {this.visit.condo.owner.fullName} related to visit from {this.visit.timePeriod}";
            emailMessage += $" for {this.visit.visitors.Count()} guests.";
            emailMessage += $" <span>Click this link to download your updated Impact of Rent status report for year {year}:</span>";
            emailMessage += " <a href='" + detailsURL + "'>Download Impact of Rent Report Year .</a>";
            Task.Run(() =>
            {
                var ownerAdress = new List<SendGrid.Helpers.Mail.EmailAddress>
                { new SendGrid.Helpers.Mail.EmailAddress(this.visit.owner.Email, this.visit.owner.fullName) };
                //Email is sent just to the admin
                var response = MailerSendGrid.sendEmailToMultipleRecipients(subject, emailMessage, ownerAdress);
                errorMessage = response.Result;
            });

            return errorMessage;
        }
    }
}