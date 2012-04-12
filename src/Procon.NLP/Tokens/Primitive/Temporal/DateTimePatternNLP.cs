// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Procon.NLP.Tokens.Primitive.Temporal {

    [Serializable]
    public class DateTimePatternNLP {

        public TimeType Rule { get; set; }

        public TimeModifier Modifier { get; set; }

        public static DateTimePatternNLP Now(DateTime? now = null) {
            if (now == null) {
                now = DateTime.Now;
            }

            return new DateTimePatternNLP() {
                Year = now.Value.Year,
                Month = now.Value.Month,
                Day = now.Value.Day,
                Hour = now.Value.Hour,
                Minute = now.Value.Minute,
                Second = now.Value.Second
            };
        }

        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }
        public System.DayOfWeek? DayOfWeek { get; set; }

        public int? Hour { get; set; }
        public int? Minute { get; set; }
        public int? Second { get; set; }

        // First Monday in August { DayOfWeek = Monday, Interval = First, Month = 8 (August) }
        public TemporalInterval TemporalInterval { get; set; }

        public DateTimePatternNLP() {
            this.TemporalInterval = TemporalInterval.Infinite;
        }

        //private bool MatchesPattern(PropertyInfo selfProperty, DateTime dateTime) {

        //    bool matches = true;

        //    if (selfProperty.GetValue(this, null) != null) {
        //        if (selfProperty.GetValue(this, null) != typeof(DateTime).GetProperty(selfProperty.Name).GetValue(dateTime, null)) {
        //            matches = false;
        //        }
        //    }

        //    return matches;
        //}

        private static object Combine(object o1, object o2) {

            object returnObject = null;

            if (o1 != null) {
                returnObject = o1;

                if (o2 != null && o1 is int && o2 is int) {
                    returnObject = (int)o1 + (int)o2;
                }
                else if (o2 != null && o1 is TemporalInterval && o2 is TemporalInterval) {

                    if (((TemporalInterval)o1) == TemporalInterval.Infinite && ((TemporalInterval)o2) != TemporalInterval.Infinite) {
                        returnObject = o2;
                    }
                    // returnObject = o1; (already equals)
                }
            }
            else if (o2 != null) { // o1 == null
                returnObject = o2;
            }
            //else {
            //    returnObject = null;
            //}

            return returnObject;
        }

        private static object AnchoredCombine(object anchor, object defaultAnchor, object relative, object defaultRelative) {
            object returnObject = null;

            object usedAnchor = anchor == null ? defaultAnchor : anchor;
            object usedRelative = relative == null ? defaultRelative : relative;

            if (usedAnchor is int && usedRelative is int) {
                returnObject = (int)usedAnchor + (int)usedRelative;
            }

            return returnObject;
        }

        protected static DateTimePatternNLP AddDefinitives(DateTimePatternNLP definitive1, DateTimePatternNLP definitive2) {
            DateTimePatternNLP newDateTimePattern = new DateTimePatternNLP() {
                Rule = TimeType.Definitive
            };

            newDateTimePattern.Year = (int?)(definitive1.Year != null ? definitive1.Year : definitive2.Year);
            newDateTimePattern.Month = (int?)(definitive1.Month != null ? definitive1.Month : definitive2.Month);
            newDateTimePattern.Day = (int?)(definitive1.Day != null ? definitive1.Day : definitive2.Day);
            newDateTimePattern.Hour = (int?)(definitive1.Hour != null ? definitive1.Hour : definitive2.Hour);
            newDateTimePattern.Minute = (int?)(definitive1.Minute != null ? definitive1.Minute : definitive2.Minute);
            newDateTimePattern.Second = (int?)(definitive1.Second != null ? definitive1.Second : definitive2.Second);
            newDateTimePattern.DayOfWeek = (System.DayOfWeek?)(definitive1.DayOfWeek != null ? definitive1.DayOfWeek : definitive2.DayOfWeek);
            newDateTimePattern.TemporalInterval = (TemporalInterval)(definitive1.TemporalInterval != TemporalInterval.Infinite ? definitive1.TemporalInterval : definitive2.TemporalInterval);

            return newDateTimePattern;
        }

        protected static DateTimePatternNLP AddDefinitiveRelative(DateTimePatternNLP definitive, DateTimePatternNLP relative) {
            DateTimePatternNLP newDateTimePattern = new DateTimePatternNLP() {
                Rule = TimeType.Definitive
            };
            // 25 when adding over something.
            newDateTimePattern.Year = (int?)DateTimePatternNLP.AnchoredCombine(definitive.Year, DateTime.Now.Year, relative.Year, 0);
            newDateTimePattern.Month = (int?)DateTimePatternNLP.AnchoredCombine(definitive.Month, DateTime.Now.Month, relative.Month, 0);
            newDateTimePattern.Day = (int?)DateTimePatternNLP.AnchoredCombine(definitive.Day, DateTime.Now.Day, relative.Day, 0);
            newDateTimePattern.Hour = (int?)DateTimePatternNLP.AnchoredCombine(definitive.Hour, DateTime.Now.Hour, relative.Hour, 0);
            newDateTimePattern.Minute = (int?)DateTimePatternNLP.AnchoredCombine(definitive.Minute, DateTime.Now.Minute, relative.Minute, 0);
            newDateTimePattern.Second = (int?)DateTimePatternNLP.AnchoredCombine(definitive.Second, DateTime.Now.Second, relative.Second, 0);
            newDateTimePattern.DayOfWeek = (System.DayOfWeek?)DateTimePatternNLP.AnchoredCombine(definitive.DayOfWeek, DateTime.Now.DayOfWeek, relative.DayOfWeek, 0);
            newDateTimePattern.TemporalInterval = (TemporalInterval)DateTimePatternNLP.Combine(definitive.TemporalInterval, relative.TemporalInterval);

            return newDateTimePattern;
        }

        protected static DateTimePatternNLP AddRelatives(DateTimePatternNLP relative1, DateTimePatternNLP relative2) {
            DateTimePatternNLP newDateTimePattern = new DateTimePatternNLP() {
                Rule = TimeType.Relative
            };

            newDateTimePattern.Year = (int?)DateTimePatternNLP.Combine(relative1.Year, relative2.Year);
            newDateTimePattern.Month = (int?)DateTimePatternNLP.Combine(relative1.Month, relative2.Month);
            newDateTimePattern.Day = (int?)DateTimePatternNLP.Combine(relative1.Day, relative2.Day);
            newDateTimePattern.Hour = (int?)DateTimePatternNLP.Combine(relative1.Hour, relative2.Hour);
            newDateTimePattern.Minute = (int?)DateTimePatternNLP.Combine(relative1.Minute, relative2.Minute);
            newDateTimePattern.Second = (int?)DateTimePatternNLP.Combine(relative1.Second, relative2.Second);
            newDateTimePattern.DayOfWeek = (System.DayOfWeek?)DateTimePatternNLP.Combine(relative1.DayOfWeek, relative2.DayOfWeek);
            newDateTimePattern.TemporalInterval = (TemporalInterval)DateTimePatternNLP.Combine(relative1.TemporalInterval, relative2.TemporalInterval);

            return newDateTimePattern;
        }

        public static DateTimePatternNLP operator +(DateTimePatternNLP c1, DateTimePatternNLP c2) {
            DateTimePatternNLP newDateTimePattern = null;

            if (c1.Modifier == c2.Modifier || c1.Modifier == TimeModifier.None || c2.Modifier == TimeModifier.None) {
                if (c1.Rule == TimeType.Definitive && c2.Rule == TimeType.Definitive) {
                    newDateTimePattern = DateTimePatternNLP.AddDefinitives(c1, c2);
                }
                else if (c1.Rule == TimeType.Relative && c2.Rule == TimeType.Definitive) {
                    newDateTimePattern = DateTimePatternNLP.AddDefinitiveRelative(c2, c1);
                }
                else if (c1.Rule == TimeType.Definitive && c2.Rule == TimeType.Relative) {
                    newDateTimePattern = DateTimePatternNLP.AddDefinitiveRelative(c1, c2);
                }
                else if (c1.Rule == TimeType.Relative && c2.Rule == TimeType.Relative) {
                    newDateTimePattern = DateTimePatternNLP.AddRelatives(c1, c2);
                }

                newDateTimePattern.Modifier = c1.Modifier == TimeModifier.None ? c2.Modifier : c1.Modifier;
            }

            return newDateTimePattern;
        }

        public DateTimePatternNLP Negate() {

            DateTimePatternNLP newDateTimePattern = new DateTimePatternNLP();

            if (this.Year != null) newDateTimePattern.Year = this.Year * -1;
            if (this.Month != null) newDateTimePattern.Month = this.Month * -1;
            if (this.Day != null) newDateTimePattern.Day = this.Day * -1;
            if (this.Hour != null) newDateTimePattern.Hour = this.Hour * -1;
            if (this.Minute != null) newDateTimePattern.Minute = this.Minute * -1;
            if (this.Second != null) newDateTimePattern.Second = this.Second * -1;

            return newDateTimePattern;
        }

        public static DateTimePatternNLP operator -(DateTimePatternNLP c1, DateTimePatternNLP c2) {
            DateTimePatternNLP newDateTimePattern = null;

            if (c1.Modifier == c2.Modifier || c1.Modifier == TimeModifier.None || c2.Modifier == TimeModifier.None) {
                //if (c1.Rule == TimeType.Definitive && c2.Rule == TimeType.Definitive) {
                //    newDateTimePattern = DateTimePatternNLP.AddDefinitives(c1, c2);
                //}
                if (c1.Rule == TimeType.Relative && c2.Rule == TimeType.Definitive) {
                    newDateTimePattern = DateTimePatternNLP.AddDefinitiveRelative(c2.Negate(), c1);
                }
                else if (c1.Rule == TimeType.Definitive && c2.Rule == TimeType.Relative) {
                    newDateTimePattern = DateTimePatternNLP.AddDefinitiveRelative(c1, c2.Negate());
                }
                else if (c1.Rule == TimeType.Relative && c2.Rule == TimeType.Relative) {
                    newDateTimePattern = DateTimePatternNLP.AddRelatives(c1, c2.Negate());
                }

                newDateTimePattern.Modifier = c1.Modifier == TimeModifier.None ? c2.Modifier : c1.Modifier;
            }

            return newDateTimePattern;
        }

        public TimeSpan? ToTimeSpan() {
            TimeSpan? ts = null;

            if (this.Modifier == TimeModifier.Interval) {
                ts = new TimeSpan(
                    this.Day == null ? 0 : (int)this.Day,
                    this.Hour == null ? 0 : (int)this.Hour,
                    this.Minute == null ? 0 : (int)this.Minute,
                    this.Second == null ? 0 : (int)this.Second
                );
            }
            else {
                DateTime now = DateTime.Now;
                TimeSpan tots = this.ToDateTime(now).Value - now;

                ts = new TimeSpan(((tots.Ticks + TimeSpan.TicksPerSecond - 1) / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond);

                
               // ts = this.ToDateTime(now).Value - now;
            }

           // TimeSpan.FromTicks(ts.Value.Ticks)

            return ts;
        }

        public DateTime? ToDateTime(DateTime? now = null) {

            DateTime dt;// = DateTime.Now;
            DateTimePatternNLP dtp = this;

            if (now == null) {
                now = DateTime.Now;
            }

            if (this.Rule == TimeType.Relative) {
                dtp = DateTimePatternNLP.AddDefinitiveRelative(DateTimePatternNLP.Now(now), this);
            }

            dt = new DateTime(
                1,
                1,
                1,
                0,
                0,
                0
            );

            dt = dt.AddYears((dtp.Year != null ? (int)dtp.Year : now.Value.Year) - 1);
            dt = dt.AddMonths((dtp.Month != null ? (int)dtp.Month : now.Value.Month) - 1);
            dt = dt.AddDays((dtp.Day != null ? (int)dtp.Day : now.Value.Day) - 1);
            dt = dt.AddHours(dtp.Hour != null ? (int)dtp.Hour : now.Value.Hour);
            dt = dt.AddMinutes(dtp.Minute != null ? (int)dtp.Minute : now.Value.Minute);
            dt = dt.AddSeconds(dtp.Second != null ? (int)dtp.Second : now.Value.Second);

            return dt;
        }

        /*
        public static DateTime operator +(DateTime dt, DateTimePatternNLP p) {

            // return (new DateTimePatternNLP(DateTime.Now) + p).ToDateTime();

            DateTime newDateTime = new DateTime(
                (p.Year != null ? (int)p.Year : dt.Year),
                (p.Month != null ? (int)p.Month : dt.Month),
                (p.Day != null ? (int)p.Day : dt.Day),
                (p.Hour != null ? (int)p.Hour : dt.Hour),
                (p.Minute != null ? (int)p.Minute : dt.Minute),
                (p.Second != null ? (int)p.Second : dt.Second)
            );
            dt = dt + (newDateTime - dt);

            //dt = dt.AddYears(p.Year != null ? dt.Year - (int)p.Year : 0);
            //dt = dt.AddMonths(p.Month != null ? dt.Month - (int)p.Month : 0);
            //dt = dt.AddDays(p.Day != null ? dt.Day - (int)p.Day : 0);
            //dt = dt.AddHours(p.Hour != null ? (int)p.Hour : 0);
            //dt = dt.AddMinutes(p.Minute != null ? (int)p.Minute : 0);
            //dt = dt.AddSeconds(p.Second != null ? (int)p.Second : 0);

            return dt;
        }
        */
        public bool Matches(DateTime dateTime) {

            bool matches = true;

            if (this.Year != null) {
                if (this.Year is int && (int)this.Year != dateTime.Year) {
                    matches = false;
                }
            }

            if (this.Month != null) {
                if (this.Month is int && (int)this.Month != dateTime.Month) {
                    matches = false;
                }
            }

            if (this.Day != null) {
                if (this.Day is int && (int)this.Day != dateTime.Day) {
                    matches = false;
                }
            }

            if (this.Hour != null) {
                if (this.Hour is int && (int)this.Hour != dateTime.Hour) {
                    matches = false;
                }
            }

            if (this.Minute != null) {
                if (this.Minute is int && (int)this.Minute != dateTime.Minute) {
                    matches = false;
                }
            }

            if (this.Second != null) {
                if (this.Second is int && (int)this.Second != dateTime.Second) {
                    matches = false;
                }
            }

            if (this.DayOfWeek != null) {
                if (this.DayOfWeek is DayOfWeek && (DayOfWeek)this.DayOfWeek != dateTime.DayOfWeek) {
                    matches = false;
                }
            }

            // TO DO: Check on basis of "The First", "Second", "Infinite" etc.

            //foreach (PropertyInfo property in typeof(DateTimePatternNLP).GetProperties()) {
            //    matches = this.MatchesPattern(property, dateTime);

            //    if (matches == false) {
            //        break;
            //    }
            //}

            return matches;
        }

        public override string ToString() {
            string rs = String.Empty;

            if (this.Modifier == TimeModifier.Interval) {
                rs = "Every " + this.ToTimeSpan();
            }
            else {
                rs = this.ToDateTime().ToString();
            }

            return rs;
        }
    }
}
