using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class DrawingFieldDescriptor : TickedFieldDescriptor
    {
        public DrawingFieldDescriptor (XElement element, int zOrder) : base(element,zOrder)
        {

        }

        public DrawingFieldDescriptor(DrawingFieldDescriptor original) : base(original)
        {

        }
    }
}
