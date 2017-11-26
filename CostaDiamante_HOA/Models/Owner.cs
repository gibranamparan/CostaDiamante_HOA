using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CostaDiamante_HOA.Models
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

        //An owner can register many Guests and has many Condos
        public virtual ICollection<Condo> Condos { get; set; }
        public virtual ICollection<Visit> visitsHistory { get; set; }
        public virtual ICollection<Payment_HOAFee> payments { get; set; }


    }
}