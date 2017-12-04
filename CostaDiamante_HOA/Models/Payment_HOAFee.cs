using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CostaDiamante_HOA.Models
{
    public class Payment_HOAFee:Payment
    {
        [DisplayName("Quarter")]
        public int quarterNumber { get; set; }

        [DisplayName("Year")]
        public int year { get; set; }

        //A payment has one owner
        [Display(Name = "Condo")]
        public int? condoID { get; set; }
        public virtual Condo condo { get; set; }
    }
}