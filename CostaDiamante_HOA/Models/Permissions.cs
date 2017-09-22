using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CostaDiamante_HOA.Models
{
    public class Permissions
    {
        [Key]
        public int permissionsID { get; set; }

        /*Guest Data*/
        [Required]
        [Display(Name = "Name")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [DisplayName("Guest Full Name")]
        public string fullName
        {
            get { return this.name + " " + this.lastName; }
        }

        public bool isWildCard { get; set; }

        //In the checkin list are registered each permission and period of time it lasts
        [Required]
        public int checkInListID { get; set; }
        public virtual CheckInList checkInList { get; set; }

        //Every visits has a checkin list
        public virtual ICollection<Permissions_Visits> visitsPermited { get; set; }
    }
}