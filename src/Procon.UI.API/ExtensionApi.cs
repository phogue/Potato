using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

using Procon.UI.API.Utils;
using Procon.UI.API.ViewModels;

namespace Procon.UI.API
{
    public static class ExtensionApi
    {
        // Sets up the parser context.
        static ParserContext mParserContext = null;
        static ExtensionApi()
        {
            if (mParserContext == null) {
                mParserContext = new ParserContext();
                mParserContext.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                mParserContext.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
                mParserContext.XmlnsDictionary.Add("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
                mParserContext.XmlnsDictionary.Add("conv", "clr-namespace:Procon.UI.API.Converters;assembly=Procon.UI.API");
                mParserContext.XmlnsDictionary.Add("view", "clr-namespace:Procon.UI.API.ViewModels;assembly=Procon.UI.API");
                mParserContext.XmlnsDictionary.Add("acmd", "clr-namespace:Procon.UI.API.Commands;assembly=Procon.UI.API");
                mParserContext.XmlnsDictionary.Add("util", "clr-namespace:Procon.UI.API.Utils;assembly=Procon.UI.API");
            }
        }

        // Used to parse xaml as a specific object.
        public static T ParseXaml<T>(String xaml) where T : class
        {
            using (MemoryStream tStreamReader = new MemoryStream(Encoding.ASCII.GetBytes(xaml)))
                return (T)XamlReader.Load(tStreamReader, mParserContext);
        }
        // Used to find controls under a specific element.
        public static T FindControl<T>(DependencyObject controlAncestor, String controlName) where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (controlAncestor == null || String.IsNullOrEmpty(controlName))
                return null;

            // Check to see if it is a content control...
            // Otherwise, check all of it's children to see if they're what we're looking for.
            ContentControl controlAncestorCC = controlAncestor as ContentControl;
            if (controlAncestorCC != null)
            {
                // Get it's content and see if it's the type of element we're looking for.
                FrameworkElement controlContent = (controlAncestorCC.Content as FrameworkElement);
                if (controlContent != null)
                {
                    // Check to see if the names match...
                    // Otherwise, check this control's children for a match.
                    if (controlContent.Name == controlName)
                        return controlContent as T;
                    else
                    {
                        T controlContentChild = FindControl<T>(controlContent, controlName);
                        if (controlContentChild != null)
                            return controlContentChild;
                    }
                }
            }
            else
            {
                // Get the number of children this control has and iterate through them all.
                Int32 childCount = VisualTreeHelper.GetChildrenCount(controlAncestor);
                for (Int32 i = 0; i < childCount; i++)
                {
                    // Get the current child and...
                    // Compare the child's name to see if this child is the control we're looking for.
                    FrameworkElement child = VisualTreeHelper.GetChild(controlAncestor, i) as FrameworkElement;
                    if (child != null)
                    {
                        // Check to see if the names match...
                        // Otherwise, check this control's children for a match.
                        if (child.Name == controlName)
                            return child as T;
                        else
                        {
                            T childChild = FindControl<T>(child, controlName);
                            if (childChild != null)
                                return childChild;
                        }
                    }
                }
            }

            // Return Child not found.
            return null;
        }


        // Easy Getter/Setter for [Settings], [Procon], [Interface], and [Connection] property.
        public static InfinityDictionary<String, Object> Settings
        {
            get { return InstanceViewModel.PublicProperties["Settings"]; }
        }
        public static InstanceViewModel Procon
        {
            get { return InstanceViewModel.PublicProperties["Procon"].Value as InstanceViewModel; }
        }
        public static InterfaceViewModel Interface
        {
            get { return InstanceViewModel.PublicProperties["Interface"].Value as InterfaceViewModel; }
            set {
                InstanceViewModel.PublicProperties["Interface"].Value                    = value;
                InstanceViewModel.PublicProperties["Settings"]["InterfaceIsLocal"].Value = (value != null) ? (Object)value.IsLocal  : null;
                InstanceViewModel.PublicProperties["Settings"]["InterfaceHost"].Value    = (value != null) ? (Object)value.Hostname : null;
                InstanceViewModel.PublicProperties["Settings"]["InterfacePort"].Value    = (value != null) ? (Object)value.Port     : null;
            }
        }
        public static ConnectionViewModel Connection
        {
            get { return InstanceViewModel.PublicProperties["Connection"].Value as ConnectionViewModel; }
            set {    
                InstanceViewModel.PublicProperties["Connection"].Value                 = value;
                InstanceViewModel.PublicProperties["Settings"]["ConnectionType"].Value = (value != null) ? (Object)value.GameType.ToString() : null;
                InstanceViewModel.PublicProperties["Settings"]["ConnectionHost"].Value = (value != null) ? (Object)value.Hostname            : null;
                InstanceViewModel.PublicProperties["Settings"]["ConnectionPort"].Value = (value != null) ? (Object)value.Port                : null;
            }
        }
    }
}
