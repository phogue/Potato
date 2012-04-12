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
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Procon.Core.Interfaces.Variables {
    using Procon.Core.Interfaces.Layer;
    using Procon.Core.Interfaces.Layer.Objects;

    public abstract class VariableController : Executable<VariableController>, IVariableController {

        /// <summary>
        /// Anything in this list is volatile and will not be saved on
        /// exit.
        /// </summary>
        public List<Variable> Variables { get; set; }

        /// <summary>
        /// Anything in this list will be saved to the config
        /// </summary>
        protected List<Variable> VariablesArchive { get; set; }

        [JsonIgnore]
        public ILayer Layer { get; set; }

        public VariableController()
            : base() {
            this.Variables = new List<Variable>();
            this.VariablesArchive = new List<Variable>();
            }



        #region Executable

        /// <summary>
        /// Begins the execution of this variable controller.
        /// Assigns events and loads the config for this file.
        /// </summary>
        public override VariableController Execute()
        {
            AssignEvents();
            return base.Execute();
        }

        /// <summary>
        /// Does nothing.  Information about this object is handled via it's parent interface.
        /// </summary>
        public override void Dispose() { }

        /// <summary>
        /// Does nothing.  Information about this object is handled via it's parent interface.
        /// </summary>
        protected override void WriteConfig(XElement xNamespace, ref FileInfo xFile) { }

        #endregion



        protected virtual void AssignEvents() {
            this.Layer.ProcessLayerEvent += new LayerGame.ProcessLayerEventHandler(Layer_ProcessLayerEvent);
        }

        private void Layer_ProcessLayerEvent(string username, Context context, CommandName command, EventName @event, object[] parameters) {
            if (context.ContextType == ContextType.All) {
                this.Execute(
                    new CommandInitiator() {
                        CommandOrigin = CommandOrigin.Remote,
                        Username = username
                    },
                    new CommandAttribute() {
                        Command = command,
                        Event = @event
                    },
                    parameters
                );
            }
        }

        /// <summary>
        /// This will first set the value, then set the value in the archived list
        /// which will be saved to the config
        /// </summary>
        /// <param name="initiator">Details of the commands origin</param>
        /// <param name="key">The unique key of the variable to set</param>
        /// <param name="value">The value of the variable</param>
        /// <returns></returns>
        public abstract Object Set(CommandInitiator initiator, string key, Object value);

        /// <summary>
        /// This will first set the value, then set the value in the archived list
        /// which will be saved to the config
        /// </summary>
        /// <param name="initiator">Details of the commands origin</param>
        /// <param name="key">The unique key of the variable to set</param>
        /// <param name="value">The value of the variable</param>
        /// <returns></returns>
        public Object Set(CommandInitiator initiator, CommonVariableNames key, Object value) {
            return this.Set(initiator, key.ToString(), value);
        }

        /// <summary>
        /// This will first set the value, then set the value in the archived list
        /// which will be saved to the config
        /// </summary>
        /// <param name="initiator">Details of the commands origin</param>
        /// <param name="key">The unique key of the variable to set</param>
        /// <param name="value">The value of the variable</param>
        /// <returns></returns>
        public abstract Object SetA(CommandInitiator initiator, string key, Object value);

        /// <summary>
        /// This will first set the value, then set the value in the archived list
        /// which will be saved to the config
        /// </summary>
        /// <param name="initiator">Details of the commands origin</param>
        /// <param name="key">The unique key of the variable to set</param>
        /// <param name="value">The value of the variable</param>
        /// <returns></returns>
        public Object SetA(CommandInitiator initiator, CommonVariableNames key, Object value) {
            return this.SetA(initiator, key.ToString(), value);
        }

        /// <summary>
        /// Gets and converts a value given a key
        /// </summary>
        /// <typeparam name="T">The type of value to return</typeparam>
        /// <param name="initiator">Details of the commands origin</param>
        /// <param name="key">The unique key of the variable to fetch</param>
        /// <returns>The converted value of the variable with the specified key</returns>
        public T Get<T>(CommandInitiator initiator, string key, T defaultValue = default(T)) {

            T result = defaultValue;
            Variable variable = null;

            if ((variable = this.Variables.Find(x => x.Key == key)) != null) {
                if (typeof(T) == typeof(Object)) {
                    result = (T)variable.Value;
                }
                else {
                    result = variable.ToValue<T>();
                }
            }

            return result;
        }

        /// <summary>
        /// Gets and converts a value given a key
        /// </summary>
        /// <typeparam name="T">The type of value to return</typeparam>
        /// <param name="initiator">Details of the commands origin</param>
        /// <param name="key">The unique key of the variable to fetch</param>
        /// <returns>The converted value of the variable with the specified key</returns>
        public T Get<T>(CommandInitiator initiator, CommonVariableNames key, T defaultValue = default(T)) {
            return this.Get<T>(initiator, key.ToString(), defaultValue);
        }

        /// <summary>
        /// Gets a raw value given a key, returned as a Object
        /// </summary>
        /// <param name="initiator">Details of the commands origin</param>
        /// <param name="key">The unique key of the variable to fetch</param>
        /// <returns>The raw object with no conversion</returns>
        public abstract Object Get(CommandInitiator initiator, string key, Object defaultValue = null);

        /// <summary>
        /// Gets a raw value given a key, returned as a Object
        /// </summary>
        /// <param name="initiator">Details of the commands origin</param>
        /// <param name="key">The unique key of the variable to fetch</param>
        /// <returns>The raw object with no conversion</returns>
        public Object Get(CommandInitiator initiator, CommonVariableNames key, Object defaultValue = null) {
            return this.Get(initiator, key.ToString());
        }
    }
}
