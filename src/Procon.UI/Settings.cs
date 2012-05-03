using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Procon.Core;
using Procon.UI.API.Utils;
using Procon.UI.API.ViewModels;

namespace Procon.UI
{
    public static class Settings
    {
        // Stores all the settings.
        static InfinityDictionary<String, Object> mData = InstanceViewModel.PublicProperties["Settings"];

        // Getter/Setter for the settings file.
        public static T    Get<T>(String key, T fallback = default(T))
        {
            if (mData.ContainsKey(key))
                return (T)mData[key].Value;
            return fallback;
        }
        public static void Set   (String key, Object value)
        {
            mData[key].Value = value;
            
        }

        // Lodas/Saves all the settings.
        public static void Load()
        {
            // Load the settings from the config file.
            XElement   tUiNode = ExecutableBase.MasterConfig.Root;
            XAttribute tAttrib = null;
            Type       tType   = null;
            if (tUiNode != null
                && (tUiNode = tUiNode.Descendants("ui").FirstOrDefault())       != null
                && (tUiNode = tUiNode.Descendants("settings").FirstOrDefault()) != null
                && tUiNode.HasElements)
                foreach (XElement node in tUiNode.Elements())
                    if ((tAttrib = node.Attributes("Type").FirstOrDefault()) != null &&
                        (tType = Type.GetType(tAttrib.Value))                != null)
                        if (tType.IsPrimitive)
                            mData.Add(node.Name.LocalName, Convert.ChangeType(node.Value, tType));
                        else
                            mData.Add(node.Name.LocalName, TypeDescriptor.GetConverter(tType).ConvertFrom(node.Value));
        }
        public static void Save()
        {
            Type   tType   = Type.GetType("Procon.UI.Settings");
            Config tConfig = new Config().Generate(tType);
            foreach (String key in mData.Keys)
                if (mData[key].Value != null)
                    tConfig.Root.Add(new XElement(key, new XAttribute("Type", mData[key].Value.GetType().AssemblyQualifiedName), mData[key].Value));
            tConfig.Save(new FileInfo(Path.Combine(Core.Utils.Defines.CONFIGS_DIRECTORY, "Procon.UI.xml")));
        }
    }
}
