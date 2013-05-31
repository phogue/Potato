using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Procon.UI.Default.Root.Main.Overview.Interfaces.Settings
{
    using Procon.UI.API;
    using Procon.UI.API.Commands;
    using Procon.UI.API.Utils;
    using Procon.UI.API.ViewModels;
    using Procon.UI.Default.Setup.Adorners;

    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Settings : IExtension
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


        // Easy access to the view setting for this panel.
        private ArrayDictionary<String, Object> mView = ExtensionApi.Settings["ViewMois"];


        // An easy accessor for Properties and Commands of this control.
        private readonly ArrayDictionary<String, Object>   mProps;
        private readonly ArrayDictionary<String, ICommand> mComms;
        public Settings()
        {
            mProps = ExtensionApi.GetProperties(GetType());
            mComms = ExtensionApi.GetCommands(GetType());
        }


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid tLayout = ExtensionApi.FindControl<Grid>(root, "MainOverviewInterfacesLayout");
            if (tLayout == null) {
                return false;
            }


            // Do what I need to setup my control.
            SettingsView tView = new SettingsView();
            Grid.SetRow(tView, 1);
            tLayout.Children.Add(tView);

            DottedBorderAdorner tDottedBorder = new DottedBorderAdorner(tView.MoisOpen);
            tDottedBorder.SetResourceReference(DottedBorderAdorner.StrokeProperty, "Brush2Dark");
            tDottedBorder.StrokeThickness = 2;
            tDottedBorder.StrokeDashArray = new DoubleCollection(new Double[] { 15, 5 });
            tDottedBorder.XRadius = 8;
            tDottedBorder.YRadius = 8;
            tDottedBorder.IsHitTestVisible = false;
            AdornerLayer tAdornerLayer = AdornerLayer.GetAdornerLayer(tView.MoisOpen);
            tAdornerLayer.Add(tDottedBorder);


            // Setup the default settings.
            mProps["Selected"].Value = tView.MoisSelected;
            View_Checked(new AttachedCommandArgs(null, null, mView.Value));


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
            mView.Value = args.Parameter;


            // Get the selected ring for animation.
            Image tSelected = mProps["Selected"].Value as Image;
            if (tSelected != null) {

                // Calculate the offset given which view we're navigating to.
                Int32 tOffset =
                    (String)args.Parameter == "Groups"  ? 78  :
                    (String)args.Parameter == "Layer"   ? 156 :
                    /* Accounts */ 0;

                // Animate the ring moving to the new value.
                Storyboard story = new Storyboard();
                Storyboard.SetTarget(story, tSelected);
                Storyboard.SetTargetProperty(story, new PropertyPath(Image.MarginProperty));

                ThicknessAnimation movement = new ThicknessAnimation();
                movement.DecelerationRatio = 1.0;
                movement.Duration = TimeSpan.FromMilliseconds(400);
                movement.From = tSelected.Margin;
                movement.To = new Thickness(tOffset, 0, 0, 5);

                story.Children.Add(movement);
                story.Begin();
            }
        }
    }
}
