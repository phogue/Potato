using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Procon.UI.Default.Root.Main.Overview.Header
{
    using Procon.UI.API;
    using Procon.UI.API.Commands;
    using Procon.UI.API.Utils;

    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Header : IExtension
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
        public Header()
        {
            mProps = ExtensionApi.GetProperties(GetType());
            mComms = ExtensionApi.GetCommands(GetType());
        }


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid tLayout = ExtensionApi.FindControl<Grid>(root, "MainOverviewLayout");
            if (tLayout == null) {
                return false;
            }


            // Do what I need to setup my control.
            HeaderView tView = new HeaderView();
            tLayout.Children.Add(tView);


            // Setup the default settings.
            mProps["Selected"].Value = tView.MohSelected;
            View_Checked(new AttachedCommandArgs(null, null, ExtensionApi.Settings["ViewMo"].Value));


            // Commands.
            mComms["View"].Value = new RelayCommand<AttachedCommandArgs>(View_Checked);


            // Exit with good status.
            return true;
        }


        /// <summary>
        /// Handles changing the view whenever a button is selected.
        /// </summary>
        private void View_Checked(AttachedCommandArgs args)
        {
            // Set the next view.
            ExtensionApi.Settings["ViewMo"].Value = args.Parameter;


            // Get the selected ring for animation.
            Image tSelected = mProps["Selected"].Value as Image;
            if (tSelected != null) {

                // Calculate the offset given which view we're navigating to.
                Int32 tOffset = 0;
                if ((String)args.Parameter == "Connections") {
                    tOffset = 78;
                }

                // Animate the ring moving to the new value.
                Storyboard story = new Storyboard();
                Storyboard.SetTarget(story, tSelected);
                Storyboard.SetTargetProperty(story, new PropertyPath(Image.MarginProperty));

                ThicknessAnimation movement = new ThicknessAnimation();
                movement.DecelerationRatio = 1.0;
                movement.Duration = TimeSpan.FromMilliseconds(400);
                movement.From = tSelected.Margin;
                movement.To   = new Thickness(tOffset, 0, 0, 0);

                story.Children.Add(movement);
                story.Begin();
            }
        }
    }
}
