using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Utils;

namespace Procon.UI.Default.Root.Tutorial.NewInterface
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
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
        private ArrayDictionary<String, Object>   tProps = ExtensionApi.Properties["Tutorial"]["NewInterface"];
        private ArrayDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Tutorial"]["NewInterface"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid tLayout = ExtensionApi.FindControl<Grid>(root, "TutorialLayout");

            // Do what I need to setup my control.
            NewInterfaceView tView = new NewInterfaceView();
            tLayout.Children.Add(tView);

            // Commands.
            tCmmds["Remote"].Value = new RelayCommand<Object>(
            #region -- Handles when the "Remote Interface" button is clicked.
                x => {
                    ExtensionApi.Commands["Interface"]["Add"].Value.Execute(
                        new Object[] {
                            tProps["Host"].Value,
                            tProps["Port"].Value,
                            tProps["User"].Value,
                            tProps["Pass"].Value
                        });
                    ExtensionApi.Interface = ExtensionApi.Procon.Interfaces.SingleOrDefault(y => y.Hostname.ToString() == (String)tProps["Host"].Value && y.Port.ToString() == (String)tProps["Port"].Value);
                    if (ExtensionApi.Interface != null)
                        ExtensionApi.Settings["Tutorial"].Value = "Done";
                },
                x => {
                    return ExtensionApi.Commands["Interface"]["Add"].Value != null &&
                           ExtensionApi.Commands["Interface"]["Add"].Value.CanExecute(
                            new Object[] {
                                tProps["Host"].Value,
                                tProps["Port"].Value,
                                tProps["User"].Value,
                                tProps["Pass"].Value
                            });
                });
            #endregion
            tCmmds["Local"].Value = new RelayCommand<Object>(
            #region -- Handles when the "Local Interface" button is clicked.
                x => {
                    ExtensionApi.Interface = ExtensionApi.Procon.Interfaces.SingleOrDefault(y => y.IsLocal);
                    if (ExtensionApi.Interface != null)
                        ExtensionApi.Settings["Tutorial"].Value = "Done";
                },
                x => {
                    return ExtensionApi.Procon != null;
                });
            #endregion
            tCmmds["Pass"].Value = new RelayCommand<String>(
            #region -- Handles when the password changes.
                pass => {
                    tProps["Pass"].Value = pass;
                });
            #endregion

            // Exit with good status.
            return true;
        }
    }
}
