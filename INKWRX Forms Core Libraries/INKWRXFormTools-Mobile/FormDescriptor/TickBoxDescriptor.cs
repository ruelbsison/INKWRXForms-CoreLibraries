using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class TickBoxDescriptor : TickedFieldDescriptor
    {
        public enum Size
        {
            Small = 0,
            Normal = 1,
            Large = 2
        }

        public Size TickSize { get; set; }

        public TickBoxDescriptor (XElement element, int zOrder) : base (element, zOrder)
        {
            switch ((int)Math.Floor(RectElement.Width))
            {
                case 11:
                    TickSize = Size.Small;
                    break;
                case 24:
                    TickSize = Size.Small;
                    break;
                default:
                    TickSize = Size.Normal;
                    break;
            }
        }

        public TickBoxDescriptor (TickBoxDescriptor original) : base (original)
        {
            TickSize = original.TickSize;
        }
    }
}
