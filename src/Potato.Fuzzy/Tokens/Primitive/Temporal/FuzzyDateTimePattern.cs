#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;

namespace Potato.Fuzzy.Tokens.Primitive.Temporal {
    [Serializable]
    public class FuzzyDateTimePattern {
        /// <summary>
        /// If the date/time should be interpretted as a definte date/time that specifies a moment
        /// or if it should be used as a relative offset from right now.
        /// </summary>
        public TimeType Rule { get; set; }

        /// <summary>
        /// Specifies how the time is used, as a delay from now, an interval or specifying a period of time.
        /// </summary>
        public TimeModifier Modifier { get; set; }

        public int? Year { get; set; }

        public int? Month { get; set; }

        public int? Day { get; set; }

        public System.DayOfWeek? DayOfWeek { get; set; }

        public int? Hour { get; set; }

        public int? Minute { get; set; }

        public int? Second { get; set; }

        /// <summary>
        /// First Monday in August { DayOfWeek = Monday, Interval = First, Month = 8 (August) }
        /// </summary>
        public TemporalInterval TemporalInterval { get; set; }

        public FuzzyDateTimePattern() {
            TemporalInterval = TemporalInterval.Infinite;
        }

        /// <summary>
        /// Fetches a copy of DateTimePatternNLP with the current time or the parsed time.
        /// </summary>
        /// <param name="now">Optional, if null the current date/time will be used.</param>
        /// <returns></returns>
        public static FuzzyDateTimePattern Now(DateTime? now = null) {
            if (now == null) {
                now = DateTime.Now;
            }

            return new FuzzyDateTimePattern() {
                Year = now.Value.Year,
                Month = now.Value.Month,
                Day = now.Value.Day,
                Hour = now.Value.Hour,
                Minute = now.Value.Minute,
                Second = now.Value.Second
            };
        }

        /// <summary>
        /// Combines two DateTimePatternNLP's, adding together elements if we already have them.
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        private static object Combine(object o1, object o2) {
            object returnObject = null;

            if (o1 != null) {
                returnObject = o1;

                if (o2 != null && o1 is int && o2 is int) {
                    returnObject = (int) o1 + (int) o2;
                }
                else if (o2 != null && o1 is TemporalInterval && o2 is TemporalInterval) {
                    if (((TemporalInterval) o1) == TemporalInterval.Infinite && ((TemporalInterval) o2) != TemporalInterval.Infinite) {
                        returnObject = o2;
                    }
                }
            }
            else if (o2 != null) {
                returnObject = o2;
            }

            return returnObject;
        }

        private static object AnchoredCombine(object anchor, object defaultAnchor, object relative, object defaultRelative) {
            object returnObject = null;

            var usedAnchor = anchor ?? defaultAnchor;
            var usedRelative = relative ?? defaultRelative;

            if (usedAnchor is int && usedRelative is int) {
                returnObject = (int) usedAnchor + (int) usedRelative;
            }

            return returnObject;
        }

        protected static FuzzyDateTimePattern AddDefinitives(FuzzyDateTimePattern definitive1, FuzzyDateTimePattern definitive2) {
            return new FuzzyDateTimePattern {
                Rule = TimeType.Definitive,
                Year = definitive1.Year ?? definitive2.Year,
                Month = definitive1.Month ?? definitive2.Month,
                Day = definitive1.Day ?? definitive2.Day,
                Hour = definitive1.Hour ?? definitive2.Hour,
                Minute = definitive1.Minute ?? definitive2.Minute,
                Second = definitive1.Second ?? definitive2.Second,
                DayOfWeek = definitive1.DayOfWeek ?? definitive2.DayOfWeek,
                TemporalInterval = definitive1.TemporalInterval != TemporalInterval.Infinite ? definitive1.TemporalInterval : definitive2.TemporalInterval
            };
        }

        protected static FuzzyDateTimePattern AddDefinitiveRelative(FuzzyDateTimePattern definitive, FuzzyDateTimePattern relative) {
            return new FuzzyDateTimePattern {
                Rule = TimeType.Definitive,
                Year = (int?) AnchoredCombine(definitive.Year, DateTime.Now.Year, relative.Year, 0),
                Month = (int?) AnchoredCombine(definitive.Month, DateTime.Now.Month, relative.Month, 0),
                Day = (int?) AnchoredCombine(definitive.Day, DateTime.Now.Day, relative.Day, 0),
                Hour = (int?) AnchoredCombine(definitive.Hour, DateTime.Now.Hour, relative.Hour, 0),
                Minute = (int?) AnchoredCombine(definitive.Minute, DateTime.Now.Minute, relative.Minute, 0),
                Second = (int?) AnchoredCombine(definitive.Second, DateTime.Now.Second, relative.Second, 0),
                DayOfWeek = (System.DayOfWeek?) AnchoredCombine(definitive.DayOfWeek, DateTime.Now.DayOfWeek, relative.DayOfWeek, 0),
                TemporalInterval = (TemporalInterval) Combine(definitive.TemporalInterval, relative.TemporalInterval)
            };
        }

        protected static FuzzyDateTimePattern AddRelatives(FuzzyDateTimePattern relative1, FuzzyDateTimePattern relative2) {
            return new FuzzyDateTimePattern {
                Rule = TimeType.Relative,
                Year = (int?) Combine(relative1.Year, relative2.Year),
                Month = (int?) Combine(relative1.Month, relative2.Month),
                Day = (int?) Combine(relative1.Day, relative2.Day),
                Hour = (int?) Combine(relative1.Hour, relative2.Hour),
                Minute = (int?) Combine(relative1.Minute, relative2.Minute),
                Second = (int?) Combine(relative1.Second, relative2.Second),
                DayOfWeek = (System.DayOfWeek?) Combine(relative1.DayOfWeek, relative2.DayOfWeek),
                TemporalInterval = (TemporalInterval) Combine(relative1.TemporalInterval, relative2.TemporalInterval)
            };
        }

        public static FuzzyDateTimePattern operator +(FuzzyDateTimePattern one, FuzzyDateTimePattern two) {
            FuzzyDateTimePattern newDateTimePattern = null;

            if (one.Modifier == two.Modifier || one.Modifier == TimeModifier.None || two.Modifier == TimeModifier.None) {
                if (one.Rule == TimeType.Definitive && two.Rule == TimeType.Definitive) {
                    newDateTimePattern = AddDefinitives(one, two);
                }
                else if (one.Rule == TimeType.Relative && two.Rule == TimeType.Definitive) {
                    newDateTimePattern = AddDefinitiveRelative(two, one);
                }
                else if (one.Rule == TimeType.Definitive && two.Rule == TimeType.Relative) {
                    newDateTimePattern = AddDefinitiveRelative(one, two);
                }
                else if (one.Rule == TimeType.Relative && two.Rule == TimeType.Relative) {
                    newDateTimePattern = AddRelatives(one, two);
                }

                if (newDateTimePattern != null) {
                    newDateTimePattern.Modifier = one.Modifier == TimeModifier.None ? two.Modifier : one.Modifier;
                }
            }

            return newDateTimePattern;
        }

        public FuzzyDateTimePattern Negate() {
            var newDateTimePattern = new FuzzyDateTimePattern();

            if (Year != null)
                newDateTimePattern.Year = Year * -1;
            if (Month != null)
                newDateTimePattern.Month = Month * -1;
            if (Day != null)
                newDateTimePattern.Day = Day * -1;
            if (Hour != null)
                newDateTimePattern.Hour = Hour * -1;
            if (Minute != null)
                newDateTimePattern.Minute = Minute * -1;
            if (Second != null)
                newDateTimePattern.Second = Second * -1;

            return newDateTimePattern;
        }

        public static FuzzyDateTimePattern operator -(FuzzyDateTimePattern one, FuzzyDateTimePattern two) {
            FuzzyDateTimePattern newDateTimePattern = null;

            if (one.Modifier == two.Modifier || one.Modifier == TimeModifier.None || two.Modifier == TimeModifier.None) {
                //if (c1.Rule == TimeType.Definitive && c2.Rule == TimeType.Definitive) {
                //    newDateTimePattern = DateTimePatternNLP.AddDefinitives(c1, c2);
                //}
                if (one.Rule == TimeType.Relative && two.Rule == TimeType.Definitive) {
                    newDateTimePattern = AddDefinitiveRelative(two.Negate(), one);
                }
                else if (one.Rule == TimeType.Definitive && two.Rule == TimeType.Relative) {
                    newDateTimePattern = AddDefinitiveRelative(one, two.Negate());
                }
                else if (one.Rule == TimeType.Relative && two.Rule == TimeType.Relative) {
                    newDateTimePattern = AddRelatives(one, two.Negate());
                }

                if (newDateTimePattern != null) {
                    newDateTimePattern.Modifier = one.Modifier == TimeModifier.None ? two.Modifier : one.Modifier;
                }
            }

            return newDateTimePattern;
        }

        public TimeSpan? ToTimeSpan() {
            TimeSpan? ts = null;

            if (Modifier == TimeModifier.Interval) {
                ts = new TimeSpan(Day == null ? 0 : (int) Day, Hour == null ? 0 : (int) Hour, Minute == null ? 0 : (int) Minute, Second == null ? 0 : (int) Second);
            }
            else {
                var now = DateTime.Now;
                var thisDateTime = ToDateTime(now);

                if (thisDateTime.HasValue == true) {
                    var tots = thisDateTime.Value - now;

                    ts = new TimeSpan((long) Math.Ceiling(tots.Ticks / (double) TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond);
                }
            }

            return ts;
        }

        public DateTime? ToDateTime(DateTime? now = null) {
            var dtp = this;

            if (now == null) {
                now = DateTime.Now;
            }

            if (Rule == TimeType.Relative) {
                dtp = AddDefinitiveRelative(Now(now), this);
            }

            var dt = new DateTime(1, 1, 1, 0, 0, 0);

            dt = dt.AddYears((dtp.Year != null ? (int) dtp.Year : now.Value.Year) - 1);
            dt = dt.AddMonths((dtp.Month != null ? (int) dtp.Month : now.Value.Month) - 1);
            dt = dt.AddDays((dtp.Day != null ? (int) dtp.Day : now.Value.Day) - 1);
            dt = dt.AddHours(dtp.Hour != null ? (int) dtp.Hour : now.Value.Hour);
            dt = dt.AddMinutes(dtp.Minute != null ? (int) dtp.Minute : now.Value.Minute);
            dt = dt.AddSeconds(dtp.Second != null ? (int) dtp.Second : now.Value.Second);

            return dt;
        }

        public override string ToString() {
            var rs = string.Empty;

            if (Modifier == TimeModifier.Interval) {
                rs = "Every " + ToTimeSpan();
            }
            else {
                rs = ToDateTime().ToString();
            }

            return rs;
        }
    }
}