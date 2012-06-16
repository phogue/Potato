using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Procon.UI.Default.Controls
{
    public partial class RadioImageButton : RadioButton
    {
        public static readonly DependencyProperty NormalImageProperty =
            DependencyProperty.Register(
                "NormalImage",
                typeof(ImageSource),
                typeof(RadioImageButton),
                new UIPropertyMetadata(null));
        public static readonly DependencyProperty HoverImageProperty =
            DependencyProperty.Register(
                "HoverImage",
                typeof(ImageSource),
                typeof(RadioImageButton),
                new UIPropertyMetadata(null));
        public static readonly DependencyProperty PressedImageProperty =
            DependencyProperty.Register(
                "PressedImage",
                typeof(ImageSource),
                typeof(RadioImageButton),
                new UIPropertyMetadata(null));
        public static readonly DependencyProperty ActiveImageProperty =
            DependencyProperty.Register(
                "ActiveImage",
                typeof(ImageSource),
                typeof(RadioImageButton),
                new UIPropertyMetadata(null));
        public static readonly DependencyProperty DisabledImageProperty =
            DependencyProperty.Register(
                "DisabledImage",
                typeof(ImageSource),
                typeof(RadioImageButton),
                new UIPropertyMetadata(null));
        
        public ImageSource NormalImage {
            get { return (ImageSource)GetValue(NormalImageProperty); }
            set { SetValue(NormalImageProperty, value); }
        }
        public ImageSource HoverImage {
            get { return (ImageSource)GetValue(HoverImageProperty); }
            set { SetValue(HoverImageProperty, value); }
        }
        public ImageSource PressedImage {
            get { return (ImageSource)GetValue(PressedImageProperty); }
            set { SetValue(PressedImageProperty, value); }
        }
        public ImageSource ActiveImage {
            get { return (ImageSource)GetValue(ActiveImageProperty); }
            set { SetValue(ActiveImageProperty, value); }
        }
        public ImageSource DisabledImage {
            get { return (ImageSource)GetValue(DisabledImageProperty); }
            set { SetValue(DisabledImageProperty, value); }
        }
        public RadioImageButton() { InitializeComponent(); }
    }
}
