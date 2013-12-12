using System;

namespace Procon.Core.Connections.TextCommands {
    /// <summary>
    /// Handles a interval of time 
    /// </summary>
    [Serializable]
    public class TextCommandInterval {

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
        public TextCommandInterval() {
            this.IntervalType = TextCommandIntervalType.Infinite;
        }

        /// <summary>
        /// Converts this interval to a time span (if possible) for simple intervals "every 5 minutes"
        /// </summary>
        /// <returns></returns>
        public TimeSpan? ToTimeSpan() {
            return new TimeSpan(this.Day == null ? 0 : (int)this.Day, this.Hour == null ? 0 : (int)this.Hour, this.Minute == null ? 0 : (int)this.Minute, this.Second == null ? 0 : (int)this.Second);
        }

        public override string ToString() {
            return "Every " + this.ToTimeSpan();
        }
    }
}
