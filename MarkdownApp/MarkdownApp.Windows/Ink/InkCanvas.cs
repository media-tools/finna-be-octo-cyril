using Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace MarkdownApp.Ink
{
    [TemplatePart(Name = "Canvas", Type = typeof(Canvas))]
    public sealed class InkCanvas : Control
    {
        // the InkManager instance
        InkManager _inkManager = new InkManager();
        // the SerializedInk instance
        SerializedInk _serializedInk = new SerializedInk();

        // internal variables
        Point _previousContactPt;
        uint _penID = 0;
        uint _touchID = 0;

        // the canvas
        private Canvas part_Canvas;

        // singleton
        public static List<InkCanvas> Instances = new List<InkCanvas>();
       
        public int PageNumber { get { return (int)GetValue(PageNumberProperty); } set { SetValue(PageNumberProperty, value); } }

        public Color StrokeColor { get { return (Color)GetValue(StrokeColorProperty); } set { SetValue(StrokeColorProperty, value); } }

        public int StrokeThickness { get { return (int)GetValue(StrokeThicknessProperty); } set { SetValue(StrokeThicknessProperty, value); } }

        public SerializedInk PreloadedInk { get { return (SerializedInk)GetValue(PreloadedInkProperty); } set { SetValue(PreloadedInkProperty, value); } }

        // Using a DependencyProperty as the backing store for PageNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageNumberProperty = DependencyProperty.Register("PageNumber", typeof(int), typeof(InkCanvas), new PropertyMetadata(1, PropertyChangedCallback));
        public static readonly DependencyProperty StrokeColorProperty = DependencyProperty.Register("StrokeColor", typeof(Color), typeof(InkCanvas), new PropertyMetadata(1, PropertyChangedCallback));
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(int), typeof(InkCanvas), new PropertyMetadata(1, PropertyChangedCallback));
        public static readonly DependencyProperty PreloadedInkProperty = DependencyProperty.Register("PreloadedInk", typeof(SerializedInk), typeof(InkCanvas), new PropertyMetadata(1, PreloadedInkCallback));


        private static void PreloadedInkCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InkCanvas instance = d as InkCanvas;
            SerializedInk ink = e.NewValue as SerializedInk;
            if (instance == null)
            {
                Log.Error("Error! PreloadedInkCallback: instance == null");
                return;
            }
            if (ink == null)
            {
                Log.Error("Error! PreloadedInkCallback: ink == null");
                return;
            }

            // load the strokes
            instance.Load(ink);
            // render the strokes, if the canvas is initialized
            if (instance.part_Canvas != null)
            {
                instance.RenderAllStrokes();
            }
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public InkCanvas()
        {
            this.DefaultStyleKey = typeof(InkCanvas);

            // default values
            StrokeThickness = 5;
            StrokeColor = Colors.Red;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Code to get the Template parts as instance member
            part_Canvas = GetTemplateChild("part_Canvas") as Canvas;

            if (part_Canvas == null)
            {
                throw new NullReferenceException("Template part not available: part_Canvas");
            }

            // render the strokes
            RenderAllStrokes();

            // Add pointer event handlers to the Canvas object.
            part_Canvas.PointerPressed += new PointerEventHandler(Canvas_PointerPressed);
            part_Canvas.PointerMoved += new PointerEventHandler(Canvas_PointerMoved);
            part_Canvas.PointerReleased += new PointerEventHandler(Canvas_PointerReleased);
            part_Canvas.PointerExited += new PointerEventHandler(Canvas_PointerReleased);

            // add this instance to the singleton data structure
            Instances.Add(this);
        }

        // Initiate ink capture.
        public void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Get information about the pointer location.
            PointerPoint pt = e.GetCurrentPoint(part_Canvas);
            _previousContactPt = pt.Position;

            // Accept input only from a pen or mouse with the left button pressed. 
            PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;
            if (pointerDevType == PointerDeviceType.Pen ||
                    pointerDevType == PointerDeviceType.Mouse &&
                    pt.Properties.IsLeftButtonPressed)
            {
                // Pass the pointer information to the InkManager.
                _inkManager.ProcessPointerDown(pt);
                _penID = pt.PointerId;

                e.Handled = true;
            }

            else if (pointerDevType == PointerDeviceType.Touch)
            {
                // Process touch input
            }
        }


        // Draw on the canvas and capture ink data as the pointer moves.
        public void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerId == _penID)
            {
                PointerPoint pt = e.GetCurrentPoint(part_Canvas);

                // Render a red line on the canvas as the pointer moves. 
                // Distance() is an application-defined function that tests
                // whether the pointer has moved far enough to justify 
                // drawing a new line.
                Point currentContactPt = pt.Position;
                if (Distance(currentContactPt, _previousContactPt) > 2)
                {
                    Line line = new Line()
                    {
                        X1 = _previousContactPt.X,
                        Y1 = _previousContactPt.Y,
                        X2 = currentContactPt.X,
                        Y2 = currentContactPt.Y,
                        StrokeThickness = StrokeThickness,
                        Stroke = new SolidColorBrush(Windows.UI.Colors.Red)
                    };

                    _previousContactPt = currentContactPt;

                    // Draw the line on the canvas by adding the Line object as
                    // a child of the Canvas object.
                    part_Canvas.Children.Add(line);

                    // Pass the pointer information to the InkManager.
                    _inkManager.ProcessPointerUpdate(pt);
                }
            }

            else if (e.Pointer.PointerId == _touchID)
            {
                // Process touch input
            }

            e.Handled = true;
        }

        // Finish capturing ink data and use it to render ink strokes on 
        // the canvas. 
        public void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerId == _penID)
            {
                PointerPoint pt = e.GetCurrentPoint(part_Canvas);

                // Pass the pointer information to the InkManager. 
                _inkManager.ProcessPointerUp(pt);
            }

            else if (e.Pointer.PointerId == _touchID)
            {
                // Process touch input
            }

            _touchID = 0;
            _penID = 0;

            // Call an application-defined function to render the ink strokes.
            RenderAllStrokes();

            e.Handled = true;
        }

        // Render ink strokes as cubic bezier segments.
        public void RenderAllStrokes()
        {
            // Clear the canvas.
            part_Canvas.Children.Clear();

            // Get the InkStroke objects.
            IReadOnlyList<InkStroke> inkStrokes = _inkManager.GetStrokes();

            // Process each stroke.
            foreach (InkStroke inkStroke in inkStrokes)
            {
                PathGeometry pathGeometry = new PathGeometry();
                PathFigureCollection pathFigures = new PathFigureCollection();
                PathFigure pathFigure = new PathFigure();
                PathSegmentCollection pathSegments = new PathSegmentCollection();

                // Create a path and define its attributes.
                Windows.UI.Xaml.Shapes.Path path = new Windows.UI.Xaml.Shapes.Path();
                path.Stroke = new SolidColorBrush(StrokeColor);
                path.StrokeThickness = StrokeThickness;

                // Get the stroke segments.
                IReadOnlyList<InkStrokeRenderingSegment> segments;
                segments = inkStroke.GetRenderingSegments();

                // Process each stroke segment.
                bool first = true;
                foreach (InkStrokeRenderingSegment segment in segments)
                {
                    // The first segment is the starting point for the path.
                    if (first)
                    {
                        pathFigure.StartPoint = segment.BezierControlPoint1;
                        first = false;
                    }

                    // Copy each ink segment into a bezier segment.
                    BezierSegment bezSegment = new BezierSegment();
                    bezSegment.Point1 = segment.BezierControlPoint1;
                    bezSegment.Point2 = segment.BezierControlPoint2;
                    bezSegment.Point3 = segment.Position;

                    // Add the bezier segment to the path.
                    pathSegments.Add(bezSegment);
                }

                // Build the path geometerty object.
                pathFigure.Segments = pathSegments;
                pathFigures.Add(pathFigure);
                pathGeometry.Figures = pathFigures;

                // Assign the path geometry object as the path data.
                path.Data = pathGeometry;

                // Render the path by adding it as a child of the Canvas object.
                part_Canvas.Children.Add(path);
            }

            foreach (SerializedStroke inkStroke in _serializedInk.Strokes)
            {
                PathGeometry pathGeometry = new PathGeometry();
                PathFigureCollection pathFigures = new PathFigureCollection();
                PathFigure pathFigure = new PathFigure();
                PathSegmentCollection pathSegments = new PathSegmentCollection();

                // Create a path and define its attributes.
                Windows.UI.Xaml.Shapes.Path path = new Windows.UI.Xaml.Shapes.Path();
                path.Stroke = new SolidColorBrush(StrokeColor);
                path.StrokeThickness = StrokeThickness;

                // Get the stroke segments.
                IEnumerable<SerializedStrokeSegment> segments;
                segments = inkStroke.Segments;

                // Process each stroke segment.
                bool first = true;
                foreach (SerializedStrokeSegment segment in segments)
                {
                    // The first segment is the starting point for the path.
                    if (first)
                    {
                        pathFigure.StartPoint = segment.BezierControlPoint1;
                        first = false;
                    }

                    // Copy each ink segment into a bezier segment.
                    BezierSegment bezSegment = new BezierSegment();
                    bezSegment.Point1 = segment.BezierControlPoint1;
                    bezSegment.Point2 = segment.BezierControlPoint2;
                    bezSegment.Point3 = segment.Position;

                    // Add the bezier segment to the path.
                    pathSegments.Add(bezSegment);
                }

                // Build the path geometerty object.
                pathFigure.Segments = pathSegments;
                pathFigures.Add(pathFigure);
                pathGeometry.Figures = pathFigures;

                // Assign the path geometry object as the path data.
                path.Data = pathGeometry;

                // Render the path by adding it as a child of the Canvas object.
                part_Canvas.Children.Add(path);
            }
        }

        private double Distance(Point currentContact, Point previousContact)
        {
            return Math.Sqrt(Math.Pow(currentContact.X - previousContact.X, 2) + Math.Pow(currentContact.Y - previousContact.Y, 2));
        }

        public void Load(SerializedInk serializedInk)
        {
            _inkManager = new InkManager();
            _serializedInk = serializedInk;
        }

        public SerializedInk Save()
        {
            // Get the InkStroke objects.
            IReadOnlyList<InkStroke> inkStrokes = _inkManager.GetStrokes();

            SerializedInk result = new SerializedInk();

            foreach (SerializedStroke inkStroke in _serializedInk.Strokes) {
                result.Add(inkStroke);
            }

            // Process each stroke.
            foreach (InkStroke inkStroke in inkStrokes)
            {
                SerializedStroke stroke = new SerializedStroke();

                // Get the stroke segments.
                IReadOnlyList<InkStrokeRenderingSegment> segments;
                segments = inkStroke.GetRenderingSegments();

                // Process each stroke segment.
                foreach (InkStrokeRenderingSegment segment in segments)
                {
                    // Copy each ink segment into a bezier segment.
                    BezierSegment bezSegment = new BezierSegment();
                    bezSegment.Point1 = segment.BezierControlPoint1;
                    bezSegment.Point2 = segment.BezierControlPoint2;
                    bezSegment.Point3 = segment.Position;

                    // Add the bezier segment to the path.
                    stroke.Add(new SerializedStrokeSegment
                    {
                        BezierControlPoint1 = segment.BezierControlPoint1,
                        BezierControlPoint2 = segment.BezierControlPoint2,
                        Position = segment.Position,
                        Pressure = segment.Pressure,
                    });
                }

                result.Add(stroke);
            }

            return result;
        }
    }
}
