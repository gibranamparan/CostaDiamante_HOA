using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sonora_HOA.Models
{
    public class Owner:ApplicationUser
    {
        public Owner() { }
        public Owner(RegisterViewModel model) {
            if (!String.IsNullOrEmpty(model.userID))
                this.Id = model.userID;
            this.UserName = model.Email;
            this.Email = model.Email;
            this.name = model.name;
            this.lastName = model.lastname;
            this.PhoneNumber = model.phone;
        }

        [DisplayName("Name")]
        public string name { get; set; }
        [DisplayName("Last Name")]
        public string lastName { get; set; }

        public string fullName {
            get { return this.name + " " + this.lastName; }
        }

        //An owner can register many Guests and has many Condos
        public virtual ICollection<Condo> Condo { get; set; }
        public virtual ICollection<Guest> Guest { get; set; }
    }
}