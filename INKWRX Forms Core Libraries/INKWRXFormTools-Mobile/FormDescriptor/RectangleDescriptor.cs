using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class RectangleDescriptor : ShapeDescriptor
    {
        public RectangleDescriptor(XElement element, int zOrder) : base(element, zOrder)
        {
            var attFillOpacity = element.Attribute(FILL);
            string fillColourString;
            if (attFillOpacity != null && attFillOpacity.Value == "0")
            {
                fillColourString = "transparent";
            }
            else {
                var att = element.Attribute(FILL);
                fillColourString = att.Value;
            }
            FillColour = ColourFromString(fillColourString, WhiteColour);
        }

        public RectangleDescriptor(RectangleDescriptor original) : base(original)
        {
            FillColour = original.FillColour;
            
        }
    }
}
