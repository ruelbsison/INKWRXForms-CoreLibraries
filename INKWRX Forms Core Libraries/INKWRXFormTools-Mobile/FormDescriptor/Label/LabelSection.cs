using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormTools.FormDescriptor.Label
{
    public class LabelSection : LabelElement
    {
        private LabelSection sectionParent = null;
        public List<LabelElement> Children { get; set; }

        #region Private Property Placeholders
        private string fontName = null;
        private ElementDescriptor.RGBColour? textColour = null;
        private double? textSize = null;
        private bool? italic = null;
        private bool? bold = null;
        private bool? underline = null;
        #endregion

        #region Public Properties
        public string FontName {
            get
            {
                return fontName != null 
                    ? fontName 
                    : sectionParent != null ? sectionParent.FontName : "arial";
            }
            set { fontName = value; }
        }

        public ElementDescriptor.RGBColour TextColour
        {
            get
            {
                return textColour != null 
                    ? textColour.Value
                    : sectionParent != null ? sectionParent.TextColour : ElementDescriptor.BlackColour;
            }
            set { textColour = value; }
        }

        public double TextSize
        {
            get
            {
                return textSize != null 
                    ? textSize.Value 
                    : sectionParent != null ? sectionParent.TextSize : 12.0;
            }
            set { textSize = value; }
        }

        public bool Italic
        {
            get
            {
                return italic != null
                    ? italic.Value
                    : sectionParent != null ? sectionParent.Italic : false;
            }
            set { italic = value; }
        }

        public bool Bold
        {
            get
            {
                return bold != null
                    ? bold.Value
                    : sectionParent != null ? sectionParent.Bold : false;
            }
            set { bold = value; }
        }

        public bool Underline
        {
            get
            {
                return underline != null
                    ? underline.Value
                    : sectionParent != null ? sectionParent.Underline : false;
            }
            set { underline = value; }
        }

        #endregion

        public LabelSection(LabelSection parent)
        {
            Children = new List<LabelElement>();
            sectionParent = parent;
        }

        public string RawString
        {
            get
            {
                var ret = new StringBuilder();
                foreach (var elem in Children)
                {
                    ret.Append(elem is LabelText ? elem.ToString() : ((LabelSection)elem).RawString);
                }
                return ret.ToString();
            }
        }
    }
}
