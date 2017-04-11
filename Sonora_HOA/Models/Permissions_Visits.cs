using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sonora_HOA.Models
{
    public class Permissions_Visits
    {
        public virtual int permissionsid { get; set; }
        public virtual Permissions permissions { get; set; }

        public virtual int visitsid { get; set; }
        public virtual Visits visits { get; set; }
    }
}