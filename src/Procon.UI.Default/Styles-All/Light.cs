using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Procon.UI.API;

namespace Procon.UI.Default.Styles
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
            Grid rootLayout     = ExtensionApi.FindControl<Grid>(root, "RootLayout");
            Grid tutorialLayout = ExtensionApi.FindControl<Grid>(root, "TutorialLayout");
            Grid mainLayout     = ExtensionApi.FindControl<Grid>(root, "MainLayout");
            if  (rootLayout == null || tutorialLayout == null || mainLayout == null) return false;

            Grid headerLayout     = ExtensionApi.FindControl<Grid>(mainLayout, "MainHeader");
            Grid navigationLayout = ExtensionApi.FindControl<Grid>(mainLayout, "MainNavigation");
            if (headerLayout == null || navigationLayout == null) return false;


            // Some color values.
            String tTextColor          = "#FF231F20";

            String tControlLightColor  = "#FFF4F4F4";
            String tControlMediumColor = "#FFE6E6E6";
            String tControlDarkColor   = "#FFCDCDCD";

            String tGlowLightColor     = "#FFBEEAF2";
            String tGlowMediumColor    = "#FF96C2C9";
            String tGlowDarkColor      = "#FF6D9A9F";

            String tBlueLightColor     = "#FF00B1CE";
            String tBlueMediumColor    = "#FF00A8C9";
            String tBlueDarkColor      = "#FF0096BD";

            String tBorderLightColor   = "#FFFCFCFC";
            String tBorderMediumColor  = "#FFDFDFDF";
            String tBorderDarkColor    = "#FFA3A3A3";


            // Some brush values.
            String tControlBackgroundBrush =
                "<LinearGradientBrush StartPoint=\"0.5, 0.0\" EndPoint=\"0.5, 1.0\">" +
                    "<GradientStop Offset=\"0.0\" Color=\""+tControlLightColor+"\" />" +
                    "<GradientStop Offset=\"1.0\" Color=\""+tControlMediumColor+"\" />" +
                "</LinearGradientBrush>";
            String tControlHoverBackgroundBrush =
                "<LinearGradientBrush StartPoint=\"0.5, 0.0\" EndPoint=\"0.5, 1.0\">" +
                    "<GradientStop Offset=\"0.0\" Color=\""+tControlLightColor+"\" />" +
                    "<GradientStop Offset=\"1.0\" Color=\""+tControlDarkColor+"\" />" +
                "</LinearGradientBrush>";
            String tControlPressedBackgroundBrush =
                "<LinearGradientBrush StartPoint=\"0.5, 0.0\" EndPoint=\"0.5, 1.0\">" +
                    "<GradientStop Offset=\"0.0\" Color=\""+tControlMediumColor+"\" />" +
                    "<GradientStop Offset=\"1.0\" Color=\""+tControlDarkColor+"\" />" +
                "</LinearGradientBrush>";

            String tControlGlowBrush =
                "<LinearGradientBrush StartPoint=\"0.5, 0.0\" EndPoint=\"0.5, 1.0\">" +
                    "<GradientStop Offset=\"0.0\" Color=\""+tGlowLightColor+"\" />" +
                    "<GradientStop Offset=\"1.0\" Color=\""+tGlowMediumColor+"\" />" +
                "</LinearGradientBrush>";
            String tControlHoverGlowBrush =
                "<LinearGradientBrush StartPoint=\"0.5, 0.0\" EndPoint=\"0.5, 1.0\">" +
                    "<GradientStop Offset=\"0.0\" Color=\"" + tGlowLightColor + "\" />" +
                    "<GradientStop Offset=\"1.0\" Color=\"" + tGlowDarkColor + "\" />" +
                "</LinearGradientBrush>";
            String tControlPressedGlowBrush =
                "<LinearGradientBrush StartPoint=\"0.5, 0.0\" EndPoint=\"0.5, 1.0\">" +
                    "<GradientStop Offset=\"0.0\" Color=\"" + tGlowMediumColor + "\" />" +
                    "<GradientStop Offset=\"1.0\" Color=\"" + tGlowDarkColor + "\" />" +
                "</LinearGradientBrush>";

            String tControlBorderBrush =
                "<LinearGradientBrush StartPoint=\"0.5, 0.0\" EndPoint=\"0.5, 1.0\">" +
                    "<GradientStop Offset=\"0.0\" Color=\""+tBorderLightColor+"\" />" +
                    "<GradientStop Offset=\"1.0\" Color=\""+tBorderDarkColor+"\" />" +
                "</LinearGradientBrush>";

            String tTutorialBgBrush =
                "<RadialGradientBrush GradientOrigin=\"0.5, 0.5\" Center=\"0.5, 0.65\" RadiusX=\"0.4\" RadiusY=\"0.6\">" +
                    "<GradientStop Offset=\"0.0\" Color=\""+tControlLightColor+"\" />" +
                    "<GradientStop Offset=\"0.2\" Color=\""+tControlLightColor+"\" />" +
                    "<GradientStop Offset=\"1.0\" Color=\""+tControlDarkColor+"\" />" +
                "</RadialGradientBrush>";
            String tHeaderBgBrush =
                "<LinearGradientBrush StartPoint=\"0.5, 0.0\" EndPoint=\"0.5, 1.0\">" +
                    "<GradientStop Offset=\"0.0\" Color=\""+tBlueLightColor+"\" />" +
                    "<GradientStop Offset=\"1.0\" Color=\""+tBlueDarkColor+"\" />" +
                "</LinearGradientBrush>";
            String tNavigationBgBrush = 
                "<ImageBrush TileMode=\"Tile\" " +
                    "Viewport=\"0, 0, 10, 10\" " +
                    "ViewportUnits=\"Absolute\" " +
                        "ImageSource=\"{Binding " +
                            "Source={x:Static view:InstanceViewModel.PublicProperties}, " +
                            "Path=[Images][Background][Navigation].Value}\" " +
                "/>";


            // Universal styles.
            #region Button Style

            rootLayout.Resources.Add(typeof(Button), ExtensionApi.ParseXaml<Style>(
                "<Style x:Key=\"{x:Type Button}\" TargetType=\"{x:Type Button}\">\n" +
                    "<Setter Property=\"Margin\"                     Value=\"0\"              />\n" +
                    "<Setter Property=\"Padding\"                    Value=\"0, 10\"          />\n" +
                    "<Setter Property=\"BorderThickness\"            Value=\"1\"              />\n" +
                    "<Setter Property=\"FontSize\"                   Value=\"12.0\"           />\n" +
                    "<Setter Property=\"FontWeight\"                 Value=\"Bold\"           />\n" +
                    "<Setter Property=\"FontFamily\"                 Value=\"Arial\"          />\n" +
                    "<Setter Property=\"VerticalContentAlignment\"   Value=\"Center\"         />\n" +
                    "<Setter Property=\"HorizontalContentAlignment\" Value=\"Center\"         />\n" +
                    "<Setter Property=\"OverridesDefaultStyle\"      Value=\"True\"           />\n" +
                    "<Setter Property=\"Foreground\"                 Value=\""+tTextColor+"\" />\n" +
                    "<Setter Property=\"Background\">\n" +
                        "<Setter.Value>"+tControlBackgroundBrush+"</Setter.Value>\n" +
                    "</Setter>\n" +
                    "<Setter Property=\"BorderBrush\">\n" +
                        "<Setter.Value>"+tControlBorderBrush+"</Setter.Value>\n" +
                    "</Setter>\n" +

                    // Remove all styles related to the state of the control.
                    "<Setter Property=\"FocusVisualStyle\">\n" +
                        "<Setter.Value>\n" +
                            "<Style>\n" +
                                "<Setter Property=\"Control.Template\">\n" +
                                    "<Setter.Value>\n" +
                                        "<ControlTemplate TargetType=\"{x:Type Control}\" />\n" +
                                    "</Setter.Value>\n" +
                                "</Setter>\n" +
                            "</Style>\n" +
                        "</Setter.Value>\n" +
                    "</Setter>\n" +

                    // Replace the looks of the button.
                    "<Setter Property=\"Template\">\n" +
                        "<Setter.Value>\n" +
                            "<ControlTemplate TargetType=\"Button\">" +
                                "<Border Name=\"border\" \n" +
                                        "CornerRadius=\"2\" \n" +
                                        "BorderBrush=\"{TemplateBinding BorderBrush}\" \n" +
                                        "BorderThickness=\"{TemplateBinding BorderThickness}\" \n" +
                                        "Padding=\"{TemplateBinding Padding}\" \n" +
                                        "Background=\"{TemplateBinding Background}\">\n" +
                                    "<ContentPresenter Name=\"content\" HorizontalAlignment=\"Center\" VerticalAlignment=\"Center\" />\n" +
                                "</Border>\n" +
                                "<ControlTemplate.Triggers>\n" +
                                    "<Trigger Property=\"IsMouseOver\" Value=\"True\">\n" +
                                        "<Setter Property=\"Background\">\n" +
                                            "<Setter.Value>"+tControlHoverBackgroundBrush+"</Setter.Value>\n" +
                                        "</Setter>\n" +
                                    "</Trigger>\n" +
                                    "<Trigger Property=\"IsPressed\" Value=\"True\">\n" +
                                        "<Setter Property=\"Background\">\n" +
                                            "<Setter.Value>"+tControlPressedBackgroundBrush+"</Setter.Value>\n" +
                                        "</Setter>\n" +
                                    "</Trigger>\n" +
                                    "<Trigger Property=\"IsEnabled\" Value=\"False\">\n" +
                                        "<Setter Property=\"Opacity\"    Value=\"0.7\" TargetName=\"border\"  />\n" +
                                        "<Setter Property=\"Foreground\" Value=\"Gray\" />\n" +
                                    "</Trigger>\n" +
                                "</ControlTemplate.Triggers>\n" +
                            "</ControlTemplate>\n" +
                        "</Setter.Value>\n" +
                    "</Setter>\n" +
                "</Style>"));

            #endregion
            #region Label Style
            
            rootLayout.Resources.Add(typeof(Label), ExtensionApi.ParseXaml<Style>(
                "<Style x:Key=\"{x:Type Label}\" TargetType=\"{x:Type Label}\">\n" +
                    "<Setter Property=\"Margin\"                     Value=\"0\"              />\n" +
                    "<Setter Property=\"Padding\"                    Value=\"0, 10, 0, 8\"    />\n" +
                    "<Setter Property=\"FontSize\"                   Value=\"12.0\"           />\n" +
                    "<Setter Property=\"FontWeight\"                 Value=\"Bold\"           />\n" +
                    "<Setter Property=\"FontFamily\"                 Value=\"Arial\"          />\n" +
                    "<Setter Property=\"VerticalContentAlignment\"   Value=\"Center\"         />\n" +
                    "<Setter Property=\"HorizontalContentAlignment\" Value=\"Left\"           />\n" +
                    "<Setter Property=\"Foreground\"                 Value=\""+tTextColor+"\" />\n" +
                "</Style>"));

            #endregion
            #region TextBlock Style
            
            rootLayout.Resources.Add(typeof(TextBlock), ExtensionApi.ParseXaml<Style>(
                "<Style x:Key=\"{x:Type TextBlock}\" TargetType=\"{x:Type TextBlock}\">\n" +
                    "<Setter Property=\"Margin\"        Value=\"0\"              />\n" +
                    "<Setter Property=\"Padding\"       Value=\"0, 10\"          />\n" +
                    "<Setter Property=\"FontSize\"      Value=\"11.0\"           />\n" +
                    "<Setter Property=\"FontWeight\"    Value=\"Bold\"           />\n" +
                    "<Setter Property=\"FontFamily\"    Value=\"Arial\"          />\n" +
                    "<Setter Property=\"TextAlignment\" Value=\"Left\"           />\n" +
                    "<Setter Property=\"TextWrapping\"  Value=\"Wrap\"           />\n" +
                    "<Setter Property=\"Foreground\"    Value=\""+tTextColor+"\" />\n" +
                "</Style>"));

            #endregion
            #region TextBox Style
            
            rootLayout.Resources.Add(typeof(TextBox), ExtensionApi.ParseXaml<Style>(
                "<Style x:Key=\"{x:Type TextBox}\" TargetType=\"{x:Type TextBox}\">\n" +
                    "<Setter Property=\"Margin\"                     Value=\"0\"                     />\n" +
                    "<Setter Property=\"Padding\"                    Value=\"3\"                     />\n" +
                    "<Setter Property=\"FontSize\"                   Value=\"12.0\"                  />\n" +
                    "<Setter Property=\"FontWeight\"                 Value=\"Bold\"                  />\n" +
                    "<Setter Property=\"FontFamily\"                 Value=\"Arial\"                 />\n" +
                    "<Setter Property=\"VerticalContentAlignment\"   Value=\"Center\"                />\n" +
                    "<Setter Property=\"HorizontalContentAlignment\" Value=\"Left\"                  />\n" +
                    "<Setter Property=\"Foreground\"                 Value=\""+tTextColor+"\"        />\n" +
                    "<Setter Property=\"BorderBrush\"                Value=\""+tControlDarkColor+"\" />\n" +
                "</Style>"));

            #endregion
            #region PasswordBox Style
            
            rootLayout.Resources.Add(typeof(PasswordBox), ExtensionApi.ParseXaml<Style>(
                "<Style x:Key=\"{x:Type PasswordBox}\" TargetType=\"{x:Type PasswordBox}\">\n" +
                    "<Setter Property=\"Margin\"                     Value=\"0\"                     />\n" +
                    "<Setter Property=\"Padding\"                    Value=\"3\"                     />\n" +
                    "<Setter Property=\"FontSize\"                   Value=\"12.0\"                  />\n" +
                    "<Setter Property=\"FontWeight\"                 Value=\"Bold\"                  />\n" +
                    "<Setter Property=\"FontFamily\"                 Value=\"Arial\"                 />\n" +
                    "<Setter Property=\"VerticalContentAlignment\"   Value=\"Center\"                />\n" +
                    "<Setter Property=\"HorizontalContentAlignment\" Value=\"Left\"                  />\n" +
                    "<Setter Property=\"Foreground\"                 Value=\""+tTextColor+"\"        />\n" +
                    "<Setter Property=\"BorderBrush\"                Value=\""+tControlDarkColor+"\" />\n" +
                "</Style>"));

            #endregion
            #region Image Style
            
            rootLayout.Resources.Add(typeof(Image), ExtensionApi.ParseXaml<Style>(
                "<Style x:Key=\"{x:Type Image}\" TargetType=\"{x:Type Image}\">\n" +
                    "<Setter Property=\"Margin\"                 Value=\"0\"        />\n" +
                    "<Setter Property=\"Stretch\"                Value=\"Uniform\"  />\n" +
                    "<Setter Property=\"StretchDirection\"       Value=\"DownOnly\" />\n" +
                    "<Setter Property=\"VerticalAlignment\"      Value=\"Center\"   />\n" +
                    "<Setter Property=\"HorizontalAlignment\"    Value=\"Center\"   />\n" +
                    "<Setter Property=\"RenderOptions.EdgeMode\" Value=\"Aliased\"  />\n" +
                "</Style>"));

            #endregion


            // Header styles.
            #region Button Style

            headerLayout.Resources.Add(typeof(Button), ExtensionApi.ParseXaml<Style>(
                "<Style x:Key=\"{x:Type Button}\" TargetType=\"{x:Type Button}\" BasedOn=\"{StaticResource {x:Type Button}}\">\n" +
                    "<Setter Property=\"Padding\"         Value=\"7, 10\" />\n" +
                    "<Setter Property=\"BorderThickness\" Value=\"0\"     />\n" +
                    "<Setter Property=\"Background\">" +
                        "<Setter.Value>"+tControlGlowBrush+"</Setter.Value>" +
                    "</Setter>" +

                    // Replace the looks of the button.
                    "<Setter Property=\"Template\">\n" +
                        "<Setter.Value>\n" +
                            "<ControlTemplate TargetType=\"Button\">" +
                                "<Border Name=\"border\" \n" +
                                        "CornerRadius=\"0\" \n" +
                                        "BorderBrush=\"{TemplateBinding BorderBrush}\" \n" +
                                        "BorderThickness=\"{TemplateBinding BorderThickness}\" \n" +
                                        "Padding=\"{TemplateBinding Padding}\" \n" +
                                        "Background=\"{TemplateBinding Background}\">\n" +
                                    "<ContentPresenter Name=\"content\" HorizontalAlignment=\"Center\" VerticalAlignment=\"Center\" />\n" +
                                "</Border>\n" +
                                "<ControlTemplate.Triggers>\n" +
                                    "<Trigger Property=\"IsMouseOver\" Value=\"True\">\n" +
                                        "<Setter Property=\"Background\">\n" +
                                            "<Setter.Value>"+tControlHoverGlowBrush+"</Setter.Value>\n" +
                                        "</Setter>\n" +
                                    "</Trigger>\n" +
                                    "<Trigger Property=\"IsPressed\" Value=\"True\">\n" +
                                        "<Setter Property=\"Background\">\n" +
                                            "<Setter.Value>"+tControlPressedGlowBrush+"</Setter.Value>\n" +
                                        "</Setter>\n" +
                                    "</Trigger>\n" +
                                    "<Trigger Property=\"IsEnabled\" Value=\"False\">\n" +
                                        "<Setter Property=\"Opacity\"    Value=\"0.7\" TargetName=\"border\"  />\n" +
                                        "<Setter Property=\"Foreground\" Value=\"Gray\" />\n" +
                                    "</Trigger>\n" +
                                "</ControlTemplate.Triggers>\n" +
                            "</ControlTemplate>\n" +
                        "</Setter.Value>\n" +
                    "</Setter>\n" +
                "</Style>"));

            #endregion
            #region Label Style

            headerLayout.Resources.Add(typeof(Label), ExtensionApi.ParseXaml<Style>(
                "<Style x:Key=\"{x:Type Label}\" TargetType=\"{x:Type Label}\" BasedOn=\"{StaticResource {x:Type Label}}\">\n" +
                    "<Setter Property=\"Padding\"             Value=\"0\"      />\n" +
                    "<Setter Property=\"VerticalAlignment\"   Value=\"Center\" />\n" +
                    "<Setter Property=\"HorizontalAlignment\" Value=\"Center\" />\n" +
                "</Style>"));

            #endregion


            // Some backgrounds for specific controls.
            tutorialLayout.Background   = ExtensionApi.ParseXaml<Brush>(tTutorialBgBrush);
            mainLayout.Background       = ExtensionApi.ParseXaml<Brush>("<SolidColorBrush Color=\""+tControlLightColor+"\" />");
            headerLayout.Background     = ExtensionApi.ParseXaml<Brush>(tHeaderBgBrush);
            navigationLayout.Background = ExtensionApi.ParseXaml<Brush>(tNavigationBgBrush);

            return true;
        }
    }
}
