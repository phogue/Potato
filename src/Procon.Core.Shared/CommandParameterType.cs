using System;

namespace Procon.Core.Shared {
    public sealed class CommandParameterType {

        /// <summary>
        /// The name of parameter.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The singular type of parameter List-String- would just assign typeof(String)
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// True if this parameter requires a list of Type, or just a single Type
        /// </summary>
        public bool IsList { get; set; }

        /// <summary>
        /// Allow the conversion of the type, default is true.
        /// </summary>
        public bool IsConvertable { get; set; }

        public CommandParameterType() {
            this.IsConvertable = true;
            this.IsList = false;
        }
    }
}
