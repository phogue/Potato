using System;
using System.Windows;

using Procon.UI.API;

namespace Procon.UI.Default.DontUseMe.ImForTesting
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Header : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Main Template"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * private ArrayDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Template"];
         * private ArrayDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Template"];
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             * Grid tLayout = ExtensionApi.FindControl<Grid>(root, "ControlName");
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            

            // Do what I need to setup my control.
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             * TemplateView tView = new TemplateView();
             * tLayout.Children.Add(tView);
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */


            // Setup default property values.
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             * tProps["Name1"].Value = 2000;
             * tProps["Name2"].Value = 2.0;
             * tProps["Name3"].Value = "Bob";
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */


            // Commands.
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             * tCmmds["Name"].Value = new RelayCommand<AttachedCommandArgs>(
             * #region -- Command Description.
             *     x => {
             *         // Do Stuff.
             *     },
             *     x => {
             *         // Check Stuff.
             *     });
             * #endregion
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */


            // Information management.
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             * NotifiableCollection<Item> tItems   = null;
             * NotifiableCollection<Item> tManaged = new NotifiableCollection<Item>();
             * 
             * Action<Item> tAction = i =>
             * #region -- Calls tProps["Action"]
             * {
             *     // Call tProps["Action"].Value
             * };
             * #endregion
             * 
             * tProps["Action"].Value = new Action<Item>(
             * #region -- Action Description.
             *     x => {
             *         // Do Stuff.
             *     });
             * #endregion
             * 
             * tProps["Swap"].Value = new Action<ViewModel>(
             * #region -- Action Description.
             *     x => {
             *         // Change to the new information.
             *     });
             * #endregion
             * 
             * ExtensionApi.Properties["ViewModel"].PropertyChanged += (s, e) => {
             *     ((Action<ViewModel>)tProps["Swap"].Value)(ExtensionApi.Properties["ViewModel"].Value);
             * };
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */


            // Exit with good status.
            return true;
        }
    }
}
