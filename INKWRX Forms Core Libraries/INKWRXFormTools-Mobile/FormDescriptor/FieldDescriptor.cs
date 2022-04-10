using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class FieldDescriptor : ElementDescriptor
    {
        internal static string ISO_FIELD = "iso";
        internal static string DROP_DOWNN = "dropdown";
        internal static string TICK_BOX = "tickBox";
        internal static string RADIO = "radioList";
        internal static string NOTES = "cursiveNotes";
        internal static string NOTES2 = "cursiveStandard";
        internal static string SIGNATURE_FIELD = "cursiveSignature";
        internal static string SKETCHBOX = "cursiveSketchBox";
        internal static string FONT_FILL = "fill";
        internal static string FONT_FAMILY = "font-family";
        internal static string FONT_SIZE = "font-size";
        internal static string FONT_STYLE = "font-style";
        internal static string FONT_WEIGHT = "font-weight";
        internal static string FONT_DECORATION = "text-decoration";
        internal static string FDT_MANDATORY = "fdtMandatory";

        public string FdtType { get; set; }
        public bool Mandatory { get; set; }

        public FieldDescriptor(XElement element, int zOrder) : base(element, zOrder)
        {
            foreach (var att in element.Attributes().ToList())
            {
                if (att.Name == FDT_TYPE)
                {
                    FdtType = att.Value;
                }
                if (att.Name == FDT_MANDATORY)
                {
                    Mandatory = att.Value == "true";
                }
            }
        }

        public FieldDescriptor(FieldDescriptor original) : base(original)
        {
            FdtType = original.FdtType;
            Mandatory = original.Mandatory;
        }
    }
}
