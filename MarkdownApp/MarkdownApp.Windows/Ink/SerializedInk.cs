using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MarkdownApp.Ink
{
    public class SerializedInkCollection
    {
        [JsonProperty("pages")]
        public List<SerializedInk> Pages = new List<SerializedInk>();

        public void Add(SerializedInk serializedInk)
        {
            Pages.Add(serializedInk);
        }
    }

    public class SerializedInk
    {
        [JsonProperty("strokes")]
        public List<SerializedStroke> Strokes = new List<SerializedStroke>();

        public void Add(SerializedStroke serializedStroke)
        {
            Strokes.Add(serializedStroke);
        }
    }

    public class SerializedStroke
    {
        [JsonProperty("segments")]
        public List<SerializedStrokeSegment> Segments = new List<SerializedStrokeSegment>();

        public void Add(SerializedStrokeSegment serializedStrokeSegment)
        {
            Segments.Add(serializedStrokeSegment);
        }
    }

    public class SerializedStrokeSegment
    {
        [JsonProperty("bezier_control_point_1")]
        public Point BezierControlPoint1 { get; set; }
        [JsonProperty("bezier_control_point_2")]
        public Point BezierControlPoint2 { get; set; }
        [JsonProperty("position")]
        public Point Position { get; set; }
        [JsonProperty("pressure")]
        public float Pressure { get; set; }
    }
}
