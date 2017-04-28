using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sonora_HOA.Models
{
    public class Owner:ApplicationUser
    {
        public Owner() { }
        public Owner(RegisterViewModel model) {
            this.UserName = model.Email;
            this.Email = model.Email;
            this.name = model.name;
            this.lastName = model.lastname;
        }

        [Display(Name = "Name")]
        public string name { get; set; }
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        //An owner can register many Guests and has many Condos
        public virtual ICollection<Condo> Condo { get; set; }
        public virtual ICollection<Guest> Guest { get; set; }
    }
}