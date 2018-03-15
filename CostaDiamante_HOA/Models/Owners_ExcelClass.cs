using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CostaDiamante_HOA.Models
{
    public class Owners_ExcelClass
    {
        [Key]
        public int regID  { get; set; }
        public string NAME { get; set; }
        public string SECTION { get; set; }
        public string LOT { get; set; }
        public string ADDRESS { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string ZIP { get; set; }
        public string EMAIL { get; set; }
        public string PHONE { get; set; }
        public string MOBILE { get; set; }
        public string MEXNUM { get; set; }
        public string OTHER_PHONE { get; set; }

        public string username { get; set; }
        public string password { get; set; }

        public List<string> condoNames { get {
                List<string> condoNames = new List<string>();
                char separ = 'y';
                string lot = this.LOT.Replace(" ", "");
                List<string> nums = lot.Split(separ).ToList();
                foreach(var num in nums)
                    condoNames.Add($"{this.SECTION.Trim()}{int.Parse(num).ToString("00")}");

                return condoNames;
            } }
    }
}