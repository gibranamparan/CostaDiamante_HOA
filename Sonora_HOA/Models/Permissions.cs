using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sonora_HOA.Models
{
    public class Permissions
    {
        public int permissionsID { get; set; }
        [DisplayFormat(DataFormatString = "{0 :yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime startDate { get; set; }

        public virtual Condo condo { get; set; }
        public int number { get; set; }

        public virtual Guest guest { get; set; }
        public int guestID { get; set; }

        public virtual ICollection<Permissions_Visits> permisionsvisits { get; set; }
    }
}