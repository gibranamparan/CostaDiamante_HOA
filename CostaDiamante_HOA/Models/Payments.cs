﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CostaDiamante_HOA.Models
{
    public class Payments
    {
        [Key]
        [Display(Name = "Notification #")]
        public int paymentsID { get; set; }

        [Display(Name = "Amount")]
        public double amount { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Notification Date")]
        public DateTime date { get; set; }

        [Display(Name = "Type of payment")]
        public typeOfPayment typeOfPayment { get; set; }

        //A payment has one owner
        [Display(Name = "Owner")]
        public string ownerID { get; set; }
        public virtual Owner owner { get; set; }

    }

    public enum typeOfPayment
    {
        HOA_FEE, RENTAL_IMPACT, MIX
    }
}