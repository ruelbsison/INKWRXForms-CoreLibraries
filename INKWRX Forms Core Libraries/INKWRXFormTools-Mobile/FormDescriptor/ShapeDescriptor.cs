using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class ShapeDescriptor : ElementDescriptor
    {
        internal const string RECTANGLE = "shapeRect";
        internal const string ROUNDED_RECTANGLE = "shapeRectRounded";
        internal const string FILL = "fill";
        internal const string FILLOPACITY = "fill-opacity";
        internal const string STROKE_WIDTH = "stroke-width";

        public int StrokeWidth;

        public ShapeDescriptor (XElement element, int zOrder) : base(element, zOrder)
        {
            foreach (var att in element.Attributes().ToList())
            {
                switch (att.Name.ToString())
                {
                    case STROKE_WIDTH:
                        StrokeWidth = int.Parse(att.Value);
                        break;
                    case STROKE:
                        StrokeColour = ColourFromString(att.Value, BlackColour);
                        break;
                }
            }
        }

        public ShapeDescriptor (ShapeDescriptor original) : base(original)
        {
            StrokeWidth = original.StrokeWidth;
            StrokeColour = original.StrokeColour;
        }
    }
}
