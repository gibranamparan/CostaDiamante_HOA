using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CostaDiamante_HOA.Models
{
    public class Condo
    {
        [Key]
        public int condoID { get; set; }
        [Display(Name = "Condo Number")]
        public string name { get; set; }

        //A condo has one owner
        [ForeignKey("owner")]
        [Display(Name = "Owner")]
        public string ownerID { get; set; }
        public virtual Owner owner { get; set; }

        public Condo() { }
        public Condo(string name)
        {
            this.name = name;
        }

        //A condo can be visited with many permissions
        public virtual ICollection<Visits> visitsHistory { get; set; }
    }
}