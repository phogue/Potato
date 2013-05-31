using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Procon.UI
{
    using Procon.Core;
    using Procon.UI.API;
    using Procon.UI.API.ViewModels;

    // Builds UI by loading in extensions at runtime.
    // Manage which extensions get loaded by running the Ui Manager.
    internal class Entry
    {
        [STAThread]
        static void Main(String[] args)
        {
            // Start Procon and load the settings file.
            InstanceViewModel tProcon = new InstanceViewModel(new Instance());
            ExtensionApi.Properties["Procon"].Value = tProcon;
            tProcon.Execute();
            Settings.Load();


            // Create the root element of the UI.
            Window tRoot = new Window() {
                Name  = "Root",
                Title = "Procon 2",
                Top    = Settings.Get<Double>("Top", Double.NaN),
                Left   = Settings.Get<Double>("Left", Double.NaN),
                Width  = Settings.Get<Double>("Width",  1024),
                Height = Settings.Get<Double>("Height", 768),
                MinWidth    = 1024,
                MinHeight   = 768,
                DataContext = tProcon,
                UseLayoutRounding   = true,
                SnapsToDevicePixels = true,
                WindowState           = Settings.Get<WindowState>("WindowState", WindowState.Normal),
                WindowStartupLocation = WindowStartupLocation.Manual
            };
            if (File.Exists(Defines.PROCON_ICON)) {
                tRoot.Icon = new BitmapImage(new Uri(Defines.PROCON_ICON, UriKind.RelativeOrAbsolute));
            }
            TextOptions.SetTextFormattingMode(tRoot, TextFormattingMode.Display);


            tRoot.Closing +=
            #region -- Save Window Settings --

            (s, e) => {
                Settings.Set("Top",         tRoot.Top);
                Settings.Set("Left",        tRoot.Left);
                Settings.Set("Width",       tRoot.Width);
                Settings.Set("Height",      tRoot.Height);
                Settings.Set("WindowState", tRoot.WindowState);
            };

            #endregion
            tRoot.SourceInitialized +=
            #region -- Fix broken MinWidth and MinHeight settings --

            (s, e) => {
                HwndSource tSource = (HwndSource)HwndSource.FromVisual((Window)s);

                // Grab into the message pump for this window.
                tSource.AddHook((IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, ref Boolean handled) => {

                    switch (msg) {
                        case /* WINDOWPOSCHANGING */ 0x0046:
                            WindowPos tPos = (WindowPos)Marshal.PtrToStructure(lParam, typeof(WindowPos));

                            // Quick exit if we don't allow resizing.
                            if ((tPos.flags & /* NOMOVE */ 0x0002) != 0) {
                                return IntPtr.Zero;
                            }

                            // Quick exit if window is invalid.
                            Window tWnd = (Window)HwndSource.FromHwnd(hwnd).RootVisual;
                            if (tWnd == null) {
                                return IntPtr.Zero;
                            }

                            // Not really accurate, but will force a min width and min height.
                            Boolean tChanged = false;
                            if (tPos.cx < tWnd.MinWidth) {
                                tPos.cx = (Int32)tWnd.MinWidth;
                                tChanged = true;
                            }
                            if (tPos.cy < tWnd.MinHeight) {
                                tPos.cy = (Int32)tWnd.MinHeight;
                                tChanged = true;
                            }

                            // Exit if there wasn't a change.
                            if (!tChanged) {
                                return IntPtr.Zero;
                            }

                            // Send back the updated window size and don't propagate the message.
                            Marshal.StructureToPtr(tPos, lParam, true);
                            handled = true;
                            break;
                    }
                    return IntPtr.Zero;
                });
            };

            #endregion


            ExtensionApi.Properties["Interface"].PropertyChanged +=
            #region -- Interface Changed --

            (s, e) => {
                ExtensionApi.Settings["InterfaceType"].Value = null;
                ExtensionApi.Settings["InterfaceHost"].Value = null;
                ExtensionApi.Settings["InterfacePort"].Value = null;

                if (ExtensionApi.Interface != null) {
                    ExtensionApi.Settings["InterfaceType"].Value = ExtensionApi.Interface.IsLocal;
                    ExtensionApi.Settings["InterfaceHost"].Value = ExtensionApi.Interface.Hostname;
                    ExtensionApi.Settings["InterfacePort"].Value = ExtensionApi.Interface.Port;
                }

                ExtensionApi.Properties["Interface"]["Valid"].Value = ExtensionApi.Connection != null;
            };

            #endregion
            ExtensionApi.Properties["Connection"].PropertyChanged +=
            #region -- Connection Changed --

            (s, e) => {
                ExtensionApi.Settings["ConnectionType"].Value = null;
                ExtensionApi.Settings["ConnectionHost"].Value = null;
                ExtensionApi.Settings["ConnectionPort"].Value = null;

                if (ExtensionApi.Connection != null) {
                    ExtensionApi.Settings["ConnectionType"].Value = ExtensionApi.Connection.GameType;
                    ExtensionApi.Settings["ConnectionHost"].Value = ExtensionApi.Connection.Hostname;
                    ExtensionApi.Settings["ConnectionPort"].Value = ExtensionApi.Connection.Port;
                }

                ExtensionApi.Properties["Connection"]["Valid"].Value = ExtensionApi.Connection != null;
            };

            #endregion


            // Load the extensions, show the window, then save the settings.
            Settings.ExecuteExtensions(tRoot);
            tRoot.ShowDialog();
            Settings.Save();


            // Shutdown Procon.
            tProcon.Shutdown();
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct WindowPos
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public Int32 x;
            public Int32 y;
            public Int32 cx;
            public Int32 cy;
            public Int32 flags;
        }
    }
}
