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

        public ICollection<Permissions> Permissions { get; set; }

        public virtual Owners owner { get; set; }
        public virtual int id { get; set; }

    }
}