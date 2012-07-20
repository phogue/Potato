using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

using Procon.Core;
using Procon.UI.API.Utils;
using Procon.UI.API.ViewModels;

namespace Procon.UI.API
{
    public static class ExtensionApi
    {
        // Internal variables.
        private static Dictionary<Object, Panel> mTemplates { get; set; }
        private static ParserContext             mContext   { get; set; }


        // The Properties and Commands of the UI.
        public static ArrayDictionary<String, Object>   Properties { get; set; }
        public static ArrayDictionary<String, ICommand> Commands   { get; set; }

        // Easy access to the currently selected Procon instance, Interface, and Connection.
        public static InstanceViewModel Procon
        {
            get { return Properties["Procon"].Value as InstanceViewModel; }
        }
        public static InterfaceViewModel Interface
        {
            get { return Properties["Interface"].Value as InterfaceViewModel; }
            set { Properties["Interface"].Value = value; }
        }
        public static ConnectionViewModel Connection
        {
            get { return Properties["Connection"].Value as ConnectionViewModel; }
            set { Properties["Connection"].Value = value; }
        }

        // Uniform access to the settings that will be saved and loaded when closing and opening Procon.
        public static ArrayDictionary<String, Object> Settings
        {
            get { return Properties["Settings"]; }
        }


        // Static Constructor.
        static ExtensionApi()
        {
            mTemplates = new Dictionary<Object, Panel>();
            mContext   = new ParserContext();

            Properties = new ArrayDictionary<String, Object>();
            Commands   = new ArrayDictionary<String, ICommand>();
        }


        // Find a control under a specific element.
        public static T FindControl<T>(UIElement controlAncestor, String controlName) where T : UIElement
        {
            // Confirm controlAncestor and controlName are valid. 
            if (controlAncestor == null || String.IsNullOrEmpty(controlName))
                return null;

            ContentControl tContent = controlAncestor as ContentControl;
            // If it's a content control...
            if (tContent != null) {
                // Check to see if the child matches our specifications...
                FrameworkElement tChild = (tContent.Content as FrameworkElement);
                if (tChild != null)
                    if (tChild.Name == controlName) return tChild as T;
                    else                            return FindControl<T>(tChild, controlName);
            }
            // Else, it's a normal control...
            else {
                // Iterate over all the childen in the control...
                Int32 tCount = VisualTreeHelper.GetChildrenCount(controlAncestor);
                for (Int32 i = 0; i < tCount; i++) {
                    // Get the current child we're dealing with...
                    FrameworkElement tChild = VisualTreeHelper.GetChild(controlAncestor, i) as FrameworkElement;
                    if (tChild != null) {
                        // Check to see if the child matches our specifications...
                        if (tChild.Name == controlName) {
                            return tChild as T;
                        }
                        else {
                            T childChild = FindControl<T>(tChild, controlName);
                            if (childChild != null)
                                return childChild;
                        }
                    }
                }
            }

            // Return Child not found.
            return null;
        }
        public static T ParseControl<T>(String controlXaml) where T : class
        {
            if (mContext.XmlnsDictionary.Count == 0) {
                mContext.XmlnsDictionary.Add("",     "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                mContext.XmlnsDictionary.Add("x",    "http://schemas.microsoft.com/winfx/2006/xaml");
                mContext.XmlnsDictionary.Add("eapi", "clr-namespace:Procon.UI.API;assembly=Procon.UI.API");
                mContext.XmlnsDictionary.Add("acmd", "clr-namespace:Procon.UI.API.Commands;assembly=Procon.UI.API");
                mContext.XmlnsDictionary.Add("conv", "clr-namespace:Procon.UI.API.Converters;assembly=Procon.UI.API");
                mContext.XmlnsDictionary.Add("util", "clr-namespace:Procon.UI.API.Utils;assembly=Procon.UI.API");
                mContext.XmlnsDictionary.Add("view", "clr-namespace:Procon.UI.API.ViewModels;assembly=Procon.UI.API");
            }
            return XamlReader.Parse(controlXaml, mContext) as T;
        }

        // Methods used to build a data template from the code behind.
        public static void CreateTemplate(String key, Panel content)
        {
            if (mTemplates.ContainsKey(key))
                mTemplates.Remove(key);
            mTemplates.Add(key, content);
        }
        public static void CreateTemplate(Type   key, Panel content)
        {
            if (mTemplates.ContainsKey(key))
                mTemplates.Remove(key);
            mTemplates.Add(key, content);
        }
        public static T GetTemplateControl<T>(String key) where T : Panel
        {
            if (mTemplates.ContainsKey(key))
                return mTemplates[key] as T;
            return null;
        }
        public static T GetTemplateControl<T>(Type   key) where T : Panel
        {
            if (mTemplates.ContainsKey(key))
                return mTemplates[key] as T;
            return null;
        }
        public static DataTemplate GetTemplate(String key)
        {
            if (mTemplates.ContainsKey(key)) {
                DataTemplate tTemplate = ParseControl<DataTemplate>("<DataTemplate>" + XamlWriter.Save(mTemplates[key]) + "</DataTemplate>");
                return tTemplate;
            }
            return null;
        }
        public static DataTemplate GetTemplate(Type   key)
        {
            if (mTemplates.ContainsKey(key)) {
                DataTemplate tTemplate = ParseControl<DataTemplate>("<DataTemplate>" + XamlWriter.Save(mTemplates[key]) + "</DataTemplate>");
                tTemplate.DataType = key;
                return tTemplate;
            }
            return null;
        }

        // String extensions.
        public static String Localize(this String key, params Object[] parameters)
        {
            Int32 nsIndex = key.LastIndexOf('.');
            if (nsIndex >= 0)
                return ExecutableBase.MasterLanguages.Loc(null, key.Substring(0, nsIndex), key.Substring(nsIndex + 1), parameters);
            return ExecutableBase.MasterLanguages.Loc(null, key, key, parameters);
        }
        public static String RelativeTo(this String from, String to)
        {
            if (String.IsNullOrEmpty(from))
                return to;
            return Uri.UnescapeDataString(
                new Uri(from).MakeRelativeUri(
                    new Uri(to)).ToString())
                .Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
