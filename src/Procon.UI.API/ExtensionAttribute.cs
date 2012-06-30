using System;

namespace Procon.UI.API
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExtensionAttribute : Attribute
    {
        // A List of extensions that must be executed before this extension is executed.
        public String[] DependsOn { get; set; }
        // A list of controls that are altered by this extension.
        public String[] Alters { get; set; }
        // A list of controls that are replaced by this extension.
        public String[] Replaces { get; set; }
    }
}
