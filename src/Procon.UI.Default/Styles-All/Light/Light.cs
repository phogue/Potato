using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using Procon.Net.Protocols.Objects;
using Procon.UI.API;
using Procon.UI.API.Events;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default.Styles.Light
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Light : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Light Style"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        [STAThread]
        public bool Entry(Window root)
        {
            // Load our resources.
            Grid rootLayout = ExtensionApi.FindControl<Grid>(root,       "RootLayout");
            Grid mainLayout = ExtensionApi.FindControl<Grid>(rootLayout, "MainLayout");
            Grid tutoLayout = ExtensionApi.FindControl<Grid>(rootLayout, "TutorialLayout");
            ResourceDictionary tResources = new ResourceDictionary() {
                Source = new Uri("pack://application:,,,/Procon.UI.Default;component/Styles-All/Light/Light.xaml")
            };

            // Setup colors.
            foreach (String key in tResources.Keys)
                if (tResources[key] is Color)
                    rootLayout.Resources[key] = tResources[key];

            // Setup brushes.
            foreach (String key in tResources.Keys)
                if (tResources[key] is Brush)
                    rootLayout.Resources[key] = tResources[key];

            // Setup base styles.
            rootLayout.Resources[typeof(Button)]      = tResources["StyleButton"];
            rootLayout.Resources[typeof(RadioButton)] = tResources["StyleRadioButton"];
            rootLayout.Resources[typeof(TextBox)]     = tResources["StyleTextBox"];
            rootLayout.Resources[typeof(PasswordBox)] = tResources["StylePasswordBox"];
            rootLayout.Resources[typeof(Label)]       = tResources["StyleLabel"];
            rootLayout.Resources[typeof(TextBlock)]   = tResources["StyleTextBlock"];
            rootLayout.Resources[typeof(ListBoxItem)] = tResources["StyleListBoxItem"];
            rootLayout.Resources[typeof(ListBox)]     = tResources["StyleListBox"];
            rootLayout.Resources[typeof(ComboBox)]    = tResources["StyleComboBox"];
            rootLayout.Resources[typeof(ScrollBar)]   = tResources["StyleScrollBar"];
            rootLayout.Resources[typeof(Image)]       = tResources["StyleImage"];

            // Setup refined styles.
            mainLayout.Resources[typeof(Button)] = tResources["StyleButtonDefault"];
            tutoLayout.Resources[typeof(Button)] = tResources["StyleButtonDefault"];

            // Setup optional styles.
            rootLayout.Resources["StyleRadioButtonImage"] = tResources["StyleRadioButtonImage"];
            rootLayout.Resources["StyleListBoxItemColor"] = tResources["StyleListBoxItemColor"];
            rootLayout.Resources["StyleListBoxItemChat"]  = tResources["StyleListBoxItemChat"];
            rootLayout.Resources["StyleComboBoxHeader"]   = tResources["StyleComboBoxHeader"];

            // Setup templates.
            rootLayout.Resources[new DataTemplateKey(typeof(ConnectionViewModel))] = tResources["DataTemplateConnectionViewModel"];
            rootLayout.Resources[new DataTemplateKey(typeof(InterfaceViewModel))]  = tResources["DataTemplateInterfaceViewModel"];
            rootLayout.Resources[new DataTemplateKey(typeof(Event))]               = tResources["DataTemplateEvent"];
            rootLayout.Resources[new DataTemplateKey(typeof(ChatEvent))]           = tResources["DataTemplateChatEvent"];
            rootLayout.Resources[new DataTemplateKey(typeof(Player))]              = tResources["DataTemplatePlayer"];

            return true;
        }
    }
}
