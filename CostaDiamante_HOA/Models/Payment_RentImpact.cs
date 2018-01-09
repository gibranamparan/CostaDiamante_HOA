using CostaDiamante_HOA.GeneralTools;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CostaDiamante_HOA.Models
{
    public class Payment_RentImpact : Payment
    {
        //A payment has one visit
        [Display(Name = "Visit #")]
        [ForeignKey("visit")]
        public int visitID { get; set; }
        public virtual Visit visit { get; set; }

        public string sendEmailNotification_NewRentPayment(HttpRequestBase Request, ControllerContext controllerContext)
        {
            string errorMessage = string.Empty;

            //Subject
            string subject = $"A impact of rent payment was registered to visit from {this.visit.timePeriod} in condo {this.visit.condo.name}";

            //URL To see Details
            int year = this.visit.arrivalDate.Year;
            string detailsURL = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
            detailsURL += $"/Reports/RentsByYear/{this.visit.condoID}?year={year}";

            //Email Body
            string emailMessage = "<h2>Costa Diamante HOA-System</h2>";
            emailMessage += $"<h3>Notification of a New {this.TypeOfPaymentName}</h3>";
            emailMessage += $"<span>A new payment of {this.amount.ToString("C")} USD to";
            emailMessage += $" condo {this.visit.condo.name }, property of {this.visit.condo.owner.fullName} related to visit from {this.visit.timePeriod}";
            emailMessage += $" for {this.visit.visitors.Count()} guests.";

            //Generate attachments for email
            List<Attachment> attachments = null;
            if (this.visit.typeOfVisit == typeOfVisit.BY_RENT || (this.visit.typeOfVisit == typeOfVisit.FRIENDS_AND_FAMILY && !this.visit.withTheOwner))
            { //Add link to see report if its a visit that cause Impact of Rent
                emailMessage += $" <span>See attached PDF or click this link to go to your Impact of Rent status report for year {year}:</span>";
                emailMessage += " <a href='" + detailsURL + "'>Download Impact of Rent Report Year .</a>";

                //Is mailer enabled
                bool emailEnabled = true;
                Boolean.TryParse(ConfigurationManager.AppSettings["enableEmail"], out emailEnabled);
                //Just send Impact of Rent report for paid visits marked as that.
                if (controllerContext != null && emailEnabled)
                {
                    //Generate the report to be send
                    var fileView = this.visit.condo.generateRotativaPDF_RentsByYearReport(year, Request);
                    //Converts report to PDF file
                    var fileBytes = fileView.BuildPdf(controllerContext);
                    //Add PDF file to attachments
                    Attachment attach = new Attachment() { Filename = fileView.FileName, Content = Convert.ToBase64String(fileBytes), Type = "application/pdf" };
                    attachments = new List<Attachment>() { attach };
                }
            }
            //Async sending of email
            Task.Run(() =>
            {
                //Compose destination
                var ownerAdress = new List<SendGrid.Helpers.Mail.EmailAddress>
                { new SendGrid.Helpers.Mail.EmailAddress(this.visit.owner.Email, this.visit.owner.fullName) };
                //Email is sent just to the admin
                var response = MailerSendGrid.sendEmailToMultipleRecipients(subject, emailMessage, ownerAdress, attachments);
                errorMessage = response.Result;
            });

            return errorMessage;
        }
    }
}