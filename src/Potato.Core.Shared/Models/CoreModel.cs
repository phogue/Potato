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
using System.ComponentModel;

namespace Potato.Core.Shared.Models {
    /// <summary>
    /// Base model of data for models located in Potato.Core.*
    /// </summary>
    [Serializable]
    public abstract class CoreModel : ICloneable {
        /// <summary>
        /// Event for whenever a property is modified on this executable object
        /// </summary>
        /// <remarks>I think this is only used for variables, which I would like to move specifically to
        /// the variables controlle. There is no need for other variables to use this functionality.</remarks>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="property"></param>
        protected void OnPropertyChanged(Object sender, String property) {
            var handler = this.PropertyChanged;
            if (handler != null) {
                handler(sender, new PropertyChangedEventArgs(property));
            }
        }

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
