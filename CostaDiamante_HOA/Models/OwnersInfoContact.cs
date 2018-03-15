using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CostaDiamante_HOA.Models
{
    public class OwnersInfoContact
    {
        public OwnersInfoContact() { }
        public OwnersInfoContact(string ownerID, Owners_ExcelClass reg)
        {
            this.ownerID = ownerID;
            this.ownerName = reg.NAME;
            this.email = reg.EMAIL;
            this.phone = reg.PHONE;
            this.mobile = reg.MOBILE;
            this.mexnum = reg.MEXNUM;
            this.otherPhone = reg.OTHER_PHONE;
            this.address = reg.ADDRESS;
            this.city = reg.CITY;
            this.state = reg.STATE;
            this.zip = reg.ZIP;
        }

        [Key]
        public int ownerContactInfo { get; set; }

        [Required]
        [ForeignKey("owner")]
        public string ownerID { get; set; }
        public Owner owner { get; set; }

        [DisplayName("Contact Name")]
        public string ownerName { get; set; }

        [DisplayName("Address")]
        public string address { get; set; }
        [DisplayName("City")]
        public string city { get; set; }
        [DisplayName("State")]
        public string state { get; set; }
        [DisplayName("ZIP")]
        public string zip { get; set; }

        [DisplayName("Email")]
        public string email { get; set; }
        [DisplayName("Phone")]
        public string phone { get; set; }
        [DisplayName("Mobile")]
        public string mobile { get; set; }
        [DisplayName("Mexican Phone")]
        public string mexnum { get; set; }
        [DisplayName("Other Phone")]
        public string otherPhone { get; set; }
    }
}