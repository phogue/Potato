using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Procon.UI.Default.Root.Introduction
{
    using Procon.UI.API;
    using Procon.UI.API.Commands;
    using Procon.UI.API.Utils;

    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Introduction : IExtension
    {
        #region IExtension Properties

        public String Author
        { get { return "Imisnew2"; } }

        public Uri Link
        { get { return new Uri("www.TeamPlayerGaming.com/members/Imisnew2.html"); } }

        public String LinkText
        { get { return "TeamPlayer Gaming"; } }

        public String Name
        { get { return GetType().Namespace; } }

        public String Description
        { get { return ""; } }

        public Version Version
        { get { return new Version(1, 0, 0, 0); } }

        #endregion IExtension Properties

        
        // An easy accessor for Properties and Commands of this control.
        private readonly ArrayDictionary<String, Object>   mProps;
        private readonly ArrayDictionary<String, ICommand> mComms;
        public Introduction()
        {
            mProps = ExtensionApi.GetProperties(GetType());
            mComms = ExtensionApi.GetCommands(GetType());
        }


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid tLayout = ExtensionApi.FindControl<Grid>(root, "RootLayout");
            if (tLayout == null) {
                return false;
            }


            // Do what I need to setup my control.
            IntroductionView tView = new IntroductionView();
            tLayout.Children.Add(tView);


            // Setup the default settings.
            if (ExtensionApi.Settings["Introduction"].Value == null) {
                ExtensionApi.Settings["Introduction"].Value = "Step 1";
            }


            // Commands.
            mComms["Pass"].Value = new RelayCommand<AttachedCommandArgs>(Password_Changed);
            mComms["Local"].Value = new RelayCommand<Object>(LocalInterface_Click, LocalInterface_CanClick);
            mComms["Remote"].Value = new RelayCommand<Object>(RemoteInterface_Click, RemoteInterface_CanClick);


            // Exit with good status.
            return true;
        }


        /// <summary>
        /// Update the underlying property for the password.
        /// </summary>
        private void Password_Changed(AttachedCommandArgs args)
        {
            PasswordBox tElement = args.Sender as PasswordBox;
            if (tElement != null) {
                mProps["Pass"].Value = tElement.Password;
            }
        }


        /// <summary>
        /// Swap the active interface to the local interface and exit the tutorial.
        /// </summary>
        private void LocalInterface_Click(Object args)
        {
            ExtensionApi.Interface = ExtensionApi.Procon.Interfaces.SingleOrDefault(y => y.IsLocal);

            // Swap to the "Done" setting of the introduction if we were successful.
            if (ExtensionApi.Interface != null) {
                ExtensionApi.Settings["Introduction"].Value = "Done";
            }
        }

        private bool LocalInterface_CanClick(Object args)
        {
            // Just simply make sure the instance exists.
            return ExtensionApi.Procon != null;
        }


        /// <summary>
        /// Attempt to create the remote interface, swap to it, and exit the tutorial. 
        /// </summary>
        private void RemoteInterface_Click(Object args)
        {
            ExtensionApi.Commands["Interface"]["Add"].Value.Execute(
                new Object[] {
                    mProps["Host"].Value,
                    mProps["Port"].Value,
                    mProps["User"].Value,
                    mProps["Pass"].Value
                });

            ExtensionApi.Interface = ExtensionApi.Procon.Interfaces.SingleOrDefault(y => 
                y.Hostname.ToString() == (String)mProps["Host"].Value
                && y.Port.ToString()  == (String)mProps["Port"].Value
            );

            // Swap to the "Done" setting of the introduction if we were successful.
            if (ExtensionApi.Interface != null) {
                ExtensionApi.Settings["Introduction"].Value = "Done";

                // Clear the fields for.. security? or something.
                mProps["Host"].Value = null;
                mProps["Port"].Value = null;
                mProps["User"].Value = null;
                mProps["Pass"].Value = null;
            }
        }

        private bool RemoteInterface_CanClick(Object args)
        {
            // Check if the command exists then call its "Can" method.
            return ExtensionApi.Commands["Interface"]["Add"].Value != null &&
                ExtensionApi.Commands["Interface"]["Add"].Value.CanExecute(
                new Object[] {
                    mProps["Host"].Value,
                    mProps["Port"].Value,
                    mProps["User"].Value,
                    mProps["Pass"].Value
                });
        }
    }
}
