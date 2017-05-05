using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sonora_HOA.Models
{
    public class Permissions
    {
        [Key]
        public int permissionsID { get; set; }
        
        //Guest who is permited to visit
        [Display(Name = "Guest")]
        public int guestID { get; set; }
        public virtual Guest guest { get; set; }

        //In the checkin list are registered each permission and period of time it lasts
        public int checkInListID { get; set; }
        public virtual CheckInList checkInList { get; set; }

        //Every visits has a checkin list
        public virtual ICollection<Visits> visits { get; set; }
    }
}