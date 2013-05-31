using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Procon.UI.Default.Debug
{
    using Procon.Net.Protocols.Objects;
    using Procon.UI.API;
    using Procon.UI.API.Events;
    using Procon.UI.API.Utils;
    using Procon.UI.API.ViewModels;
    using System.IO;
    using System.Windows.Input;

    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Debug : IExtension
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


        [STAThread]
        public bool Entry(Window root)
        {
            root.PreviewKeyDown += (s, e) => {
                if ((e.KeyboardDevice.Modifiers | ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.D) {
                    Console.SetOut(new StreamWriter(File.OpenWrite("out.txt")));
                    PrintProperties(ExtensionApi.Properties);
                    PrintVisualTree(root);
                    Console.Out.Flush();
                    Console.Out.Close();
                    MessageBox.Show("Done with debug!");
                }
            };

            return true;
        }

        private void PrintProperties(ArrayDictionary<String, Object> c, int indent = 0)
        {
            foreach (var key in c.Keys) {
                Console.Write(new String(' ', indent));
                Console.Write("[{0}]", key);
                if (c[key].Value != null) Console.WriteLine(" => {0}", c[key].Value);
                else                      Console.WriteLine();
                PrintProperties(c[key], indent + 2);
            }
        }

        private void PrintVisualTree(FrameworkElement root, int indent = 0)
        {
            if (root == null)
                return;

            // Print name.
            Console.Write(new String(' ', indent));
            FrameworkElement tRoot = (root as FrameworkElement);
            if (tRoot != null && tRoot.Name != null && tRoot.Name != "") {
                Console.WriteLine("[{0}]", tRoot.Name);
            }
            else {
                Console.WriteLine("[{0}]", root.GetType().FullName);
            }

            // If it's a content control...
            ContentControl tContent = root as ContentControl;
            if (tContent != null) {
                PrintVisualTree(tContent.Content as FrameworkElement, indent + 2);
            }
            // Else, it's a normal control...
            else {
                Int32 tCount = VisualTreeHelper.GetChildrenCount(root);
                for (Int32 i = 0; i < tCount; i++) {
                    PrintVisualTree(VisualTreeHelper.GetChild(root, i) as FrameworkElement, indent + 2);
                }
            }
        }
    }
}
