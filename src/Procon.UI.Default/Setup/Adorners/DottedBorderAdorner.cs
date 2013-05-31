using System;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Procon.UI.Default.Setup.Adorners
{
    class DottedBorderAdorner : Adorner
    {
        public DottedBorderAdorner(UIElement adornedElement) : base(adornedElement)
        {
            SetResourceReference(DottedBorderAdorner.StrokeProperty, "BrushControlBorder");
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(
                "Stroke",
                typeof(Brush),
                typeof(DottedBorderAdorner),
                new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(
                "StrokeThickness",
                typeof(Double),
                typeof(DottedBorderAdorner),
                new FrameworkPropertyMetadata(2.0));
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register(
                "StrokeDashArray",
                typeof(DoubleCollection),
                typeof(DottedBorderAdorner),
                new FrameworkPropertyMetadata(new DoubleCollection(new Double[] { 2, 2 })));
        public static readonly DependencyProperty StrokeDashCapProperty =
            DependencyProperty.Register(
                "StrokeDashCap",
                typeof(PenLineCap),
                typeof(DottedBorderAdorner),
                new FrameworkPropertyMetadata(PenLineCap.Flat));
        public static readonly DependencyProperty XRadiusProperty =
            DependencyProperty.Register(
                "XRadius",
                typeof(Double),
                typeof(DottedBorderAdorner),
                new FrameworkPropertyMetadata(1.0));
        public static readonly DependencyProperty YRadiusProperty =
            DependencyProperty.Register(
                "YXRadius",
                typeof(Double),
                typeof(DottedBorderAdorner),
                new FrameworkPropertyMetadata(1.0));

        public Brush Stroke
        {
            get { return (Brush)this.GetValue(StrokeProperty); }
            set { this.SetValue(StrokeProperty, value); }
        }
        public Double StrokeThickness
        {
            get { return (Double)this.GetValue(StrokeThicknessProperty); }
            set { this.SetValue(StrokeThicknessProperty, value); }
        }
        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)this.GetValue(StrokeDashArrayProperty); }
            set { this.SetValue(StrokeDashArrayProperty, value); }
        }
        public PenLineCap StrokeDashCap
        {
            get { return (PenLineCap)this.GetValue(StrokeDashCapProperty); }
            set { this.SetValue(StrokeDashCapProperty, value); }
        }
        public Double XRadius
        {
            get { return (Double)this.GetValue(XRadiusProperty); }
            set { this.SetValue(XRadiusProperty, value); }
        }
        public Double YRadius
        {
            get { return (Double)this.GetValue(YRadiusProperty); }
            set { this.SetValue(YRadiusProperty, value); }
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // Draw rounded corners first.
            Size   tSize      = AdornedElement.RenderSize;
            Double tDash      = StrokeDashArray[0];
            Double tThickness = StrokeThickness;

            // Top left
            DrawCorner(drawingContext,
                0, XRadius, XRadius + tDash,
                0, YRadius, YRadius + tDash,
                tThickness, tThickness);
            // Top Right
            DrawCorner(drawingContext,
                tSize.Width, tSize.Width - XRadius, tSize.Width - XRadius - tDash,
                0, YRadius, YRadius + tDash,
                -tThickness, tThickness);
            // Bottom Right
            DrawCorner(drawingContext,
                tSize.Width, tSize.Width - XRadius, tSize.Width - XRadius - tDash,
                tSize.Height, tSize.Height - YRadius, tSize.Height - YRadius - tDash,
                -tThickness, -tThickness);
            // Bottom Left
            DrawCorner(drawingContext,
                0, XRadius, XRadius + tDash,
                tSize.Height, tSize.Height - YRadius, tSize.Height - YRadius - tDash,
                tThickness, -tThickness);

            // Draw the lines between the corners.
            Double tSpace = StrokeDashArray[1];

            // Top
            DrawLine(drawingContext,
                 XRadius + tDash + tSpace * tThickness,
                -XRadius - tDash - tSpace * tThickness + tSize.Width,
                tThickness / 2,
                tThickness / 2);
            // Bottom
            DrawLine(drawingContext,
                 XRadius + tDash + tSpace * tThickness,
                -XRadius - tDash - tSpace * tThickness + tSize.Width,
                tSize.Height - tThickness / 2,
                tSize.Height - tThickness / 2);
            // Left
            DrawLine(drawingContext,
                tThickness / 2,
                tThickness / 2,
                 YRadius + tDash + tSpace * tThickness,
                -YRadius - tDash - tSpace * tThickness + tSize.Height);
            // Right
            DrawLine(drawingContext,
                tSize.Width - tThickness / 2,
                tSize.Width - tThickness / 2,
                 YRadius + tDash + tSpace * tThickness,
                -YRadius - tDash - tSpace * tThickness + tSize.Height);
        }

        private void DrawCorner(
            DrawingContext dc,
            Double x1, Double x2, Double x3,
            Double y1, Double y2, Double y3,
            Double xThick, Double yThick)
        {
            StringBuilder tGeom = new StringBuilder();

            // Ex: Top Left Corner:
            // Move to left most x, bottom most y.
            // Move up to bottom edge of corner.
            tGeom.AppendFormat("M {0},{1} ", x1, y3);
            tGeom.AppendFormat("V {0} ",         y2);

            // Curve around to top most right edge of corner.
            // Move to right most x.
            tGeom.AppendFormat("Q {0},{1} {2},{3} ", x1, y1, x2, y1);
            tGeom.AppendFormat("H {0} ",                     x3);

            // Move down the thickness of the pen.
            // Move back to right edge of corner.
            tGeom.AppendFormat("V {0} ", y1 + yThick);
            tGeom.AppendFormat("H {0} ", x2);

            // Curve back around to inside bottom edge of corner.
            // Move back down to bottom most y.
            tGeom.AppendFormat("Q {0},{1} {2},{3} ", x1 + xThick, y1 + yThick, x1 + xThick, y2);
            tGeom.AppendFormat("V {0} ", y3);

            // Connect the bottom most y's.
            tGeom.AppendFormat("Z");

            dc.DrawGeometry(Stroke, new Pen(), Geometry.Parse(tGeom.ToString()));
        }

        private void DrawLine(
            DrawingContext dc,
            Double x1, Double x2,
            Double y1, Double y2)
        {
            Pen tPen = new Pen(Stroke, StrokeThickness);
            tPen.DashCap = StrokeDashCap;
            tPen.DashStyle = new DashStyle(StrokeDashArray, StrokeDashArray[0] + StrokeDashArray[1] / 2);

            // Draw from the middle instead of from end-to-end.
            Point tStart = new Point(x1, y1);
            Point tStop  = new Point(x2, y2);
            Point tMid   = new Point((x2 - x1) / 2 + x1, (y2 - y1) / 2 + y1);
            dc.DrawLine(tPen, new Point(tMid.X, tMid.Y), tStart);
            dc.DrawLine(tPen, new Point(tMid.X, tMid.Y), tStop);
        }
    }
}
