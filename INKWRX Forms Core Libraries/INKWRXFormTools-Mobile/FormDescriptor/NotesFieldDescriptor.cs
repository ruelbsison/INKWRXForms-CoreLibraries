using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class NotesFieldDescriptor : FieldDescriptor
    {
        private const string FDT_TYPE_FORMAT = "fdtTypeFormat";
        private const string FDT_BARCODE_FIELD = "fdtBarcodeField";

        public int LimitPerLine { get; set; }
        public List<RectElement> RectElements = new List<RectElement>();
        public bool IsGraphical { get; set; }
        public bool IsBarcode { get; set; }

        public NotesFieldDescriptor (XElement element, int zOrder) : base(element, zOrder)
        {
            IsGraphical = false;
            IsBarcode = false;
            foreach (var child in element.Elements().ToList().Where(x=>x.Name.LocalName == RECT))
            {
                RectElements.Add(new RectElement(child));
            }

            foreach (var att in element.Attributes().ToList())
            {
                switch (att.Name.ToString())
                {
                    case FDT_TYPE_FORMAT:
                        if (att.Value == "Graphical")
                        {
                            IsGraphical = true;
                        }
                        break;
                    case FDT_BARCODE_FIELD:
                        if (att.Value == "true")
                        {
                            IsBarcode = true;
                        }
                        break;
                }
            }

            var firstRect = RectElements.FirstOrDefault();
            var lastRect = RectElements.LastOrDefault();
            if (firstRect != null && lastRect != null)
            {
                StrokeColour = firstRect.StrokeColour;
                Origin.X = firstRect.X;
                Origin.Y = firstRect.Y;
                Width = firstRect.Width;
                Height = lastRect.Height + lastRect.Y - firstRect.Y;
            }
        }

        public NotesFieldDescriptor (NotesFieldDescriptor original) : base (original)
        {
            RectElements = original.RectElements;
            LimitPerLine = original.LimitPerLine;
            IsGraphical = original.IsGraphical;
            IsBarcode = original.IsBarcode;
        }
    }
}
