using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Procon.UI.Default.Setup.Controls
{
    public partial class Banner : UserControl
    {
        public Banner()
        {
            InitializeComponent();
            FontSize = 26;
            SetResourceReference(Banner.HighlightProperty, "BrushHighlight2");
            SetResourceReference(Banner.ForegroundProperty, "BrushTextDarkSoft");
        }

        public static readonly DependencyProperty HighlightProperty =
            DependencyProperty.Register("Highlight",
                typeof(SolidColorBrush),
                typeof(Banner),
                new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius",
                typeof(Thickness),
                typeof(Banner),
                new FrameworkPropertyMetadata(null));

        public SolidColorBrush Highlight
        {
            get { return (SolidColorBrush)this.GetValue(HighlightProperty); }
            set { this.SetValue(HighlightProperty, value); }
        }
        public Thickness CornerRadius
        {
            get { return (Thickness)this.GetValue(CornerRadiusProperty); }
            set { this.SetValue(CornerRadiusProperty, value); }
        }
    }
}
