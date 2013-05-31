using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Procon.Core;
using Procon.Core.Interfaces.Connections.Plugins;
using Procon.Core.Interfaces.Connections.Plugins.Variables;
using Procon.Net.Protocols.Objects;
using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Utils;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default.Root.Main.Connection.Plugins
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Plugins : IExtension
    {
        #region IExtension Properties

        public String Author
        { get { return "Imisnew2"; } }

        public Uri Link
        { get { return new Uri("www.TeamPlayerGaming.com/members/Imisnew2.html"); } }

        public String LinkText
        { get { return "Team Player Gaming"; } }

        public String Name
        { get { return "Main Connection Plugins"; } }

        public String Description
        { get { return ""; } }

        public Version Version
        { get { return new Version(1, 0, 0, 0); } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private ArrayDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Connection"]["Plugins"];
        private ArrayDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Connection"]["Plugins"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid tLayout = ExtensionApi.FindControl<Grid>(root, "MainConnectionLayout");


            // Do what I need to setup my control.
            PluginsView tView = new PluginsView();
            Grid.SetRow(tView, 1);
            tLayout.Children.Add(tView);


            // Setup default property values.
            tProps["Variables"].Value = new NotifiableCollection<Tuple<String, NotifiableCollection<Variable>>>();


            // Commands.
            tCmmds["Selected"].Value = new RelayCommand<AttachedCommandArgs>(
            #region -- Populates the variables list with the current plugin's info.
                x => {
                    var tPlugin    = (Plugin)tProps["Selected"].Value;
                    var tVariables = (NotifiableCollection<Tuple<String, NotifiableCollection<Variable>>>)tProps["Variables"].Value;

                    // Clear old info.
                    if (tVariables != null) {
                        foreach (Tuple<String, NotifiableCollection<Variable>> coll in tVariables)
                            coll.Item2.Clear();
                        tVariables.Clear();
                    }

                    // Add new info.
                    if (tPlugin != null) {
                        foreach (IGrouping<String, Variable> group in tPlugin.PluginDetails.PluginVariables.GroupBy(y => y.Group)) {
                            var varGroup = new Tuple<String, NotifiableCollection<Variable>>(group.Key, new NotifiableCollection<Variable>());
                            foreach (Variable var in group)
                                varGroup.Item2.Add(var);
                            tVariables.Add(varGroup);
                        }
                    }
                });
            #endregion
            tCmmds["SetVariable"].Value = new RelayCommand<AttachedCommandArgs>(
            #region -- Sets the passed in variable's value.
                x => {
                    var tPlugin   = (Plugin)tProps["Selected"].Value;
                    var tVariable = (Variable)x.Parameter;
                    if (tPlugin != null && tVariable != null)
                        tPlugin.SetPluginVariable(CommandInitiator.Local, tVariable);
                });
            #endregion


            // Exit with good status.
            return true;
        }
    }
}
