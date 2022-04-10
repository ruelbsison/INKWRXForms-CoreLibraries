using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class LineDescriptor : ShapeDescriptor
    {
        private const string X1 = "x1";
        private const string X2 = "x2";
        private const string Y1 = "y1";
        private const string Y2 = "y2";

        public Point StartPoint = new Point { X = 0, Y = 0 };
        public Point EndPoint = new Point { X = 0, Y = 0 };

        public string LineType { get; set; }

        public LineDescriptor (XElement element, int zOrder) : base (element, zOrder)
        {
            foreach (var att in element.Attributes().ToList())
            {
                switch (att.Name.ToString())
                {
                    case X1:
                        StartPoint.X = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        break;
                    case X2:
                        EndPoint.X = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        break;
                    case Y1:
                        StartPoint.Y = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        break;
                    case Y2:
                        EndPoint.Y = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        break;
                    case FDT_TYPE:
                        LineType = att.Value.Replace("shapeLine", "");
                        break;
                }
            }
        }

        public LineDescriptor (LineDescriptor original) : base (original)
        {
            StartPoint = original.StartPoint;
            EndPoint = original.EndPoint;
            LineType = original.LineType;
        }
    }
}
