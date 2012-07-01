using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Events;
using Procon.UI.API.Utils;

namespace Procon.UI.Default.Root.Main.Connection.Chat
{
    [Extension(
        Alters    = new String[] { "MainConnectionLayout" },
        Replaces  = new String[] { },
        DependsOn = new String[] { "Main Connection Layout" })]
    public class Chat : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Main Connection Chat"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // An easy accessor for Properties and Commands of this control.
        private ArrayDictionary<String, Object>   tProps = ExtensionApi.Properties["Main"]["Connection"]["Chat"];
        private ArrayDictionary<String, ICommand> tCmmds = ExtensionApi.Commands["Main"]["Connection"]["Chat"];


        [STAThread]
        public bool Entry(Window root)
        {
            // Find the controls I want to use and check for issues.
            Grid layout = ExtensionApi.FindControl<Grid>(root, "MainConnectionLayout");
            if (layout == null) return false;

            // Do what I need to setup my control.
            ChatView     view = new ChatView();
            GridSplitter splt = new GridSplitter();
            Grid.SetRow(view, 2);
            Grid.SetRow(splt, 2);
            splt.Height = 10;
            splt.Background = Brushes.Transparent;
            splt.ResizeBehavior = GridResizeBehavior.PreviousAndCurrent;
            splt.VerticalAlignment = VerticalAlignment.Top;
            splt.HorizontalAlignment = HorizontalAlignment.Stretch;
            layout.Children.Add(view);
            layout.Children.Add(splt);

            // Load/Save the chat box height setting (unable to bind to it).
            if (ExtensionApi.Settings["ChatHeight"].Value != null) {
                if (layout.RowDefinitions.Count > 2)
                    layout.RowDefinitions[2].Height = (GridLength)ExtensionApi.Settings["ChatHeight"].Value;
            }
            root.Closing +=
                (s, e) => {
                    if (layout.RowDefinitions.Count > 2)
                        ExtensionApi.Settings["ChatHeight"].Value = layout.RowDefinitions[2].Height;
                };
            
            // Commands.
            GridLength tHeight = new GridLength(Double.MaxValue);
            tCmmds["MinMax"].Value = new RelayCommand<AttachedCommandArgs>(
            #region  -- Handles when the chat box title is clicked.
                x => {
                    RowDefinition tRowDef = layout.RowDefinitions.Count > 2 ? layout.RowDefinitions[2] : null;
                    if (tRowDef != null) {
                        if (tRowDef.Height.Value != tRowDef.MinHeight) {
                            tHeight = tRowDef.Height;
                            tRowDef.Height = new GridLength(tRowDef.MinHeight);
                        }
                        else tRowDef.Height = tHeight;
                    }
                });
            #endregion

            // Used to manage the chat list.
            ObservableCollection<Event>         tEvents      = null;
            NotifyCollectionChangedEventHandler tEventAdded  = null;
            PropertyChangedEventHandler         tResetEvents = null;

            // Manage the events's collection.
            tEventAdded =
            #region -- Handles updates due to an event being added.
                (s, e) => {
                    // Scroll to the bottom if we're at the bottom.
                    if (e.Action == NotifyCollectionChangedAction.Add) {
                        if (tEvents.Count > 1) {
                            new Thread(x => {
                                Thread.Sleep(100);
                                view.Dispatcher.Invoke(() => {
                                    view.ConnectionChatEvents.ScrollIntoView(tEvents.Last());
                                });
                            }).Start();
                        }
                    }
                };
            #endregion
            tResetEvents = 
            #region -- Detaches the old list, re-creates the list correctly sorted, retaches the new list.
                (s, e) => {
                    // Cleanup old stuff.
                    if (tEvents != null) {
                        tEvents.CollectionChanged -= tEventAdded;
                        tEvents = null;
                    }
                    // Bind to new list of events.
                    if (ExtensionApi.Connection != null) {
                        tEvents = ExtensionApi.Connection.Events;
                        tEvents.CollectionChanged += tEventAdded;
                    }
                };
            #endregion

            // Let the managing begin.
            ExtensionApi.Properties["Connection"].PropertyChanged += tResetEvents;

            // Exit with good status.
            return true;
        }
    }
}
