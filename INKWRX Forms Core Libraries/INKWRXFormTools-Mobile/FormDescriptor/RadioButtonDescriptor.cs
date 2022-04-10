using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class RadioButtonDescriptor : TickedFieldDescriptor
    {
        public RadioButtonDescriptor(XElement element, int zOrder) : base(element, zOrder)
        {

        }

        public RadioButtonDescriptor (RadioButtonDescriptor original) : base(original)
        {

        }
    }
}
