using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sonora_HOA.Models
{
    public class Condo
    {
        [Key]
        public int condoID { get; set; }

        [Display(Name = "Number")]
        public int num { get; set; }

        [Display(Name = "Condo Name")]
        public string name { get; set; }

        //A condo has one owner
        [ForeignKey("owner")]
        [Display(Name = "Owner")]
        public string ownerID { get; set; }
        public virtual Owner owner { get; set; }

        //A condo can be visited with many permissions
        public virtual ICollection<Permissions> Permissions { get; set; }
    }
}