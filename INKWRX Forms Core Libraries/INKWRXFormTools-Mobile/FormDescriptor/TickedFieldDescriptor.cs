using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class TickedFieldDescriptor : FieldDescriptor
    {
        internal const string TICKED = "fdtTicked";
        internal const string NOT_TICKED = "fdtNotTicked";
        internal const string GROUP = "fdtGroupName";

        public string TickedValue { get; set; }
        public string NotTickedValue { get; set; }

        public string GroupName { get; set; }
        public RectElement RectElement { get; set; }


        public TickedFieldDescriptor(XElement element, int zOrder) : base(element, zOrder)
        {
            TickedValue = "";
            NotTickedValue = "";
            foreach (var att in element.Attributes().ToList())
            {
                switch(att.Name.ToString())
                {
                    case TICKED:
                        TickedValue = att.Value;
                        break;
                    case NOT_TICKED:
                        NotTickedValue = att.Value;
                        break;
                    case GROUP:
                        GroupName = att.Value;
                        break;
                }
            }

            foreach (var child in element.Elements().ToList().Where(x=>x.Name.LocalName == RECT))
            {
                RectElement = new RectElement(child);
            }

            Origin.X = RectElement.X;
            Origin.Y = RectElement.Y;
            Width = RectElement.Width;
            Height = RectElement.Height;
            StrokeColour = RectElement.StrokeColour;
            FillColour = RectElement.FillColour;
        }
        public TickedFieldDescriptor(TickedFieldDescriptor original) : base(original)
        {
            TickedValue = original.TickedValue;
            NotTickedValue = original.NotTickedValue;
            GroupName = original.GroupName;
            RectElement = original.RectElement;
        }
    }
}
