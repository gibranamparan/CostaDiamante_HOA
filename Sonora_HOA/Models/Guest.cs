using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sonora_HOA.Models
{
    public class Guest
    {
        public int guestID { get; set; }
        public string name { get; set; }
        public string lastName { get; set; }

        public virtual ICollection<Permissions> Permissions { get; set; }

        public virtual Owners owner { get; set; }
        public string Id { get; set; }
    }
}