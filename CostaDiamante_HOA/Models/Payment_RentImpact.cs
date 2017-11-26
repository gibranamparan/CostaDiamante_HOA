using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CostaDiamante_HOA.Models
{
    public class Payment_RentImpact : Payment
    {
        //A payment has one visit
        [Display(Name = "Visit #")]
        [ForeignKey("visit")]
        public int visitID { get; set; }
        public virtual Visit visit { get; set; }
    }
}