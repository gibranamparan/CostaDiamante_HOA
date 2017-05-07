using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sonora_HOA.GeneralTools
{
    public class TimePeriod
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

        public TimePeriod() { }
        public TimePeriod(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }

        public bool isValid()
        {
            return this.startDate <= this.endDate;
        }

        public override string ToString()
        {
            return string.Format("{0:MMM/dd/yyyy} - {1:MMM/dd/yyyy}",
                     this.startDate, this.endDate);
        }

        public string ToString(string dateFormat)
        {
            return string.Format("{0:" + dateFormat + "} - {1:" + dateFormat + "}",
                     this.startDate, this.endDate);
        }
        public bool isInside(DateTime date)
        {
            return date >= this.startDate && date <= this.endDate;
        }
    }
}