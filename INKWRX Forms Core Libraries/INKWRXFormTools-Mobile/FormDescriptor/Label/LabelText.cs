using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormTools.FormDescriptor.Label
{
    public class LabelText : LabelElement
    {
        public string Value { private get; set; }
        public LabelText(string text)
        {
            Value = text;
        }

        public override string ToString()
        {
            return Value != null ? Value : "";
        }
    }
}
