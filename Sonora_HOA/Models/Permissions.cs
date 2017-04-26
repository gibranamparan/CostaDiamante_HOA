using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public DateTime startDate { get; set; }

        //Condo where visit is permited
        public int number { get; set; }
        public virtual Condo condo { get; set; }

        //Guest who is permited to visit
        public int guestID { get; set; }
        public virtual Guest guest { get; set; }

        public virtual ICollection<Permissions_Visits> permisionsvisits { get; set; }
    }
}