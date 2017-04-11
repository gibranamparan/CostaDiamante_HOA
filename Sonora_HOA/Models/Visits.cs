using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sonora_HOA.Models
{
    public class Visits
    {
        [Key]
        public int visitsID { get; set; }

        [DisplayFormat(DataFormatString = "{0 :yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime date { get; set; }

        [DisplayFormat(DataFormatString = "{0 :yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime arrivedDate { get; set; }

        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime departureDate { get; set; }

        public virtual ICollection<Permissions_Visits> permisionsvisits {get;set;} 
    }
}