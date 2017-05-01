﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sonora_HOA.Models
{
    public class Guest
    {
        [Key]
        [Display(Name = "Guest")]
        public int guestID { get; set; }
        [Display(Name = "Name")]
        public string name { get; set; }
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        //One guest can be invited and registered by one owner
        [ForeignKey("owner")]
        [Display(Name = "Owner")]
        public string Id { get; set; }
        public virtual Owner owner { get; set; }

        [DisplayName("Full Name")]
        public string fullName
        {
            get { return this.name + " " + this.lastName; }
        }

        //One guest can has many permissions in time to visit the condoes
        public virtual ICollection<Permissions> Permissions { get; set; }
    }
}