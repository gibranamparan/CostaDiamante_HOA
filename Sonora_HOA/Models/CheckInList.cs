﻿using CostaDiamante_HOA.GeneralTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CostaDiamante_HOA.Models
{
    public class CheckInList
    {
        public int checkInListID { get; set; }

        [DisplayName("From")]
        public DateTime startDate { get; set; }
        [DisplayName("To")]
        public DateTime endDate { get; set; }
        
        //A condo has one owner
        [ForeignKey("owner")]
        [Display(Name = "Owner")]
        public string ownerID { get; set; }
        public virtual Owner owner { get; set; }


        [Display(Name ="Check in List Period")]
        public virtual TimePeriodPermissions period {
            get {
                return new TimePeriodPermissions(this);
            }
        }

        //Checkin List Status
        public CheckInListStatus status
        {
            get
            {
                if (this.checkInListID == 0)
                    return CheckInListStatus.NO_REGISTERED;
                else if (this.NumberOfUnnamedGuests < 4)
                    return CheckInListStatus.WILD_CARDS_AVAILABLE;
                else if (this.period.hasInside(DateTime.Now))
                    return CheckInListStatus.CLOSED;
                else
                    return CheckInListStatus.FINISHED;
            }
        }

        public int NumberOfRegularGuests
        {
            get
            {
                List<Permissions> res = new List<Permissions>();
                if (this.permissions != null && this.permissions.Count() > 0)
                    res = this.permissions.Where(per => !per.isWildCard).ToList();

                return res.Count();
            }
        }

        public int NumberOfUnnamedGuests
        {
            get
            {
                List<Permissions> res = new List<Permissions>();
                if (this.permissions != null && this.permissions.Count() > 0)
                    res = this.permissions.Where(per => per.isWildCard).ToList();

                return res.Count();
            }
        }

        public virtual ICollection<Permissions> permissions { get; set; }

        public void setPeriod(TimePeriodPermissions tpp)
        {
            this.startDate = tpp.startDate;
            this.endDate = tpp.endDate;
        }

        /// <summary>
        /// Generate a list of two elements, first element reprsents the recurrent semester
        /// of the year, second element the next semester.
        /// </summary>
        /// <returns>Two elements TimePeriodPermissions.</returns>
        public static List<TimePeriodPermissions> generatePermissionPeriods()
        {
            List<TimePeriodPermissions> objs = new List<TimePeriodPermissions>();
            DateTime today = DateTime.Today;

            //Temporal para arrnacar
            if (today <= new DateTime(2017, 07, 1))
                today = new DateTime(2017, 07, 1);

            //First semester of the year
            TimePeriodPermissions tp1 = new
                TimePeriodPermissions(today.Year,
                TimePeriodPermissions.YearPeriod.First);
            //Second semester of the year
            TimePeriodPermissions tp2 = new
                TimePeriodPermissions(today.Year,
                TimePeriodPermissions.YearPeriod.Second);
            //If Today is later than the first semester
            if (!tp1.hasInside(today) && today > tp1.endDate)
            {
                TimePeriodPermissions tpm = new TimePeriodPermissions(tp2);
                //Second semester becomes the first one of the next year
                tp1 = tp2;
                tp2 = tp2.getNextTimePeriod();
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

            DateTime today = DateTime.Today;
            //Temporal para arrnacar
            if (today <= new DateTime(2017, 07, 1))
                today = new DateTime(2017, 07, 1);

            var cil = cils.Take(2).ToList()
                .FirstOrDefault(list => list.period.hasInside(today));
            if (cil == null) { 
                cil = new CheckInList();
                //Sets current period
                cil.setPeriod(new TimePeriodPermissions(today.Year,
                    TimePeriodPermissions.yearPeriodOfDate(today)));
                //Associate new permissions list
                cil.permissions = new List<Permissions>();
            }
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
            if (cil == null) { 
                cil = new CheckInList();
                cil.permissions = new List<Permissions>();
            }
            return cil;
        }

        public enum CheckInListStatus
        {
            NO_REGISTERED, WILD_CARDS_AVAILABLE, CLOSED, FINISHED
        }

        /// <summary>
        /// Represents a period of the year where guest are registered to be allowed to visits
        /// a owner's condo.
        /// </summary>
        public class TimePeriodPermissions:TimePeriod
        {
            private static readonly DateTime DT_START1 = new DateTime(DateTime.Today.Year, 1, 1);
            private static readonly DateTime DT_END1 = new DateTime(DateTime.Today.Year, 06, 30);
            private static readonly DateTime DT_START2 = new DateTime(DateTime.Today.Year, 07, 1);
            private static readonly DateTime DT_END2 = new DateTime(DateTime.Today.Year, 12, 31);

            public DateTime StartDate { get { return this.startDate; } }
            public DateTime EndDate { get { return this.endDate; } }

            /// <summary>
            /// Enumerates fir First Semester, Second Semester or invalid dates.
            /// </summary>
            public enum YearPeriod { Invalid, First, Second }
            public YearPeriod yearPeriod {
                get {
                    YearPeriod result=YearPeriod.Invalid;
                    if(startDate.Year == endDate.Year) { 
                        if (startDate.Day == TimePeriodPermissions.DT_START1.Day && startDate.Month == TimePeriodPermissions.DT_START1.Month
                            && endDate.Day == TimePeriodPermissions.DT_END1.Day && endDate.Month == TimePeriodPermissions.DT_END1.Month)
                            result = YearPeriod.First;
                        if (startDate.Day == TimePeriodPermissions.DT_START2.Day && startDate.Month == TimePeriodPermissions.DT_START2.Month
                            && endDate.Day == TimePeriodPermissions.DT_END2.Day && endDate.Month == TimePeriodPermissions.DT_END2.Month)
                            result = YearPeriod.Second;
                    }
                    return result;
                }
            }

            public TimePeriodPermissions getNextTimePeriod()
            {
                YearPeriod yp;
                if(this.yearPeriod == YearPeriod.First)
                {
                    return new CheckInList.TimePeriodPermissions(this.startDate.Year, YearPeriod.Second);
                }

                if (this.yearPeriod == YearPeriod.Second)
                {
                    return new CheckInList.TimePeriodPermissions(this.startDate.Year+1, YearPeriod.First);
                }
                return new TimePeriodPermissions(this);
            }

            /// <summary>
            /// By default, an instance with current year and period will be generated
            /// </summary>
            public TimePeriodPermissions() { }

            public static YearPeriod yearPeriodOfDate(DateTime dt)
            {
                TimePeriod tp1 = new TimePeriod(TimePeriodPermissions.DT_START1, TimePeriodPermissions.DT_END1);
                TimePeriod tp2 = new TimePeriod(TimePeriodPermissions.DT_START1, TimePeriodPermissions.DT_END1);
                YearPeriod yp = YearPeriod.Invalid;
                if (tp1.hasInside(dt))
                    yp = YearPeriod.First;
                else if (tp2.hasInside(dt))
                    yp = YearPeriod.Second;

                return yp;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="year">Year where is the six months period.</param>
            /// <param name="period">Period enumeration (first or second).</param>
            public TimePeriodPermissions(int year, YearPeriod period)
            {
                if (period == YearPeriod.First)
                {
                    this.startDate = TimePeriodPermissions.DT_START1.AddYears(year - TimePeriodPermissions.DT_START1.Year);
                    this.endDate = TimePeriodPermissions.DT_END1.AddYears(year - TimePeriodPermissions.DT_END1.Year);

                    //Temporal Exception TODO: Delete when this period is finished
                    if (year == 2017)
                        this.startDate = new DateTime(2017, 02, 25);

                }
                else if (period == YearPeriod.Second)
                {
                    this.startDate = TimePeriodPermissions.DT_START2.AddYears(year - TimePeriodPermissions.DT_START2.Year);
                    this.endDate = TimePeriodPermissions.DT_END2.AddYears(year - TimePeriodPermissions.DT_END2.Year);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="year">Year where is the six months period.</param>
            /// <param name="period">Period enumeration (first or second).</param>
            public TimePeriodPermissions(DateTime dt)
            {
                YearPeriod period = yearPeriodOfDate(dt);
                if (period == YearPeriod.First)
                {
                    this.startDate = TimePeriodPermissions.DT_START1.AddYears(dt.Year - TimePeriodPermissions.DT_START1.Year);
                    this.endDate = TimePeriodPermissions.DT_END1.AddYears(dt.Year - TimePeriodPermissions.DT_END1.Year);

                    //Temporal Exception TODO: Delete when this period is finished
                    if (dt.Year == 2017)
                        this.startDate = new DateTime(2017, 02, 25);

                }
                else if (period == YearPeriod.Second)
                {
                    this.startDate = TimePeriodPermissions.DT_START2.AddYears(dt.Year - TimePeriodPermissions.DT_START2.Year);
                    this.endDate = TimePeriodPermissions.DT_END2.AddYears(dt.Year - TimePeriodPermissions.DT_END2.Year);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="tp2">An already instanced period.</param>
            public TimePeriodPermissions(TimePeriodPermissions tp2)
            {
                //Temporal
                if(tp2.startDate.Day==25 && tp2.startDate.Month == 2 && tp2.startDate.Year == 2017)
                    this.startDate = new DateTime(startDate.Year, 01, 01);
                else
                    this.startDate = tp2.StartDate;

                this.endDate = tp2.EndDate;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="cil">Another instanced CheckIInList, the period will be copied.</param>
            public TimePeriodPermissions(CheckInList cil)
            {
                this.startDate = cil.startDate;
                this.endDate = cil.endDate;
            }
            

            public void setYear(int year)
            {
                this.startDate = TimePeriodPermissions.DT_START2.AddYears(year - TimePeriodPermissions.DT_START2.Year);
                this.endDate = TimePeriodPermissions.DT_END2.AddYears(year - TimePeriodPermissions.DT_END2.Year);
            }

            public bool Equals(TimePeriodPermissions other)
            {
                return this.yearPeriod == other.yearPeriod 
                    && this.startDate.Year == other.startDate.Year;
            }
        }
    }
}