using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class RoundedRectangleDescriptor : RectangleDescriptor
    {
        private const string RADIUS_X = "rx";
        private const string RADIUS_Y = "ry";
        
        public Point Radius = new Point { X = 0, Y = 0 };

        public RoundedRectangleDescriptor (XElement element, int zOrder) : base (element, zOrder)
        {
            foreach (var att in element.Attributes().ToList())
            {
                switch (att.Name.ToString())
                {
                    case RADIUS_X:
                        Radius.X = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        break;
                    case RADIUS_Y:
                        Radius.Y = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        break;
                }
            }
        }

        public RoundedRectangleDescriptor (RoundedRectangleDescriptor original) : base (original)
        {
            Radius = original.Radius;
        }
    }
}
