using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sonora_HOA.Models
{
    public class Condo
    {
        [Key]
        public int number { get; set; }

        public string name { get; set; }


        //A condo has one owner
        public string Id { get; set; }
        public virtual Owners owner { get; set; }

        //A condo can be visited with many permissions
        public virtual ICollection<Permissions> Permissions { get; set; }

    }
}