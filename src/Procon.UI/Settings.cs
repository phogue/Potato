using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;

using Procon.Core;
using Procon.UI.API;
using Procon.UI.API.Classes;
using Procon.UI.API.ViewModels;

namespace Procon.UI
{
    public static class Settings
    {
        // Holds the information for the Settings, Assemblies, and Extensions.
        private static InfinityDictionary<String, Object> mSettings   = InstanceViewModel.PublicProperties["Settings"];
        private static List<Assembly>                     mAssemblies = new List<Assembly>();
        private static List<Extension>                    mExtensions = new List<Extension>();

        // Easy to use accessor/mutator for the settings.
        public static T Get<T>(String key, T fallback = default(T))
        {
            if (mSettings.ContainsKey(key))
                return (T)mSettings[key].Value;
            return fallback;
        }
        public static void Set(String key, Object value)
        {
            mSettings[key].Value = value;
        }

        // Loads/Saves the Settings, Assemblies, and Extensions.
        public static void Load()
        {
            // Load the settings from the config file.
            XElement tNode = ExecutableBase.MasterConfig.Root;
            if (tNode != null
                && (tNode = tNode.Element("ui"))         != null
                && (tNode = tNode.Element("settings"))   != null) {
                XAttribute tAttribute  = null;
                Type       tAttribType = null;

                // Iterate over each setting and save it in the "Settings" array.
                foreach (XElement tNodeSetting in tNode.Elements())
                    if ((tAttribute = tNodeSetting.Attributes("type").FirstOrDefault()) != null &&
                        (tAttribType = Type.GetType(tAttribute.Value))                  != null)
                        if (tAttribType.IsPrimitive)
                            mSettings.Add(tNodeSetting.Name.LocalName, Convert.ChangeType(tNodeSetting.Value, tAttribType));
                        else
                            mSettings.Add(tNodeSetting.Name.LocalName, TypeDescriptor.GetConverter(tAttribType).ConvertFrom(tNodeSetting.Value));

                // Load the assemblies and extensions from the config file.
                if ((tNode = tNode.Element("extensions")) != null) {
                    List<Extension> tExtensions = new List<Extension>();
                    Extension       tExtension  = null;
                    Assembly        tAssembly   = null;
                    List<Type>      tTypes      = null;

                    // Load all the assemblies specified into the app domain.
                    foreach (XElement tNodeAssembly in tNode.Elements("assembly"))
                        try {
                            tAssembly = Assembly.LoadFrom(tNodeAssembly.Value);
                            tTypes    = new List<Type>(tAssembly.GetTypes().Where(x => typeof(IExtension).IsAssignableFrom(x)));

                            // Create an instance of each extension contained in the assemblies.
                            foreach (Type tType in tTypes)
                                try {
                                    tExtension = new Extension((IExtension)tAssembly.CreateInstance(tType.FullName));
                                    if (tExtension.IExtension != null)
                                        tExtensions.Add(tExtension);
                                } catch (Exception) { }

                            mAssemblies.Add(tAssembly);
                        } catch (Exception) { }

                    // Keep only the specified extensions in the list.
                    foreach (XElement tNodeExtension in tNode.Elements("extension"))
                        if ((tExtension = tExtensions.FirstOrDefault(x => x.IExtension.Name == tNodeExtension.Value)) != null)
                            mExtensions.Add(tExtension);
                }
            }
        }
        public static void Save()
        {
            // Generate the config file.
            Config tConfig = new Config().Generate(typeof(Settings));

            // Save each item in the "Settings" array.
            foreach (String tKey in mSettings.Keys)
                if (mSettings[tKey].Value != null)
                    tConfig.Root.Add(
                        new XElement(tKey,
                            new XAttribute("type",
                                (mSettings[tKey].Value.GetType().Module.Name == "mscorlib.dll") ?
                                    mSettings[tKey].Value.GetType().FullName :
                                    mSettings[tKey].Value.GetType().AssemblyQualifiedName
                            ),
                            mSettings[tKey].Value
                    ));

            // Save each assembly and extension that was loaded.
            XElement tNode = new XElement("extensions");
            foreach (Assembly tAssembly in mAssemblies)
                tNode.Add(new XElement("assembly", AppDomain.CurrentDomain.BaseDirectory.RelativeTo(tAssembly.Location)));
            foreach (Extension tExtension in mExtensions)
                tNode.Add(new XElement("extension", tExtension.IExtension.Name));
            tConfig.Root.Add(tNode);

            // Write the config file.
            tConfig.Save(new FileInfo(Path.Combine(Core.Utils.Defines.CONFIGS_DIRECTORY, String.Format("{0}.xml", typeof(Settings).Namespace))));
        }

        // Executes the extensions that should be and have not been executed.
        public static void ExecuteExtensions(Window root)
        {
            foreach (Extension tExtension in mExtensions.Where(x => !x.IsExecuted))
                tExtension.Execute(root);
        }
    }
}
