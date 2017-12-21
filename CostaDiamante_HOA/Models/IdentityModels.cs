using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace CostaDiamante_HOA.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public ApplicationUser() { }
        public ApplicationUser(RegisterViewModel model)
        {
            if (!System.String.IsNullOrEmpty(model.userID))
                this.Id = model.userID;
            this.UserName = model.Email;
            this.Email = model.Email;
            this.PhoneNumber = model.phone;
            this.name = model.name;
            this.lastName= model.lastname;
            this.PasswordHash = model.hash;
            this.SecurityStamp = model.stamp;
            this.registrationDate = model.registrationDate;
        }

        /// <summary>
        /// Overload that given an userID, it get EmailConfirmed from database
        /// </summary>
        /// <param name="model">Data that come from the form</param>
        /// <param name="db">Application DB Context</param>
        public ApplicationUser(RegisterViewModel model, ApplicationDbContext db) : this(model)
        {
            var userFromDB = db.Users.Find(model.userID);

            this.EmailConfirmed = userFromDB.EmailConfirmed;
            this.PhoneNumberConfirmed = this.PhoneNumberConfirmed;
            this.TwoFactorEnabled = this.TwoFactorEnabled;
            this.LockoutEnabled = this.LockoutEnabled;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        [DisplayName("Name")]
        public string name { get; set; }
        [DisplayName("Last Name")]
        public string lastName { get; set; }

        [DisplayName("Owner")]
        public string fullName
        {
            get { return this.name + " " + this.lastName; }
        }

        [DisplayName("Registered At")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? registrationDate { get; set; }

        public static class RoleNames
        {
            public const string OWNER = "Owner";
            public const string ADMIN = "Admin";
            public const string LANDLORD = "Landlord";
            public static string[] ROLES_ARRAY = new string[] { OWNER, LANDLORD, ADMIN };
        }

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        

        public System.Data.Entity.DbSet<CostaDiamante_HOA.Models.Owner> Owners { get; set; }

        public System.Data.Entity.DbSet<CostaDiamante_HOA.Models.Payment> Payments { get; set; }

        public System.Data.Entity.DbSet<CostaDiamante_HOA.Models.Condo> Condoes { get; set; }

        public System.Data.Entity.DbSet<CostaDiamante_HOA.Models.Visit> Visits { get; set; }

        public System.Data.Entity.DbSet<CostaDiamante_HOA.Models.Visitor> Visitor { get; set; }

        public System.Data.Entity.DbSet<CostaDiamante_HOA.Models.Payment_HOAFee> Payment_HOAFee { get; set; }

        public System.Data.Entity.DbSet<CostaDiamante_HOA.Models.Payment_RentImpact> Payment_RentImpact { get; set; }
    }
}