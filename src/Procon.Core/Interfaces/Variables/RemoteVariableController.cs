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

namespace Procon.Core.Interfaces.Variables {
    public class RemoteVariableController : VariableController {

        public RemoteVariableController Synchronize(VariableController variableController) {

            // This will overwrite the object, ignore the assigned events and won't fire the ItemAdded event..
            //this.Variables = variableController.Variables;
            this.Variables.Clear();
            foreach (Variable variable in variableController.Variables) {
                if (this.Variables.Find(x => x.Key == variable.Key) == null) {
                    this.Variables.Add(variable);
                }
                // else - silly layer listener
            }

            return this;
        }

        [Command(Event = EventName.VariableSet)]
        protected Object VariableSet(CommandInitiator initiator, string key, Object value) {
            Variable variable = null;

            if ((variable = this.Variables.Find(x => x.Key == key)) == null) {
                this.Variables.Add(
                    variable = new Variable() {
                        Key = key,
                        Value = value
                    }
                );
            }
            else {
                variable.Value = value;
            }

            return variable;
        }

        public override Object Set(CommandInitiator initiator, string key, Object value) {
            this.Layer.Request(
                new Layer.Objects.Context() {
                    ContextType = Interfaces.Layer.Objects.ContextType.All
                },
                CommandName.Set,
                EventName.None,
                key,
                value
            );

            return value;
        }


        public override Object SetA(CommandInitiator initiator, string key, Object value) {
            this.Layer.Request(
                new Layer.Objects.Context() {
                    ContextType = Interfaces.Layer.Objects.ContextType.All
                },
                CommandName.SetA,
                EventName.None,
                key,
                value
            );

            return value;
        }

        public override Object Get(CommandInitiator initiator, string key, Object defaultValue = null) {
            return this.Get<Object>(initiator, key, defaultValue);
        }

    }
}
