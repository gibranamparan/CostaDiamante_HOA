using CostaDiamante_HOA.GeneralTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CostaDiamante_HOA.Models
{
    public class Visit
    {
        public static int MAX_GUESTS_ALLOWED = 8;

        [Key]
        [Display(Name = "Notification #")]
        public int visitID { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Notification Date")]
        public DateTime date { get; set; }

        [Display(Name = "Type of visit")]
        public typeOfVisit typeOfVisit { get; set; }

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

        [Display(Name = "Period")]
        public TimePeriod timePeriod { get {
                return new TimePeriod(this.arrivalDate, this.departureDate);
            }
        }
        
        [Display(Name = "Total")]
        public decimal totalCost { get; set; }

        [Display(Name = "Payment omitted")]
        public bool paymentOmitted { get; set; }

        //A visits is for one condo
        [Display(Name = "Condo Number")]
        public int condoID { get; set; }
        public virtual Condo condo { get; set; }

        //A visits is for one owner
        [Display(Name = "Owner")]
        public string ownerID { get; set; }
        public virtual Owner owner { get; set; }

        //Every visits has a list of visitors
        public virtual ICollection<Visitor> visitors { get; set; }

        //Every visits has a list of payments
        public virtual ICollection<Payments> payments { get; set;  } 

        public bool isInHouseInPeriod(TimePeriod tp)
        {
            return tp.hasPartInside(this.timePeriod) || this.timePeriod.hasPartInside(tp);
        }

        public class VMVisitsFilter
        {
            private TimePeriod _timePeriod { get; set; }
            public TimePeriod TimePeriod {
                get {
                    if(_timePeriod == null)
                    {
                        _timePeriod = new TimePeriod(DateTime.Now,DateTime.Now.AddDays(7));
                    }
                    return _timePeriod;
                }
                set { _timePeriod = value; } }

            [DisplayName("Include Inhouse Visits")]
            public bool isInHouse { get; set; }
        }
    }
    public enum typeOfVisit
    {
        FRIENDS_AND_FAMILY, BY_RENT
    }
}