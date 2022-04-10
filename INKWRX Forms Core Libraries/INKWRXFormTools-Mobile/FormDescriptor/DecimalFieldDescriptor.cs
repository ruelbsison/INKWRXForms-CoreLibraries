using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class DecimalFieldDescriptor : ISOFieldDescriptor
    {
        public DecimalFieldDescriptor(XElement element, int zOrder) : base(element, zOrder)
        {

        }

        public DecimalFieldDescriptor(DecimalFieldDescriptor original) : base(original)
        {

        }
    }
}
