using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CostaDiamante_HOA.Models
{
    public class Visitor
    {
        [Key]
        [Display(Name = "Notification #")]
        public int visitorID { get; set; }

        [Display(Name = "Name")]
        public string name { get; set; }

        [Display(Name = "Last name")]
        public string lastName { get; set; }

        [Display(Name = "Name")]
        public string fullName {
            get {
                return String.Format("{0} {1}", this.name, this.lastName);
            }
        }

        [Display(Name = "Is less than 6 years old")]
        public Boolean isYounger { get; set; }

        //A visitor is for one visit
        [Display(Name = "Visit #")]
        public int visitID { get; set; }
        public virtual Visit visit { get; set; }

        /// <summary>
        /// A visitor model is created to avoid returning all collections that have to do with this model.
        /// <sumary>
        public class VMVisitor
        {
            public int visitorID { get; set; }
            public string name { get; set; }
            public string lastName { get; set; }
            public bool isYounger { get; set; }
            public int visitID { get; set; }

            public VMVisitor(Visitor visit)
            {
                this.visitorID = visit.visitorID;
                this.name = visit.name;
                this.lastName = visit.lastName;
                this.isYounger = visit.isYounger;
                this.visitID = visit.visitID;
            }
        }
    }
}