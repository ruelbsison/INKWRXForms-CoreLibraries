using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class ElementDescriptor
    {
        public struct Point
        {
            public double X;
            public double Y;
        }

        public struct RGBColour
        {
            public double Alpha { get; set; }
            public double Red { get; set; }
            public double Green { get; set; }
            public double Blue { get; set; }
        }

        public static RGBColour BlackColour = new RGBColour { Red = 0, Blue = 0, Green = 0, Alpha = 255 };
        public static RGBColour WhiteColour = new RGBColour { Red = 255, Green = 255, Blue = 255, Alpha = 255 };
        public static RGBColour ClearColour = new RGBColour { Red = 0, Green = 0, Blue = 0, Alpha = 0 };

        internal const string X = "x";
        internal const string Y = "y";
        internal const string ID = "id";
        internal const string FDT_TYPE = "fdtType";
        internal const string WIDTH = "width";
        internal const string HEIGHT = "height";
        internal const string FDT_FIELD_NAME = "fdtFieldName";
        internal const string X_OFFSET = "dx";
        internal const string Y_OFFSET = "dy";
        internal const string STROKE = "stroke";
        internal const string RECT = "rect";


        internal double xOffset = 0;
        internal double yOffset = 0;

        internal bool didSetXOffset = false;
        internal bool didSetYOffset = false;

        internal bool fieldOk = false;
        internal XElement source = null;

        public Point Origin = new Point { X = 0, Y = 0 };
        public int ZOrder { get; set; }
        public double Height { get; set;}
        public double Width = 0.0;
        public string FieldId = "";
        public RGBColour StrokeColour = BlackColour;
        public RGBColour FillColour = WhiteColour;
        public string FdtFieldName = "";
        public int RepeatingIndex;

        public ElementDescriptor(XElement element, int zOrder)
        {
            source = element;
            ZOrder = zOrder;
            RepeatingIndex = -1;
            foreach (var att in source.Attributes().ToList())
            {
                switch (att.Name.LocalName.ToString())
                {
                    case ID:
                        FieldId = att.Value;
                        break;
                    case X:
                        Origin.X = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        break;
                    case Y:
                        Origin.Y = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        break;
                    case WIDTH:
                        Width = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        break;
                    case HEIGHT:
                        Height = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        break;
                    case STROKE:
                        StrokeColour = ColourFromString(att.Value, BlackColour);
                        break;
                    case FDT_FIELD_NAME:
                        FdtFieldName = att.Value;
                        break;
                }
            }
        }

        public ElementDescriptor(ElementDescriptor original)
        {
            RepeatingIndex = -1;
            FieldId = original.FieldId;
            Origin.X = original.Origin.X;
            Origin.Y = original.Origin.Y;
            source = original.source;
            xOffset = original.xOffset;
            yOffset = original.yOffset;
            Width = original.Width;
            fieldOk = original.fieldOk;
            Height = original.Height;
            ZOrder = original.ZOrder;
            StrokeColour = original.StrokeColour;
            FillColour = original.FillColour;
            FdtFieldName = original.FdtFieldName;
        }

        public string RepeatingFieldId
        {
            get { return RepeatingIndex == -1 ? FieldId : string.Format("{0}_{1}", FieldId, RepeatingIndex); }
        }

        public static RGBColour ColourFromString(string colourText, RGBColour defaultColour) {
            try
            {
                if (colourText.StartsWith("rgb"))
                {
                    colourText = colourText.Replace("rgb(", "").Replace(" ", "").Replace(")", "");
                    var split = colourText.Split(',');
                    return new RGBColour
                    {
                        Red = double.Parse(split[0], CultureInfo.InvariantCulture),
                        Green = double.Parse(split[1], CultureInfo.InvariantCulture),
                        Blue = double.Parse(split[2], CultureInfo.InvariantCulture),
                        Alpha = 255
                    };
                }
                if (colourText == "transparent")
                {
                    return new RGBColour
                    {
                        Red = 0.0,
                        Green = 0.0,
                        Blue = 0.0,
                        Alpha = 0.0
                    };
                }
                return ColourFromHexaString(colourText, defaultColour);
            }
            catch 
            {
                return defaultColour;
            }
        }

        private static RGBColour ColourFromHexaString(string colourText, RGBColour defaultColour)
        {
            try
            {
                string cleanHex = colourText.Replace("0x", "").TrimStart('#');
                if (cleanHex.Length < 6)
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i < cleanHex.Length; i++)
                    {
                        sb.Append(cleanHex[i]);
                        sb.Append(cleanHex[i]);
                    }
                    cleanHex = sb.ToString();
                }
                if (cleanHex.Length == 6)
                {
                    //Affix fully opaque alpha hex value of FF (225)
                    cleanHex = "FF" + cleanHex;
                }

                return new RGBColour
                {
                    Alpha = Int32.Parse(cleanHex.Substring(0, 2), NumberStyles.HexNumber),
                    Red = Int32.Parse(cleanHex.Substring(2, 2), NumberStyles.HexNumber),
                    Green = Int32.Parse(cleanHex.Substring(4, 2), NumberStyles.HexNumber),
                    Blue = Int32.Parse(cleanHex.Substring(6), NumberStyles.HexNumber)
                };
            }
            catch
            {
                return defaultColour;
            }
        }
    }
}
