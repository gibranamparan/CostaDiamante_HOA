using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Sonora_HOA.Models
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
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public static class RoleNames
        {
            public static string OWNER = "Owner";
            public static string ADMIN = "Admin";
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

        public System.Data.Entity.DbSet<Sonora_HOA.Models.Owner> Owners { get; set; }

        public System.Data.Entity.DbSet<Sonora_HOA.Models.Guest> Guests { get; set; }

        public System.Data.Entity.DbSet<Sonora_HOA.Models.Condo> Condoes { get; set; }

        public System.Data.Entity.DbSet<Sonora_HOA.Models.Permissions> Permissions { get; set; }

        public System.Data.Entity.DbSet<Sonora_HOA.Models.Visits> Visits { get; set; }

        public System.Data.Entity.DbSet<Sonora_HOA.Models.CheckInList> CheckInLists { get; set; }

        public System.Data.Entity.DbSet<Sonora_HOA.Models.Permissions_Visits> Permissions_Visits { get; set; }
    }
}