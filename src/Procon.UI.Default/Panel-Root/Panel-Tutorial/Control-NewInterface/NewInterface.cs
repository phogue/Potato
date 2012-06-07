using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Procon.UI.API;
using Procon.UI.API.Classes;
using Procon.UI.API.Commands;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default.Root.Tutorial.NewInterface
{
    [Extension(
        Alters    = new String[] { "TutorialLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Tutorial Layout" })]
    public class NewInterface : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Tutorial New Interface"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private InfinityDictionary<String, Object>   tProps = InstanceViewModel.PublicProperties["Tutorial"]["NewInterface"];
        private InfinityDictionary<String, ICommand> tCmmds = InstanceViewModel.PublicCommands["Tutorial"]["NewInterface"];

        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid layout = ExtensionApi.FindControl<Grid>(root, "TutorialLayout");
            if  (layout == null) return false;

            // Do what I need to setup my control.
            NewInterfaceView view = new NewInterfaceView();
            layout.Children.Add(view);

            // Commands
            tCmmds["Remote"].Value = new RelayCommand<Object>(
                // -- Handles when the "Remote Interface" button is clicked.
                x => {
                    InstanceViewModel.PublicCommands["Interface"]["Add"].Value.Execute(
                        new Object[] {
                            tProps["Host"].Value,
                            tProps["Port"].Value,
                            tProps["User"].Value,
                            tProps["Pass"].Value
                        });
                    ExtensionApi.Interface = ExtensionApi.Procon.Interfaces.SingleOrDefault(y => y.Hostname.ToString() == tProps["Host"].Value && y.Port.ToString() == tProps["Port"].Value);
                    if (ExtensionApi.Interface != null)
                        ExtensionApi.Settings["Tutorial"].Value = "Done";
                },
                x => {
                    return InstanceViewModel.PublicCommands["Interface"]["Add"].Value != null &&
                           InstanceViewModel.PublicCommands["Interface"]["Add"].Value.CanExecute(
                            new Object[] {
                                tProps["Host"].Value,
                                tProps["Port"].Value,
                                tProps["User"].Value,
                                tProps["Pass"].Value
                            });
                });
            tCmmds["Local"].Value = new RelayCommand<Object>(
                // -- Handles when the "Local Interface" button is clicked.
                x => {
                    ExtensionApi.Interface = ExtensionApi.Procon.Interfaces.SingleOrDefault(y => y.IsLocal);
                    if (ExtensionApi.Interface != null)
                        ExtensionApi.Settings["Tutorial"].Value = "Done";
                },
                x => {
                    return ExtensionApi.Procon != null;
                });
            tCmmds["Pass"].Value = new RelayCommand<String>(
                // -- Handles when the password changes.
                pass => {
                    tProps["Pass"].Value = pass;
                });

            // Exit with good status.
            return true;
        }
    }
}
