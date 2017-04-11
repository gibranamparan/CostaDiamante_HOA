using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sonora_HOA.Models
{
    //The permissions 
    public class Permissions_Visits
    {
        //The permissions are registered for many guests to program many visits in a specific period of time
        [Key]
        [Column(Order = 0)]
        public int permissionsID { get; set; }
        public virtual Permissions permissions { get; set; }

        //The visits are a group of guests with an active permission
        [Key]
        [Column(Order = 1)]
        public int visitsID { get; set; }
        public virtual Visits visits { get; set; }
        
    }
}