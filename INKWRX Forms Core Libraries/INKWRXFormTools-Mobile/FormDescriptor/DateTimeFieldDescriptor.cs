using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class DateTimeFieldDescriptor : ISOFieldDescriptor
    {
        private const string DELIMITER = "fdtDefDelimiter";

        public string FdtDelimiter { get; set; }

        public DateTimeFieldDescriptor (XElement element, int zOrder) : base(element, zOrder)
        {
            var att = element.Attribute(DELIMITER);
            FdtDelimiter = att == null ? "" :  att.Value;
        }

        public DateTimeFieldDescriptor (DateTimeFieldDescriptor original) : base(original)
        {
            FdtDelimiter = original.FdtDelimiter;
        }
    }
}
