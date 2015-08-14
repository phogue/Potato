#region Copyright
// Copyright 2015 Geoff Green.
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

namespace Potato.Core.Shared.Models {
    /// <summary>
    /// Handles a interval of time 
    /// </summary>
    [Serializable]
    public class TextCommandIntervalModel : CoreModel {

        /// <summary>
        /// The number of years in the interval
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// The number of months in the interval
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// The number of days in the interval
        /// </summary>
        public int? Day { get; set; }

        /// <summary>
        /// The day of the week the interval will occur on.
        /// </summary>
        public System.DayOfWeek? DayOfWeek { get; set; }

        /// <summary>
        /// The number of hours in the interval
        /// </summary>
        public int? Hour { get; set; }

        /// <summary>
        /// The number of minutes in the interval
        /// </summary>
        public int? Minute { get; set; }

        /// <summary>
        /// The number of seconds in the interval
        /// </summary>
        public int? Second { get; set; }

        /// <summary>
        /// First Monday in August { DayOfWeek = Monday, Interval = First, Month = 8 (August) }
        /// </summary>
        public TextCommandIntervalType IntervalType { get; set; }

        /// <summary>
        /// Initializes with default values
        /// </summary>
        public TextCommandIntervalModel() {
            IntervalType = TextCommandIntervalType.Infinite;
        }

        /// <summary>
        /// Converts this interval to a time span (if possible) for simple intervals "every 5 minutes"
        /// </summary>
        /// <returns></returns>
        public TimeSpan? ToTimeSpan() {
            return new TimeSpan(Day == null ? 0 : (int)Day, Hour == null ? 0 : (int)Hour, Minute == null ? 0 : (int)Minute, Second == null ? 0 : (int)Second);
        }

        public override string ToString() {
            return "Every " + ToTimeSpan();
        }
    }
}
