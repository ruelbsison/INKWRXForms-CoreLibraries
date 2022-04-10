using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class DropdownDescriptor : FieldDescriptor
    {
        private const string LEXICON_ID = "fdtLexicon";
        private const string FDT_IS_CALC = "fdtIsCalc";


        public List<string> Lexicon;
        public RectElement RectElement { get; set; }
        public string LexiconId { get; set; }
        public bool IsCalc { get; set; }

        public DropdownDescriptor (XElement element, int zOrder) :base(element, zOrder)
        {
            Lexicon = new List<string>();
            LexiconId = "";

            foreach (var att in element.Attributes().ToList())
            {
                switch (att.Name.ToString())
                {
                    case LEXICON_ID:
                        LexiconId = att.Value;
                        break;
                    case FDT_IS_CALC:
                        if (att.Value == "true")
                        {
                            IsCalc = true;
                        }
                        break;
                }
            }

            foreach (var child in element.Elements().ToList().Where(x=>x.Name.LocalName == RECT))
            {
                RectElement = new RectElement(child);
            }

            StrokeColour = RectElement.StrokeColour;
            Origin.X = RectElement.X;
            Origin.Y = RectElement.Y;
            Height = RectElement.Height;
            Width = RectElement.Width;
        }

        public DropdownDescriptor(DropdownDescriptor original) : base(original)
        {
            RectElement = original.RectElement;
            Lexicon = original.Lexicon;
            LexiconId = original.LexiconId;
            IsCalc = original.IsCalc;
        }
    }
}
