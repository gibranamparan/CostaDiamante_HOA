using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sonora_HOA.Models
{
    public class Permissions_Visits
    {
        
        public virtual Permissions permissions { get; set; }
        public int permissionsID { get; set; }

        public virtual Visits visits { get; set; }
        public int visitsID { get; set; }
        
    }
}