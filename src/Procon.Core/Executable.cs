// Copyright 2011 Geoffrey 'Phogue' Green
// Modified by Cameron 'Imisnew2' Gunnin
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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Procon.Core {
    using Procon.Core.Utils;

    public abstract class Executable<T> : ExecutableBase, INotifyPropertyChanged, IDisposable where T : MarshalByRefObject
    {
        // Private Objects
        private Dictionary<CommandAttribute, MethodInfo> Commands;

        // Public Objects
        public  List<String> Arguments {
            get { return mArguments; }
            set {
                if (mArguments != value) {
                    mArguments = value;
                    OnPropertyChanged(this, "Arguments");
                }
            }
        }
        private List<String> mArguments;

        // Default Initialization
        public Executable() {
            Commands  = new Dictionary<CommandAttribute, MethodInfo>();
            Arguments = new List<String>();
        }
        ~Executable() {
            SaveConfig();
        }



        #region INotifyPropertyChanged

        /// <summary>
        /// Is fired whenever a property in this instance changes. Passes on the name of
        /// the property that was changed along with the object it was changed in.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Fires the PropertyChanged event using the specified sender and property name.
        /// </summary>
        protected void OnPropertyChanged(object sender, String property)
        {
            if (PropertyChanged != null)
                PropertyChanged(sender, new PropertyChangedEventArgs(property));
        }

        #endregion INotifyPropertyChanged
        #region IDisposable

        /// <summary>
        /// Allows children classes to override this method to cleanup relevant information.
        /// </summary>
        public virtual void Dispose() { }

        #endregion



        /// <summary>
        /// Executes the commands specified in the master config file and returns a reference itself.
        /// </summary>
        public virtual T Execute()
        {
            Execute(CommandInitiator.Local, MasterConfig);

            return this as T;
        }

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public virtual T Execute(Config config)
        {
            Execute(CommandInitiator.Local, config);

            return this as T;
        }

        /// <summary>
        /// Allows children classes to override this method to save relevant information.
        /// </summary>
        protected virtual void WriteConfig(XElement xNamespace, ref FileInfo xFile) { }



        /// <summary>
        /// Execute all the commands located in the config for this object type.
        /// </summary>
        private void Execute(CommandInitiator initiator, Config config)
        {
            if (config != null && config.Root != null)
            {
                // Allocate space for this objects type and the config's nodes.
                Type oType = GetType();
                var oNodes = config.Root.Elements();

                // Drill down in the config to this object's type.
                foreach (String oName in oType.FullName.Split('`').First().Split('.').Skip(1))
                    oNodes = oNodes.DescendantsAndSelf(oName.ToLower());

                // For each of the commands for this object...
                foreach (XElement XCommand in oNodes.Descendants("command"))
                {
                    // Get the name of the method / command.
                    CommandAttribute command;
                    if (Enum.IsDefined(typeof(CommandName), XCommand.Attribute("name").Value))
                        command = new CommandAttribute() { Command = (CommandName)Enum.Parse(typeof(CommandName), XCommand.Attribute("name").Value) };
                    else
                        command = new CommandAttribute() { CustomName = XCommand.Attribute("name").Value };

                    // Find the method and check it's parameters, then execute it.
                    MethodInfo method = null;
                    List<Object> parameters = null;
                    if (TryGetCommandMethod(command, out method) && TryGetMethodParameters(XCommand, method, out parameters))
                    {
                        initiator.Command = command.Command;
                        parameters.Insert(0, initiator);

                        method.Invoke(this, parameters.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to get a method of this class (or it's children) by specifying a command,
        /// reflecting across all methods of this class, then determining if this class has the
        /// method that represents the command specified.
        /// </summary>
        private Boolean TryGetCommandMethod(CommandAttribute command, out MethodInfo commandMethod)
        {
            // Default to failed.
            commandMethod = null;

            // Init commands if it has not been initialized yet.
            if (Commands.Count == 0)
                foreach (MethodInfo method in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    CommandAttribute attribute = method.GetCustomAttributes(typeof(CommandAttribute), false).FirstOrDefault() as CommandAttribute;
                    if (attribute != null)
                        Commands.Add(attribute, method);
                }

            // Find the command.
            if (Commands.ContainsKey(command))
                commandMethod = this.Commands[command];

            // Return whether we were successful.
            return commandMethod != null;
        }

        /// <summary>
        /// Attempts to parse an object into an enum by either parsing the value directly as a string
        /// or first getting the enum the value represents as a string then parsing it.
        /// </summary>
        private Object ParseEnumValue(Type type, Object value) {

            Object enumValue = null;

            if (type.IsEnum)
                if (value is string) {
                    if (Enum.IsDefined(type, value) == true)
                        enumValue = Enum.Parse(type, (string)value);
                }   
                else {
                    int val = (int)System.Convert.ChangeType(value.ToString(), typeof(int));
                    if (Enum.IsDefined(type, val) == true)
                        enumValue = Enum.Parse(type, Enum.GetName(type, value));
                }

            return enumValue;
        }

        /// <summary>
        /// Attempts to match the parameters and values from an xml object to a method's parameters.
        /// Sets "parameters" to a list of parameters in the order that they should be passed to the method.
        /// </summary>
        private Boolean TryGetMethodParameters(XElement xCommand, MethodInfo method, out List<Object> parameters)
        {
            // Get all the parameters for this method, minus the 'this' parameter because it's an instance method.
            List<ParameterInfo> parameterInfoList = method.GetParameters().Skip(1).ToList();
            parameters = null;
            if (parameterInfoList.Count > 0)
                parameters = new List<Object>();

            // If there are parameters, then loop through each parameter the method is expecting.
            if (parameters != null)
                foreach (ParameterInfo parameterInfo in parameterInfoList)
                {
                    // Check to see if the parameter matches something in the xml object.
                    XElement xParameter = xCommand.Elements().FirstOrDefault(x => String.Compare(x.Name.LocalName, parameterInfo.Name, true) == 0);
                    if (xParameter != null)
                        try
                        {
                            // Parse the parameter from a string into the correct type.
                            Object nParameter = ParseEnumValue(parameterInfo.ParameterType, xParameter.Value);
                            if (nParameter == null)
                                nParameter = System.Convert.ChangeType(xParameter.Value, parameterInfo.ParameterType);
                            parameters.Add(nParameter);
                        }
                        catch (Exception) { break; }
                    else { break; }
                }

            // Return whether we were successful.
            return (parameters == null) || (parameters.Count == parameterInfoList.Count);
        }

        /// <summary>
        /// Checks to see whether the parameters passed in are valid for the specified method.
        /// Attempts to convert the parameters into their correct type to see if they match.
        /// </summary>
        private Boolean IsMethodParametersMatch(MethodInfo method, List<Object> parameters) {

            bool isMatch = true;

            List<ParameterInfo> parameterInfo = method.GetParameters().Skip(1).ToList();

            if (parameterInfo.Count == parameters.Count) {
                for (int offset = 0; offset < parameterInfo.Count && offset < parameters.Count && isMatch == true; offset++) {
                    if (parameters[offset] != null && parameterInfo[offset].ParameterType.IsAssignableFrom(parameters[offset].GetType()) == false) {
                    //if (parameterInfo[offset].ParameterType != parameters[offset].GetType()) {

                        try {
                            if (parameterInfo[offset].ParameterType.IsEnum == true) {
                                parameters[offset] = this.ParseEnumValue(parameterInfo[offset].ParameterType, parameters[offset]);
                            }
                            else {
                                parameters[offset] = System.Convert.ChangeType(parameters[offset].ToString(), parameterInfo[offset].ParameterType);
                            }
                        }
                        catch (Exception) {
                            isMatch = false;
                        }
                    }
                }
            }
            else {
                isMatch = false;
            }

            return isMatch;
        }

        /// <summary>
        /// Creates a configuration file for this object, using children's overload of
        /// WriteConfig to save all the relevant information about this object.
        /// </summary>
        private void SaveConfig()
        {
            Config   mConfig = new Config().Generate(GetType());
            FileInfo mFile   = new FileInfo(Path.Combine(Defines.CONFIGS_DIRECTORY, String.Format("{0}.xml", GetType().Namespace)));
            WriteConfig(mConfig.Root, ref mFile);
            mConfig.Save(mFile);
        }



        #region [Obsolete] Eventually I would like to remove this..

        [Obsolete]
        protected Object Execute(CommandInitiator initiator, CommandAttribute command, params Object[] parameters)
        {

            Object result = null;
            MethodInfo method = null;

            List<Object> parameterList = new List<Object>(parameters);

            if (this.TryGetCommandMethod(command, out method) == true && this.IsMethodParametersMatch(method, parameterList) == true)
            {
                initiator.Command = command.Command;
                parameterList.Insert(0, initiator);

                result = method.Invoke(this, parameterList.ToArray());
            }
            else
            {
                result = this.ExecutionFailed(initiator, command, parameters);
            }

            return result;
        }

        [Obsolete]
        protected virtual Object ExecutionFailed(CommandInitiator initiator, CommandAttribute command, Object[] parameters)
        {
            return null;
        }

        #endregion
    }
}
