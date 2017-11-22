using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CostaDiamante_HOA.Models
{
    public class Visitor
    {
        [Key]
        [Display(Name = "Notification #")]
        public int visitorID { get; set; }

        [Display(Name = "Name")]
        public string name { get; set; }

        [Display(Name = "Last name")]
        public string lastName { get; set; }

        [Display(Name = "Name")]
        public string fullName {
            get {
                return String.Format("{0} {1}", this.name, this.lastName);
            }
        }

        [Display(Name = "IsYounger")]
        public Boolean isYounger { get; set; }

        //A visitor is for one visit
        [Display(Name = "Visit")]
        public virtual Visit visit { get; set; }
    }
}