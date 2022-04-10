using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class RectElement
    {
        private static string FDT_ACTUAL_X = "fdtActualX";
        private static string FDT_ACTUAL_Y = "fdtActualY";
        private static string WIDTH = "width";
        private static string HEIGHT = "height";
        private static string STROKE = "stroke";
        private static string FILL = "fill";
        internal const string FILLOPACITY = "fill-opacity";

        public float X;
        public float Y;
        public float Width;
        public float Height;

        public ElementDescriptor.RGBColour FillColour;
        public ElementDescriptor.RGBColour StrokeColour;
        public RectElement(XElement elem)
        {
            X = float.Parse(elem.Attribute(FDT_ACTUAL_X).Value, CultureInfo.InvariantCulture);
            Y = float.Parse(elem.Attribute(FDT_ACTUAL_Y).Value, CultureInfo.InvariantCulture);
            var widthAtt = elem.Attributes().FirstOrDefault(x => x.Name.LocalName == WIDTH);
            var heightAtt = elem.Attributes().FirstOrDefault(x => x.Name.LocalName == HEIGHT);

            if (widthAtt !=  null) Width = float.Parse(widthAtt.Value, CultureInfo.InvariantCulture);
            if (heightAtt != null) Height = float.Parse(heightAtt.Value, CultureInfo.InvariantCulture);
            var strokeAtt = elem.Attributes().FirstOrDefault(x => x.Name.LocalName == STROKE);
            if (strokeAtt != null) StrokeColour = ElementDescriptor.ColourFromString(strokeAtt.Value, ElementDescriptor.BlackColour);


            var fillOpacityAtt = elem.Attributes().FirstOrDefault(x => x.Name.LocalName == FILLOPACITY);
            if (fillOpacityAtt != null && fillOpacityAtt.Value == "0")
            {
                FillColour = ElementDescriptor.ColourFromString("transparent", ElementDescriptor.WhiteColour);
            }
            else {
                var fillAtt = elem.Attributes().FirstOrDefault(x => x.Name.LocalName == FILL);
                if (fillAtt != null) FillColour = ElementDescriptor.ColourFromString(fillAtt.Value, ElementDescriptor.WhiteColour);
            }
        }

    }
}
