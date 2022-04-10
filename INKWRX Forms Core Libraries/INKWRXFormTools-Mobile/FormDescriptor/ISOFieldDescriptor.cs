using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class ISOFieldDescriptor : FieldDescriptor
    {
        internal const string FDT_FORMAT = "fdtFormat";
        internal const string FDT_DEF_LETTERS = "fdtDefLetters";
        internal const string FDT_LEXICON_ID = "fdtLexicon";
        internal const string TEXT = "text";
        internal const string FDT_CALC = "fdtCalc";

        public List<RectElement> RectElements = new List<RectElement>();
        public List<TextLabelDescriptor> TextLabelDescriptors = new List<TextLabelDescriptor>();
        public string HintChars { get; set; }
        public int LexiconId { get; set; }
        public string FdtFormat { get; set; }
        public string FdtListArray { get; set; }
        public bool HasDelimiterCharacters { get; set; }
        public string Calc { get; set; }
        public bool IsCalcField { get; set; }
        public bool IsCalcInput { get; set; }

        public ISOFieldDescriptor (XElement element, int zOrder) : base (element, zOrder)
        {
            LexiconId = -1;
            Calc = null;
            IsCalcField = false;
            IsCalcInput = false;
            HasDelimiterCharacters = false;

            foreach (var att in element.Attributes().ToList())
            {
                switch (att.Name.LocalName.ToString())
                {
                    case FDT_FORMAT:
                        FdtFormat = att.Value;
                        break;
                    case FDT_DEF_LETTERS:
                        HintChars = att.Value;
                        break;
                    case FDT_LEXICON_ID:
                        int lex = -1;
                        if (Int32.TryParse(att.Value, out lex))
                        {
                            LexiconId = lex;
                        }
                        break;
                    case FDT_CALC:
                        Calc = att.Value;
                        if (!string.IsNullOrEmpty(Calc))
                        {
                            IsCalcField = true;
                        } else
                        {
                            IsCalcInput = true;
                        }
                        break;
                }
            }

            var listChar = 0;
            var hints = "DMYH";
            var newListArrayString = new StringBuilder();
            foreach (var child in element.Elements().ToList())
            {
                if (child.Name.LocalName == RECT)
                {
                    RectElements.Add(new RectElement(child));
                    listChar++;
                }
                else if (child.Name.LocalName == TEXT && !hints.Contains(child.Value))
                {
                    HasDelimiterCharacters = true;
                    TextLabelDescriptors.Add(new TextLabelDescriptor(child, zOrder));
                    if (!string.IsNullOrEmpty(newListArrayString.ToString()))
                    {
                        newListArrayString.Append("|");
                    }
                    newListArrayString.Append(listChar);
                    listChar = 0;
                }
            }
            if (!string.IsNullOrEmpty(newListArrayString.ToString()))
            {
                newListArrayString.Append("|");
            }
            newListArrayString.Append(listChar);
            FdtListArray = newListArrayString.ToString();

            var firstRect = RectElements.FirstOrDefault();
            var lastRect = RectElements.LastOrDefault();
            if (firstRect != null && lastRect != null)
            {
                StrokeColour = firstRect.StrokeColour;
                Origin.X = firstRect.X;
                Origin.Y = firstRect.Y;
                Height = firstRect.Height;
                Width = lastRect.X + lastRect.Width - firstRect.X;
            }
        }

        public ISOFieldDescriptor (ISOFieldDescriptor original) :base(original)
        {
            RectElements = original.RectElements;
            TextLabelDescriptors = new List<TextLabelDescriptor>();
            foreach (var tld in original.TextLabelDescriptors)
            {
                TextLabelDescriptors.Add(new TextLabelDescriptor(tld));
            }
            FdtFormat = original.FdtFormat;
            LexiconId = original.LexiconId;
            HintChars = original.HintChars;
            FdtListArray = original.FdtListArray;
            IsCalcField = original.IsCalcField;
            IsCalcInput = original.IsCalcInput;
            Calc = original.Calc;
            HasDelimiterCharacters = original.HasDelimiterCharacters;
        }
    }
}
