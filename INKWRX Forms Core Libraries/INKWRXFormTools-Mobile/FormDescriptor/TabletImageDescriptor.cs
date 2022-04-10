using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class TabletImageDescriptor : FieldDescriptor
    {
        private const string RADIUS_X = "rx";
        private const string RADIUS_Y = "ry";
        private const string RECTANGLE = "shapeRect";
        private const string ROUNDED_RECTANGLE = "shapeRectRounded";
        private const string FILL = "fill";
        private const string STROKE_WIDTH = "stroke-width";

        public RectElement RectElement { get; set; }
        public Point Radius = new Point { X = 0, Y = 0 };
        public double StrokeWidth { get; set; }

        public TabletImageDescriptor (XElement element, int zOrder) : base (element, zOrder)
        {
            RectElement = new RectElement(element);
            FdtFieldName = FieldId;
            foreach (var att in element.Attributes().ToList())
            {
                switch (att.Name.ToString())
                {
                    case STROKE_WIDTH:
                        StrokeWidth = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        break;
                    case RADIUS_X:
                        Radius.X = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        break;
                    case RADIUS_Y:
                        Radius.Y = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        break;
                    case STROKE:
                        StrokeColour = ColourFromString(att.Value, BlackColour);
                        break;
                }
            }
        }

        public TabletImageDescriptor (TabletImageDescriptor original) : base(original)
        {
            StrokeWidth = original.StrokeWidth;
            Radius = original.Radius;
            RectElement = original.RectElement;
        }
    }
}
