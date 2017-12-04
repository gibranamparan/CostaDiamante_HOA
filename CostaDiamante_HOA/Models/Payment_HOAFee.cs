using CostaDiamante_HOA.GeneralTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
                var response = MailerSendGrid.sendEmailToMultipleRecipients(subject, emailMessage, null);
                errorMessage = response.Result;
            });

            return errorMessage;
        }
    }
}