using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CostaDiamante_HOA.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public string userID { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DisplayName("Name")]
        public string name { get; set; }

        [DisplayName("Phone")]
        public string phone { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string lastname { get; set; }

        [DisplayName("User Permissions As")]
        [Required]
        public string roleName { get; set; }

        [DisplayName("Registration Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
            ApplyFormatInEditMode = true)]
        public DateTime registrationDate { get; set; }

        public string hash { get; set; }
        public string stamp { get; set; }

        /// <summary>
        /// Generates a list to select in a GUI form.
        /// </summary>
        /// <returns>List of items with "Value" and "Text" attributes</returns>
        public static System.Web.Mvc.SelectList selectListUserRoles() {
            var values = new List<string>(ApplicationUser.RoleNames.ROLES_ARRAY);
            var items = from it in values
                        select new { Text = it, Value = it };
            return new System.Web.Mvc.SelectList(items,"Value","Text");
        }


        public RegisterViewModel() { }
        public RegisterViewModel(ApplicationUser owner)
        {
            this.Email = owner.Email;
            this.name = owner.name;
            this.phone = owner.PhoneNumber;
            this.lastname = owner.lastName;
            this.userID = owner.Id;
            this.hash = owner.PasswordHash;
            this.stamp = owner.SecurityStamp;
            this.registrationDate = owner.registrationDate.HasValue ? owner.registrationDate.Value : DateTime.Today;
        }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
