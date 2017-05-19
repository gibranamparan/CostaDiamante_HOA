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
        public const int WILD_CARDS_LIMIT = 4;

        [Key]
        public int permissions_visitsID { get; set; }

        //The permissions are registered for many guests to program many visits in a specific period of time
        public int? permissionsID { get; set; }
        public virtual Permissions permissions { get; set; }

        public string guestFullName { get; set; }
        public bool isWildCard { get; set; }

        //The visits are a group of guests with an active permission
        [Required]
        public int visitsID { get; set; }
        public virtual Visits visits { get; set; }
    }
}