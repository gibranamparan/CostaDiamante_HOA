using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        
        //A condo has one owner
        [ForeignKey("owner")]
        [Display(Name = "Owner")]
        public string ownerID { get; set; }
        public virtual Owner owner { get; set; }

        [Display(Name ="Year Period")]
        public TimePeriodPermissions period { get { return new TimePeriodPermissions(this); } }

        public virtual ICollection<Permissions> permissions { get; set; }

        public static List<TimePeriodPermissions> generatePermissionPeriods()
        {
            List<TimePeriodPermissions> objs = new List<TimePeriodPermissions>();
            //First semester of the year
            TimePeriodPermissions tp1 = new
                TimePeriodPermissions(DateTime.Today.Year,
                TimePeriodPermissions.YearPeriod.First);
            //Second semester of the year
            TimePeriodPermissions tp2 = new
                TimePeriodPermissions(DateTime.Today.Year,
                TimePeriodPermissions.YearPeriod.Second);
            //If Today is later than the first semester
            if (!tp1.isInside(DateTime.Today) && DateTime.Today > tp1.StartDate)
            {
                TimePeriodPermissions tpm = new TimePeriodPermissions(tp2);
                //Second semester becomes the first one of the next year
                tp2 = tp1;
                tp2.setYear(DateTime.Today.Year + 1);
                //First semester of this year becomes the second semester of this one
                tp1 = tpm;
            }

            objs.Add(tp1);
            objs.Add(tp2);
            objs = objs.OrderBy(tp => tp.StartDate).ToList();

            return objs;
        }

        /// <summary>
        /// Look for the current check in list registered to create new visits. If no one is found, a new empty instance
        /// is returned with primary key 0.
        /// </summary>
        /// <returns>The current CheckInList instance for this Owner.</returns>
        internal static CheckInList getCurrentCheckInList(string ownerID, ApplicationDbContext db)
        {
            Owner owner = db.Owners.Find(ownerID);
            var cils = owner.checkInListHistory
                .OrderByDescending(list => list.startDate);
            var cil = cils.Take(2).ToList()
                .FirstOrDefault(list => list.period.isInside(DateTime.Today));
            if (cil == null)
                cil = new CheckInList();
            return cil;
        }

        /// <summary>
        /// Look for a check in list by a PeriodTimePermission Instance
        /// </summary>
        /// <returns>The result CheckInList instance for this Owner.</returns>
        internal static CheckInList findCheckInListByPeriod(string ownerID,CheckInList.TimePeriodPermissions tpp, ApplicationDbContext db)
        {
            Owner owner = db.Owners.Find(ownerID);
            var checkInLists = owner.checkInListHistory.OrderByDescending(list => list.startDate)
                .Take(3).ToList();
            var cil = checkInLists.FirstOrDefault(list => list.period.Equals(tpp));
            if (cil == null)
                cil = new CheckInList();
            return cil;
        }

        public class TimePeriodPermissions:IEquatable<TimePeriodPermissions>
        {
            public enum YearPeriod { Invalid, First, Second }
            private readonly DateTime DT_START1 = new DateTime(DateTime.Today.Year, 1, 1);
            private readonly DateTime DT_END1 = new DateTime(DateTime.Today.Year, 06, 30);
            private readonly DateTime DT_START2 = new DateTime(DateTime.Today.Year, 07, 1);
            private readonly DateTime DT_END2 = new DateTime(DateTime.Today.Year, 12, 31);
            private DateTime startDate, endDate;
            public YearPeriod yearPeriod {
                get {
                    YearPeriod result=YearPeriod.Invalid;
                    if(startDate.Year == endDate.Year) { 
                        if (startDate.Day == this.DT_START1.Day && startDate.Month == this.DT_START1.Month
                            && endDate.Day == this.DT_END1.Day && endDate.Month == this.DT_END1.Month)
                            result = YearPeriod.First;
                        if (startDate.Day == this.DT_START2.Day && startDate.Month == this.DT_START2.Month
                            && endDate.Day == this.DT_END2.Day && endDate.Month == this.DT_END2.Month)
                            result = YearPeriod.Second;
                    }
                    return result;
                }
            }

            public DateTime StartDate { get { return this.startDate; } }
            public DateTime EndDate { get { return this.endDate; } }

            public override string ToString()
            {
                return string.Format("{0:MMM/dd/yyyy} - {1:MMM/dd/yyyy}",
                         this.startDate, this.endDate);
            }

            public string ToString(string dateFormat)
            {
                return string.Format("{0:"+ dateFormat + "} - {1:"+ dateFormat + "}",
                         this.startDate, this.endDate);
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

            public TimePeriodPermissions(CheckInList cil)
            {
                this.startDate = cil.startDate;
                this.endDate = cil.endDate;
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

            public bool Equals(TimePeriodPermissions other)
            {
                return this.yearPeriod == other.yearPeriod 
                    && this.startDate.Year == other.startDate.Year;
            }
        }
    }
}