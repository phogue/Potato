using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Procon.UI.Default.Root.Main.Overview.Interfaces.Manage
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
    public class Manage : IExtension
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
        public Manage()
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
            ManageView tView = new ManageView();
            tLayout.Children.Add(tView);

            DottedBorderAdorner tDottedBorder = new DottedBorderAdorner(tView.MoimOpen);
            tDottedBorder.SetResourceReference(DottedBorderAdorner.StrokeProperty, "Brush2Dark");
            tDottedBorder.StrokeThickness = 2;
            tDottedBorder.StrokeDashArray = new DoubleCollection(new Double[] { 15, 5 });
            tDottedBorder.XRadius = 8;
            tDottedBorder.YRadius = 8;
            tDottedBorder.IsHitTestVisible = false;
            AdornerLayer tAdornerLayer = AdornerLayer.GetAdornerLayer(tView.MoimOpen);
            tAdornerLayer.Add(tDottedBorder);


            // Setup the default settings.
            mProps["Default"].Value = tView.MoimContentContainer;
            mProps["AddArea"].Value  = tView.MoimAddContainer;


            // Commands.
            mComms["Open"].Value   = new RelayCommand<Object>(Open_Click);
            mComms["Close"].Value  = new RelayCommand<Object>(Close_Click);
            mComms["Add"].Value    = new RelayCommand<Object>(AddInterface_Click, AddInterface_CanClick);
            mComms["Remove"].Value = new RelayCommand<InterfaceViewModel>(RemoveInterface_Click, RemoveInterface_CanClick);
            mComms["Pass"].Value   = new RelayCommand<AttachedCommandArgs>(Password_Changed);


            // Exit with good status.
            return true;
        }


        /// <summary>
        /// Runs the storyboard to open the add interface view.
        /// </summary>
        private void Open_Click(Object nothing)
        {
            Border tDefaultContainer = mProps["Default"].Value as Border;
            Grid   tAddContainer     = mProps["AddArea"].Value as Grid;
            if (tAddContainer == null
                || tDefaultContainer == null
                || tAddContainer.Visibility != Visibility.Collapsed) {
                return;
            }

            Storyboard story = new Storyboard();

            // Make sure the container is targetable before we start.
            ObjectAnimationUsingKeyFrames visibility = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTarget(visibility, tAddContainer);
            Storyboard.SetTargetProperty(visibility, new PropertyPath(Grid.VisibilityProperty));
            visibility.KeyFrames.Add(new DiscreteObjectKeyFrame() { 
                Value = Visibility.Visible,
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))
            });

            // Make sure the container pops out of the bottom,
            // then fills the area once it's fully open.
            ObjectAnimationUsingKeyFrames valignment = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTarget(valignment, tAddContainer);
            Storyboard.SetTargetProperty(valignment, new PropertyPath(Grid.VerticalAlignmentProperty));
            valignment.KeyFrames.Add(new DiscreteObjectKeyFrame() {
                Value = VerticalAlignment.Bottom,
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))
            });
            valignment.KeyFrames.Add(new DiscreteObjectKeyFrame() {
                Value = VerticalAlignment.Stretch,
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1000))
            });

            // Make sure the container is not transparent as well.
            DoubleAnimation opacity = new DoubleAnimation();
            Storyboard.SetTarget(opacity, tAddContainer);
            Storyboard.SetTargetProperty(opacity, new PropertyPath(Grid.OpacityProperty));
            opacity.Duration = TimeSpan.FromMilliseconds(0);
            opacity.From = 0;
            opacity.To = 1;

            // The actual animation.
            // Make the container grow until it fills the area of the default container.
            DoubleAnimationUsingKeyFrames movement = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(movement, tAddContainer);
            Storyboard.SetTargetProperty(movement, new PropertyPath(Grid.HeightProperty));
            movement.DecelerationRatio = 1.0;
            movement.Duration = TimeSpan.FromMilliseconds(1000);
            movement.KeyFrames.Add(new LinearDoubleKeyFrame() {
                Value = 0,
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))
            });
            movement.KeyFrames.Add(new LinearDoubleKeyFrame() {
                Value = tDefaultContainer.ActualHeight - 1,
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1000))
            });
            movement.KeyFrames.Add(new LinearDoubleKeyFrame() {
                Value = Double.NaN,
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1000))
            });

            story.Children.Add(visibility);
            story.Children.Add(valignment);
            story.Children.Add(opacity);
            story.Children.Add(movement);
            story.Begin();
        }


        /// <summary>
        /// Runs the storyboard to close the add interface view.
        /// </summary>
        private void Close_Click(Object nothing)
        {
            Grid tAddContainer = mProps["AddArea"].Value as Grid;
            if (tAddContainer == null
                || tAddContainer.Opacity < 1.0) { return; }

            Storyboard story = new Storyboard();

            // Make sure the container is non-targetable (collapsed) when it's closed.
            ObjectAnimationUsingKeyFrames visibility = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTarget(visibility, tAddContainer);
            Storyboard.SetTargetProperty(visibility, new PropertyPath(Grid.VisibilityProperty));
            visibility.KeyFrames.Add(new DiscreteObjectKeyFrame() { 
                Value = Visibility.Collapsed,
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1000))
            });

            // Slowly fade the container to completely transparent.
            DoubleAnimation opacity = new DoubleAnimation();
            Storyboard.SetTarget(opacity, tAddContainer);
            Storyboard.SetTargetProperty(opacity, new PropertyPath(Grid.OpacityProperty));
            opacity.AccelerationRatio = 1.0;
            opacity.Duration = TimeSpan.FromMilliseconds(1000);
            opacity.From = 1;
            opacity.To = 0;

            story.Children.Add(visibility);
            story.Children.Add(opacity);
            story.Begin();
        }


        /// <summary>
        /// Attempts to add the interface and close the add interface view.
        /// </summary>
        private void AddInterface_Click(Object nothing)
        {
            ExtensionApi.Commands["Interface"]["Add"].Value.Execute(
                new Object[] {
                    mProps["Host"].Value,
                    mProps["Port"].Value,
                    mProps["User"].Value,
                    mProps["Pass"].Value
                });

            mProps["Host"].Value = String.Empty;
            mProps["Port"].Value = String.Empty;
            mProps["User"].Value = String.Empty;
            mProps["Pass"].Value = String.Empty;

            mComms["Close"].Value.Execute(null);
        }

        private bool AddInterface_CanClick(Object nothing)
        {
            return ExtensionApi.Commands["Interface"]["Add"].Value != null &&
                ExtensionApi.Commands["Interface"]["Add"].Value.CanExecute(
                new Object[] {
                    mProps["Host"].Value,
                    mProps["Port"].Value,
                    mProps["User"].Value,
                    mProps["Pass"].Value
                });
        }

        /// <summary>
        /// Attempts to remove the currently selected interface.
        /// </summary>
        private void RemoveInterface_Click(InterfaceViewModel toRemove)
        {
            ExtensionApi.Commands["Interface"]["Remove"].Value.Execute(new Object[] {
                toRemove.Hostname,
                toRemove.Port.ToString()
            });
        }

        private bool RemoveInterface_CanClick(InterfaceViewModel toRemove)
        {
            return toRemove != null &&
                ExtensionApi.Commands["Interface"]["Remove"].Value != null &&
                ExtensionApi.Commands["Interface"]["Remove"].Value.CanExecute(new Object[] {
                    toRemove.Hostname,
                    toRemove.Port.ToString()
                });
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
    }
}
