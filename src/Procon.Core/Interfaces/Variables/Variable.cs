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
using System.ComponentModel;

namespace Procon.Core.Interfaces.Variables {
    public class Variable : INotifyPropertyChanged {

        /// <summary>
        /// The unique key of the variable 
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The value of the variable
        /// </summary>
        private Object m_value { get; set; }
        public Object Value {
            get {
                return this.m_value;
            }
            set {
                this.m_value = value;

                this.OnPropertyChanged(this, "Value");
            }
        
        }

        /// <summary>
        /// Tells VariableController this variable is readonly and should not be written to
        /// </summary>
        public bool Readonly { get; set; }

        public Variable() {
            this.Key = String.Empty;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(object sender, string propertyName) {
            if (this.PropertyChanged != null) {
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        /// <summary>
        /// Converts this variables value to a specified type
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="default">The default value to use if a conversion is not possible</param>
        /// <returns>The converted or default value</returns>
        public T ToValue<T>(T @default = default(T)) {
            T result = @default;

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            string value = this.Value.ToString();

            if (value.Length > 0 && converter.CanConvertFrom(typeof(string)) == true) {
                result = (T)converter.ConvertFrom(value);
            }
            else {
                result = @default;
            }

            return result;
        }
    }
}
