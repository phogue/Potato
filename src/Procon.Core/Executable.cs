using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Procon.Core
{

    public abstract class Executable<T> : ExecutableBase, INotifyPropertyChanged, IDisposable where T : MarshalByRefObject
    {
        // Public Accessors/Mutators.
        public List<String> Arguments {
            get { return mArguments; }
            set {
                if (mArguments != value) {
                    mArguments = value;
                    OnPropertyChanged(this, "Arguments");
        } } }
        // Internal Variables.
        private List<String> mArguments;
        private Dictionary<CommandAttribute, MethodInfo> mCommands;


        // Constructor.
        public Executable() {
            mCommands = new Dictionary<CommandAttribute, MethodInfo>();
            Arguments = new List<String>();
        }


        // Execute:
        // -- Loads the configuration file.
        public virtual T Execute()
        {
            Execute(CommandInitiator.Local, MasterConfig);

            return this as T;
        }
        // Execute:
        // -- Loads the specified configuration file.
        public virtual T Execute(Config config)
        {
            Execute(CommandInitiator.Local, config);

            return this as T;
        }
        // Dispose:
        // -- Allows for an optional child implementation.
        public virtual void Dispose() { }
        // WriteConfig:
        // -- Allows for an optional child implementation.
        internal virtual void WriteConfig(Config config) { }


        // Finds the commands specified in the config file and invokes them with the specified attributes.
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
                foreach (XElement xCommand in oNodes.Descendants("command"))
                {
                    // Get the name of the method / command.
                    CommandAttribute command;
                    if (Enum.IsDefined(typeof(CommandName), xCommand.Attribute("name").Value))
                        command = new CommandAttribute() { Command = (CommandName)Enum.Parse(typeof(CommandName), xCommand.Attribute("name").Value) };
                    else
                        command = new CommandAttribute() { CustomName = xCommand.Attribute("name").Value };

                    // Find the method and check it's parameters, then execute it.
                    MethodInfo   method     = null;
                    List<Object> parameters = null;
                    if (TryGetCommandMethod(command, out method) && TryGetMethodParameters(xCommand, method, out parameters))
                    {
                        initiator.Command = command.Command;
                        parameters.Insert(0, initiator);
                        method.Invoke(this, parameters.ToArray());
                    }
                }
            }
        }
        // Attempts to get a method by searching the type for the method who has the specified command attribute.
        private Boolean TryGetCommandMethod(CommandAttribute command, out MethodInfo commandMethod)
        {
            commandMethod = null;

            // Init commands if it has not been initialized yet.
            if (mCommands.Count == 0)
                foreach (MethodInfo method in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    CommandAttribute attribute = method.GetCustomAttributes(typeof(CommandAttribute), false).FirstOrDefault() as CommandAttribute;
                    if (attribute != null)
                        mCommands.Add(attribute, method);
                }

            // Find the command.
            if (mCommands.ContainsKey(command))
                commandMethod = this.mCommands[command];

            // Return whether we were successful.
            return commandMethod != null;
        }
        // Attempts to get a method's parameters by searching for the parameters whose name matches the name in the config.
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
        // Parses an enum either directly by assuming the value is a string or by assuming it's a primitive type.
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


        // Events.
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(Object sender, String property)
        {
            if (PropertyChanged != null)
                PropertyChanged(sender, new PropertyChangedEventArgs(property));
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
        private Boolean IsMethodParametersMatch(MethodInfo method, List<Object> parameters)
        {

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

        [Obsolete]
        protected virtual Object ExecutionFailed(CommandInitiator initiator, CommandAttribute command, Object[] parameters)
        {
            return null;
        }

        #endregion
    }
}
