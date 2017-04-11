using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sonora_HOA.Models
{
    public class Permissions
    {
        public int permissionsid { get; set; }
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime startDate { get; set; }

        public virtual Condo condo { get; set; }
        public virtual int number { get; set; }

        public virtual Guest guest { get; set; }
        public virtual int guestid { get; set; }
    }
}