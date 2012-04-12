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
using System.Web;
using Newtonsoft.Json;

namespace Procon.Core.Interfaces.Connections.Plugins.Variables {

    [Serializable]
    public abstract class Variable {

        /// <summary>
        /// A group name to group this variable by
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// A friendly name for the variable
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of the variable
        /// </summary>
        //public Object Value { get; set; }

        /// <summary>
        /// If this variable is visible.  This is helper attribute to guide
        /// UI's to show or hide the variable.
        /// </summary>
        public bool Visible { get; set; }

        public Variable() {
            this.Group = String.Empty;
            this.Name = String.Empty;

            this.Visible = true;
        }

        #region JSON Serializer

        /// <summary>
        /// Converts a html encoded json string into a variable object
        /// 
        /// This method is predominantly used by config saving/loading.
        /// </summary>
        /// <param name="json"></param>
        public static Variable FromJson(string json) {
            Variable variable = JsonConvert.DeserializeObject<Variable>(
                Uri.UnescapeDataString(json),
                new JsonSerializerSettings() {
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );

            //this.Group = variable.Group;
            //this.Name = variable.Name;
            //this.Value = variable.Value;
            //this.Visible = variable.Visible;

            return variable;
        }

        /// <summary>
        /// Converts this variable object into a html encoded string
        /// 
        /// This method is predominantly used by config saving/loading.
        /// </summary>
        /// <param name="json"></param>
        public string ToJson() {
            return Uri.EscapeDataString(
                JsonConvert.SerializeObject(
                    this,
                    Formatting.None,
                    new JsonSerializerSettings() {
                        TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects
                    }
                )
            );
        }

        #endregion
    }
}