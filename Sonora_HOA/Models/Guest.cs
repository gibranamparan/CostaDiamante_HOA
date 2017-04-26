using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sonora_HOA.Models
{
    public class Guest
    {
        [Key]
        public int guestID { get; set; }

        public string name { get; set; }
        public string lastName { get; set; }

        //One guest can be invited and registered by one owner
        [ForeignKey("owner")]
        public string Id { get; set; }
        public virtual Owner owner { get; set; }
       

        //One guest can has many permissions in time to visit the condoes
        public virtual ICollection<Permissions> Permissions { get; set; }
    }
}