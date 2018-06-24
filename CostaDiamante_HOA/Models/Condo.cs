using CostaDiamante_HOA.GeneralTools;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CostaDiamante_HOA.Models
{
    public class Condo
    {
        [Key]
        public int condoID { get; set; }
        [Display(Name = "Condo Number")]
        public string name { get; set; }
        
        //A condo has one owner
        [ForeignKey("owner")]
        [Display(Name = "Owner")]
        public string ownerID { get; set; }
        public virtual Owner owner { get; set; }

        public Condo() { }
        public Condo(string name)
        {
            this.name = name;
        }

        //A condo can be visited with many permissions
        public virtual ICollection<Visit> visitsHistory { get; set; }

        //Every condo has a HOAFees history
        public virtual ICollection<Payment_HOAFee> payments { get; set; }

        /// <summary>
        /// Generates the HOA Fees payment status for every quarter of a year of owner's current instance.
        /// </summary>
        /// <param name="year">Year for the report.</param>
        /// <returns>Return a List of VMHOAQuartes, ordered from 1sts quarter of January-April to 
        /// last quarter from October to January of next year.</returns>
        public List<VMHOAQuarter> getHOAStatusByYear(int year)
        {
            List<VMHOAQuarter> quartersStatus = new List<VMHOAQuarter>();
            int delayed = 0;
            for (int quarter = 1; quarter <= 4; quarter++)
            {
                quartersStatus.Add(new VMHOAQuarter(year, quarter, this));
            }
            var interes = quartersStatus[1].interest;

            return quartersStatus;
        }

        /// <summary>
        /// Genera un PDF del reporte de todos los pagos de HOAFEe hechos en un trimestre de año
        /// </summary>
        /// <param name="year"></param>
        /// <param name="quarter">numero del 1 al 4 que representa un trimestre de año.</param>
        /// <param name="Request">Contexto HTTP actual.</param>
        /// <returns></returns>
        public Rotativa.ActionAsPdf generateRotativaPDF_RentsByYearReport(int? year, HttpRequestBase Request)
        {
            System.Web.Routing.RouteValueDictionary rvd = new System.Web.Routing.RouteValueDictionary();
            rvd.Add("id", this.condoID);
            rvd.Add("year", year);
            rvd.Add("pdfMode", true);
            var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
            var fileView = new Rotativa.ActionAsPdf("../Reports/RentsByYear", rvd)
            {
                FileName = $"Rent Imp {this.name}_{year}_{DateTime.Today.ToString("dd-MMM-yyyy")}" + ".pdf",
                FormsAuthenticationCookieName = System.Web.Security.FormsAuthentication.FormsCookieName,
                Cookies = cookies
            };

            return fileView;
        }

        /// <summary>
        /// Genera un PDF del reporte de todos los pagos de impacto de renta en un año dado
        /// </summary>
        /// <param name="year"></param>
        /// <param name="Request">Contexto HTTP actual.</param>
        /// <returns></returns>
        public Rotativa.ActionAsPdf generateRotativaPDF_HOAFeeInvoice(int? year, int? quarter, HttpRequestBase Request)
        {
            System.Web.Routing.RouteValueDictionary rvd = new System.Web.Routing.RouteValueDictionary();
            rvd.Add("id", this.condoID);
            rvd.Add("year", year);
            rvd.Add("quarter", quarter);
            rvd.Add("pdfMode", true);
            var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
            var fileView = new Rotativa.ActionAsPdf("../Invoice/Invoice_HOAFee", rvd)
            {
                FileName = $"HOAFee Invoice {this.name}_{year}_qrt{quarter}{DateTime.Today.ToString("dd-MMM-yyyy")}" + ".pdf",
                FormsAuthenticationCookieName = System.Web.Security.FormsAuthentication.FormsCookieName,
                Cookies = cookies
            };

            return fileView;
        }

        public VMOwnerHOAQuartersRow ReportHOAFeeByYear(int year)
        {
            return new VMOwnerHOAQuartersRow(this, year);
        }

        /// <summary>
        /// Sends by email a PDF of "Impact of Rent Report" of a the current instance given a specific year.
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="controllerContext"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<Payment.InvoiceSentStatus> sendEmail_ImpactOfRentReport(HttpRequestBase Request, ControllerContext controllerContext, int year = 0, string cryptedQueryString = "")
        {
            string errorMessage = string.Empty;
            year = year == 0 ? DateTime.Today.Year : year;
            string strDate = DateTime.Today.ToString("dd MMMM yyyy");

            //Subject
            string subject = $"Impact of Rent report to {strDate} in condo {this.name}";

            //URL To see Details
            string downloadURL = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
            downloadURL += $"/Invoice/DownloadInvoiceByToken?cryptedStr={cryptedQueryString}";

            //Email Body
            string emailMessage = "<h2>Costa Diamante HOA-System</h2>";
            emailMessage += $"<h3>Impact of Rent Report</h3>";

            emailMessage += $"<span>In this email you will find a link to download your Impact of Rent status report for year {year} for";
            emailMessage += $" , condo {this.name }, property of {this.owner.name}. Click the following link:";

            //Add link to see report if its a visit that cause Impact of Rent
            emailMessage += " <a href='" + downloadURL + "'>Download Impact of Rent Report.</a>";

            //Generate attachments for email
            List<Attachment> attachments = null;

            //Is mailer enabled
            bool emailEnabled = true;
            Boolean.TryParse(ConfigurationManager.AppSettings["enableEmail"], out emailEnabled);
            //Just send Impact of Rent report for paid visits marked as that.
            /*
            if (controllerContext != null && emailEnabled)
            {
                //Generate the report to be send
                var fileView = this.generateRotativaPDF_RentsByYearReport(year, Request);
                //Converts report to PDF file
                var fileBytes = fileView.BuildPdf(controllerContext);
                //Add PDF file to attachments
                Attachment attach = new Attachment() { Filename = fileView.FileName, Content = Convert.ToBase64String(fileBytes), Type = "application/pdf" };
                attachments = new List<Attachment>() { attach };
            }*/

            //Async sending of email
            //Compose destination
            var ownerAdress = new List<SendGrid.Helpers.Mail.EmailAddress>
                { new SendGrid.Helpers.Mail.EmailAddress(this.owner.Email.Trim(), this.owner.name) };
            var contacts = owner.condosInfoContact;
            foreach(var con in contacts) //For each related contact, an email is added to receipts
            {
                //If its a valid email, different from the owner emails, its added as an aditional receipment
                if (MailerSendGrid.IsValid(con.email) && con.email != ApplicationUser.NULL_EMAIL && con.email.Trim() != owner.Email.Trim())
                    ownerAdress.Add(new SendGrid.Helpers.Mail.EmailAddress(con.email.Trim(), con.ownerName));
            }

            //Email is sent just to the admin
            MailerSendGrid.MailerResult res = await MailerSendGrid.sendEmailToMultipleRecipients(subject, emailMessage, ownerAdress, attachments);
            Payment.InvoiceSentStatus status = new Payment.InvoiceSentStatus { condoID = this.condoID, mailStatus = res, sendDate = DateTime.Today };

            return status;
        }

        public async Task<Payment.InvoiceSentStatus> sendEmail_HOAFeeInvoice(HttpRequestBase Request, ControllerContext controllerContext, int quarter, int year = 0, string cryptedQueryString = "")
        {
            string errorMessage = string.Empty;
            year = year == 0 ? DateTime.Today.Year : year;
            string strDate = DateTime.Today.ToString("dd MMMM yyyy");

            //Subject
            string subject = $"HOA Fee invoice to for quarter { Payment.InvoiceHOA.quarterMonths(quarter) } of { year } in condo {this.name}";

            //URL To see Details
            string downloadURL = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
            downloadURL += $"/Invoice/DownloadInvoiceByToken?cryptedStr={cryptedQueryString}";

            //Email Body
            string emailMessage = "<h2>Costa Diamante HOA-System</h2>";
            emailMessage += $"<h3>HOA Fee Invoice</h3>";
            emailMessage += $"<span>In this email you will find a link to download your HOA Invoice for { Humanizer.OrdinalizeExtensions.Ordinalize(quarter) } quarter { Payment.InvoiceHOA.quarterMonths(quarter) } in year {year}";
            emailMessage += $" , condo {this.name }, property of {this.owner.name}. Click the following link:";

            //Add link to see report if its a visit that cause Impact of Rent
            emailMessage += " <a href='" + downloadURL + "'>Download HOA Invoice.</a>";

            //Generate attachments for email
            List<Attachment> attachments = null;

            //Is mailer enabled
            bool emailEnabled = true;
            Boolean.TryParse(ConfigurationManager.AppSettings["enableEmail"], out emailEnabled);
            //Just send Impact of Rent report for paid visits marked as that.
            /*
            if (controllerContext != null && emailEnabled)
            {
                //Generate the report to be send
                var fileView = this.generateRotativaPDF_HOAFeeInvoice(year,quarter, Request);
                //Converts report to PDF file
                var fileBytes = fileView.BuildPdf(controllerContext);
                //Add PDF file to attachments
                Attachment attach = new Attachment() { Filename = fileView.FileName, Content = Convert.ToBase64String(fileBytes), Type = "application/pdf" };
                attachments = new List<Attachment>() { attach };
            }*/

            //Async sending of email
            //Compose destination
            var ownerAdress = new List<SendGrid.Helpers.Mail.EmailAddress>
                { new SendGrid.Helpers.Mail.EmailAddress(this.owner.Email.Trim(), this.owner.name) };
            var contacts = owner.condosInfoContact;
            foreach (var con in contacts) //For each related contact, an email is added to receipts
            {
                //If its a valid email, different from the owner emails, its added as an aditional receipment
                if (MailerSendGrid.IsValid(con.email) && con.email != ApplicationUser.NULL_EMAIL && con.email.Trim() != owner.Email.Trim())
                    ownerAdress.Add(new SendGrid.Helpers.Mail.EmailAddress(con.email.Trim(), con.ownerName));
            }

            //Email is sent just to the admin
            MailerSendGrid.MailerResult res = await MailerSendGrid.sendEmailToMultipleRecipients(subject, emailMessage, ownerAdress, attachments);
            Payment.InvoiceSentStatus status = new Payment.InvoiceSentStatus { condoID = this.condoID, mailStatus = res, sendDate = DateTime.Today };

            return status;
        }

        /// <summary>
        /// View Model for condo used for API
        /// </summary>
        public class VMCondo
        {
            public int condoID { get; set; }
            public string condoName { get; set; }

            public string ownerID { get; set; }
            public string ownerName { get; set; }

            public string[] emails{ get; set; }
            public string primaryEmail { get; set; }

            public string concatEmails { get
                {
                    string concat = emails.Count() > 0 ? string.Join(", ", this.emails) : string.Empty;
                    return (string.IsNullOrEmpty(this.primaryEmail) ? string.Empty : (this.primaryEmail+", ")) + concat;
                } }

            public VMCondo() { }
            public VMCondo(Condo condo)
            {
                this.condoID = condo.condoID;
                this.condoName = condo.name;
                this.ownerID = condo.ownerID;
                this.ownerName = condo.owner != null ? condo.owner.fullName : string.Empty;
                this.emails = condo.owner != null ? condo.owner.condosInfoContact.ToList().Select(info=>info.email).ToArray() : new string[0];
                this.primaryEmail = condo.owner == null ? string.Empty : condo.owner.Email;
            }

        }

    }
}