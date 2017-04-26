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

        public string name { get; set; }
        public string lastName { get; set; }

        //And owner can register many Guests and has many Condoes
        public virtual ICollection<Condo> Condo { get; set; }
        public virtual ICollection<Guest> Guest { get; set; }
    }
}