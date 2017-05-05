using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sonora_HOA.Models
{
    public class Permissions
    {
        [Key]
        public int permissionsID { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", 
            ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime startDate { get; set; }


        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
            ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime endDate { get { return this.startDate.AddMonths(6); } }
        
        //Guest who is permited to visit
        [Display(Name = "Guest")]
        public int guestID { get; set; }
        public virtual Guest guest { get; set; }

        public virtual ICollection<Permissions_Visits> permisionsvisits { get; set; }
    }
}