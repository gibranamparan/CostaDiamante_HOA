using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sonora_HOA.Models
{
    public class CheckInList
    {
        public int checkInListID { get; set; }

        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

        public virtual ICollection<Permissions> permissions { get; set; }

        public static SelectList generatePermissionPeriods()
        {
            List<object> objs = new List<object>();
            TimePeriodPermissions tp1 = new
                TimePeriodPermissions(DateTime.Today.Year,
                TimePeriodPermissions.YearPeriod.First);
            TimePeriodPermissions tp2 = new
                TimePeriodPermissions(DateTime.Today.Year,
                TimePeriodPermissions.YearPeriod.Second);
            if (!tp1.isInside(DateTime.Today) && DateTime.Today > tp1.StartDate)
            {
                TimePeriodPermissions tpm = new TimePeriodPermissions(tp2);
                tp2 = tp1;
                tp2.setYear(DateTime.Today.Year + 1);
                tp1 = tpm;
            }

            objs.Add(tp1);
            objs.Add(tp2);
            SelectList slPeriods = new SelectList(objs, "code", "code");
            return slPeriods;
        }

        public class TimePeriodPermissions
        {
            public enum YearPeriod { First, Second }
            private readonly DateTime DT_START1 = new DateTime(DateTime.Today.Year, 1, 1);
            private readonly DateTime DT_END1 = new DateTime(DateTime.Today.Year, 06, 30);
            private readonly DateTime DT_START2 = new DateTime(DateTime.Today.Year, 07, 1);
            private readonly DateTime DT_END2 = new DateTime(DateTime.Today.Year, 12, 31);
            private DateTime startDate, endDate;

            public DateTime StartDate { get { return this.startDate; } }
            public DateTime EndDate { get { return this.endDate; } }
            public string code
            {
                get
                {
                    return string.Format("{0:MMM/dd/yyyy} - {1:MMM/dd/yyyy}",
                        this.startDate, this.endDate);
                }
                set { }
            }

            public TimePeriodPermissions() { }
            public TimePeriodPermissions(int year, YearPeriod period)
            {
                if (period == YearPeriod.First)
                {
                    this.startDate = this.DT_START1.AddYears(year - this.DT_START1.Year);
                    this.endDate = this.DT_END1.AddYears(year - this.DT_END1.Year);
                }
                else if (period == YearPeriod.Second)
                {
                    this.startDate = this.DT_START2.AddYears(year - this.DT_START2.Year);
                    this.endDate = this.DT_END2.AddYears(year - this.DT_END2.Year);
                }
            }

            public TimePeriodPermissions(TimePeriodPermissions tp2)
            {
                this.startDate = tp2.StartDate;
                this.endDate = tp2.EndDate;
            }

            public bool isInside(DateTime date)
            {
                return date >= this.startDate && date <= this.endDate;
            }

            public void setYear(int year)
            {
                this.startDate = this.DT_START2.AddYears(year - this.DT_START2.Year);
                this.endDate = this.DT_END2.AddYears(year - this.DT_END2.Year);
            }
        }
    }
}