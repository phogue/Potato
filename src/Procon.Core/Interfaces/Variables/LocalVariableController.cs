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
    using Procon.Core.Interfaces.Security;

    public class LocalVariableController : VariableController {

        public SecurityController Security { get; set; }

        public LocalVariableController() : base() {
            this.SetupDefaultVariables();
        }

        private void SetupDefaultVariables() {
            this.Set(CommandInitiator.Local, CommonVariableNames.TextCommandPublicPrefix, "!");
            this.Set(CommandInitiator.Local, CommonVariableNames.TextCommandProtectedPrefix, "#");
            this.Set(CommandInitiator.Local, CommonVariableNames.TextCommandPrivatePrefix, "@");
        }

        public override VariableController Execute() {

            if (this.Arguments != null) {

                // We only care about the second to last argument since the last
                // argument should be the preceeding variable keys' value.
                for (int x = 0; x < this.Arguments.Count - 1; x++) {
                    try {
                        CommonVariableNames variableName = (CommonVariableNames)Enum.Parse(typeof(CommonVariableNames), this.Arguments[x].TrimStart('-'), true);

                        Variable variable = this.Set(CommandInitiator.Local, variableName, this.Arguments[++x]) as Variable;
                        
                        // Now set this variable to readonly - it won't be able to be changed anywhere
                        // else in this interfaces lifetime
                        variable.Readonly = true;
                    }
                    catch (Exception) {
                        // Error, move on to the next argument to try and recover.
                    }
                }
            }

            return base.Execute();
        }

        protected override void AssignEvents() {
            base.AssignEvents();

            // this.Variables.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Variables_CollectionChanged);
            // ItemRemoved is never hooked.. once added a value is only ever set to null.
        }

        void Variables_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) {
                foreach (Variable item in e.NewItems) {
                    this.Layer.Request(
                        new Layer.Objects.Context() {
                            ContextType = Interfaces.Layer.Objects.ContextType.All
                        },
                        CommandName.None,
                        EventName.VariableSet,
                        item.Key,
                        item.Value
                    );

                    item.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(item_PropertyChanged);
                }
            }
        }

        private void item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            this.Layer.Request(
                new Layer.Objects.Context() {
                    ContextType = Interfaces.Layer.Objects.ContextType.All
                },
                CommandName.None,
                EventName.VariableSet,
                (sender as Variable).Key,
                (sender as Variable).Value
            );
        }

        [Command(Command = CommandName.Set)]
        public override Object Set(CommandInitiator initiator, string key, Object value) {
            if (initiator.CommandOrigin == CommandOrigin.Remote && this.Security.Can(this.Security.Account(initiator.Username), initiator.Command) == false) {
                return null;
            }

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
                if (variable.Readonly == false) {
                    variable.Value = value;
                }
            }

            return variable;
        }


        [Command(Command = CommandName.SetA)]
        public override Object SetA(CommandInitiator initiator, string key, Object value) {
            if (initiator.CommandOrigin == CommandOrigin.Remote && this.Security.Can(this.Security.Account(initiator.Username), initiator.Command) == false) {
                return null;
            }

            Variable variable = this.Set(initiator, key, value) as Variable;

            if (variable.Readonly == false) {
                if (this.VariablesArchive.Find(x => x.Key == variable.Key) == null) {
                    this.VariablesArchive.Add(variable);
                }
                else {
                    this.VariablesArchive.Find(x => x.Key == variable.Key).Value = variable.Value;
                }
            }

            return variable;
        }

        /// <summary>
        /// Gets a raw value given a key, returned as a Object
        /// </summary>
        /// <param name="initiator"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Command(Command = CommandName.Get)]
        public override Object Get(CommandInitiator initiator, string key, Object defaultValue = null) {
            if (initiator.CommandOrigin == CommandOrigin.Remote && this.Security.Can(this.Security.Account(initiator.Username), initiator.Command) == false) {
                return null;
            }

            return this.Get<Object>(initiator, key, defaultValue);
        }

    }
}
