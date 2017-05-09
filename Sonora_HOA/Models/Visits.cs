using Sonora_HOA.GeneralTools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sonora_HOA.Models
{
    public class Visits
    {
        public static int MAX_GUESTS_ALLOWED = 8;

        [Key]
        public int visitsID { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Notification Date")]
        public DateTime date { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Arrival")]
        public DateTime arrivalDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Departure")]
        public DateTime departureDate { get; set; }

        public TimePeriod timePeriod { get {
                return new TimePeriod(this.arrivalDate, this.departureDate);
            } }

        //A visits is for one condo
        public int condoID { get; set; }
        public virtual Condo condo { get; set; }

        //A visits is for one condo
        public string ownerID { get; set; }
        public virtual Owner owner { get; set; }

        //Every visits has a checkin list
        public virtual ICollection<Permissions_Visits> visitors { get; set; }
    }
}