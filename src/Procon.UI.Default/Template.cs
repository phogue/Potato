using System;
using System.Windows;

using Procon.UI.API;

namespace Procon.UI.Default.DontUseMe.ImForTesting
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Template : IExtension
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


        //// An easy accessor for Properties and Commands of this control.
        // private readonly ArrayDictionary<String, Object>   mProps;
        // private readonly ArrayDictionary<String, ICommand> mComms;
        // public Template()
        // {
        //     mProps = ExtensionApi.GetProperties(GetType());
        //     mComms = ExtensionApi.GetCommands(GetType());
        // }
        //// * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * //


        [STAThread]
        public bool Entry(Window root)
        {
            //// Find the controls I want to use and check for issues.
            // var tLayout = ExtensionApi.FindControl<Grid>(root, "ControlName");
            // if (tLayout == null) {
            //     return false;
            // }
            //// * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * //
            

            //// Do what I need to setup my control.
            // TemplateView tView = new TemplateView();
            // tLayout.Children.Add(tView);
            //// * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * //


            //// Setup default property values.
            // tProps["Name1"].Value = 2000;
            // tProps["Name2"].Value = 2.0;
            // tProps["Name3"].Value = "Bob";
            //// * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * //


            //// Commands.
            // tCmmds["Name"].Value = new RelayCommand<AttachedCommandArgs>(
            // #region -- Command Description.
            //     x => {
            //         // Do Stuff.
            //     },
            //     x => {
            //         // Check Stuff.
            //     });
            // #endregion
            //// * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * //


            //// Information management.
            // NotifiableCollection<Item> tItems   = null;
            // NotifiableCollection<Item> tManaged = new NotifiableCollection<Item>();
            // 
            // Action<Item> tAction = i =>
            // #region -- Calls tProps["Action"]
            // {
            //     // Call tProps["Action"].Value
            // };
            // #endregion
            // 
            // tProps["Action"].Value = new Action<Item>(
            // #region -- Action Description.
            //     x => {
            //         // Do Stuff.
            //     });
            // #endregion
            // 
            // tProps["Swap"].Value = new Action<ViewModel>(
            // #region -- Action Description.
            //     x => {
            //         // Change to the new information.
            //     });
            // #endregion
            // 
            // ExtensionApi.Properties["ViewModel"].PropertyChanged += (s, e) => {
            //     ((Action<ViewModel>)tProps["Swap"].Value)(ExtensionApi.Properties["ViewModel"].Value);
            // };
            //// * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * //


            // Exit with good status.
            return true;
        }
    }
}
