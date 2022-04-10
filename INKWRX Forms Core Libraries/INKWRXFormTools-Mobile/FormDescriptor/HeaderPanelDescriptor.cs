using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormTools.FormDescriptor
{
    public class HeaderPanelDescriptor : DynamicPanelDescriptor
    {
        public HeaderPanelDescriptor() : base()
        {
            HeaderBackground = ElementDescriptor.BlackColour;
            HeaderStroke = ElementDescriptor.BlackColour;
        }

        public ElementDescriptor.RGBColour HeaderBackground { get; set; }
        public ElementDescriptor.RGBColour HeaderStroke { get; set; }
        public TextLabelDescriptor HeaderLabel { get; set; }
    }
}
