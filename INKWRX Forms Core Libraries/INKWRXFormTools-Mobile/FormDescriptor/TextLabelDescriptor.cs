using FormTools.FormDescriptor.Label;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class TextLabelDescriptor : ElementDescriptor
    {

        private const string FONT_FILL = @"fill";
        private const string FONT_FAMILY = @"font-family";
        private const string FONT_SIZE = @"font-size";
        private const string FONT_STYLE = @"font-style";
        private const string FONT_WEIGHT = @"font-weight";
        private const string FONT_DECORATION = @"text-decoration";


        public LabelSection BaseSection = null;

        private enum AttributeKey
        {
            TextColor,
            FontSize,
            Bold,
            Italic,
            Underline,
            FontName,
            BaseStart
        }

        private string lastY = null;

        private void processTSpanElement(XElement element, bool isFirstElement, bool baseStart)
        {
            this.processTSpanElement(element, isFirstElement, baseStart, null);
        }

        private void processTSpanElement(XElement element, bool isFirstElement, bool baseStart, LabelSection baseSection)
        {
            var addNewLine = false;
            var thisSection = new LabelSection(baseSection);
            if (isFirstElement)
            {
                BaseSection = thisSection;
            }
            var startTagHere = false;
            foreach (var att in element.Attributes().ToList())
            {
                if (att.Name.LocalName == "starttag")
                {
                    if (baseSection != null && baseSection.Children.Count > 0)
                    {
                        addNewLine = true;
                        startTagHere = true;
                    }
                }

                if (att.Name.LocalName == "display")
                {
                    if (att.Value == "none")
                    {
                        return;
                    }
                }

                if (att.Name.LocalName == X_OFFSET)
                {
                    if (!didSetXOffset)
                    {
                        xOffset = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        didSetXOffset = true;
                    }
                }
                else if (att.Name.LocalName == Y_OFFSET)
                {
                    string thisY = att.Value;
                    if (lastY == null)
                    {
                        lastY = thisY;
                    }
                    else
                    {
                        if (lastY != "0" && thisY == "0")
                        {
                            double lastYDouble = double.Parse(lastY, CultureInfo.InvariantCulture);
                            Origin.Y -= lastYDouble;
                        }
                    }
                    if (thisY != lastY && thisY != "0")
                    {
                        if (baseStart)
                        {
                            addNewLine = true;
                        }
                    }

                    if (!didSetYOffset)
                    {
                        yOffset = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        didSetYOffset = true;
                    }
                }
                else if (att.Name.LocalName == FONT_FAMILY)
                {
                    thisSection.FontName = att.Value;
                }
                else if (att.Name.LocalName == FONT_DECORATION)
                {
                    thisSection.Underline = att.Value == "underline";
                }
                else if (att.Name.LocalName == FONT_FILL)
                {
                    thisSection.TextColour = ColourFromString(att.Value, BlackColour);
                }
                else if (att.Name.LocalName == FONT_SIZE)
                {
                    thisSection.TextSize = double.Parse(att.Value.Replace(" ", "").Replace("pt", ""), CultureInfo.InvariantCulture) - 1;
                }
                else if (att.Name.LocalName == FONT_STYLE)
                {
                    thisSection.Italic = att.Value == "italic";
                }
                else if (att.Name.LocalName == FONT_WEIGHT)
                {
                    thisSection.Bold = att.Value == "bold";
                }
            }

            if (addNewLine)
            {
                thisSection.Children.Add(new LabelText("\n"));
            }

            if (element.Nodes().ToList().Count > 0)
            {
                foreach (var child in element.Nodes().ToList())
                {
                    if (child.NodeType == XmlNodeType.Element)
                    {
                        processTSpanElement(XElement.Parse(child.ToString()), false, startTagHere, thisSection);
                        baseStart = false;
                    }
                    else
                    {
                        thisSection.Children.Add(new LabelText(child.ToString()));
                    }
                }
            }

            if (isFirstElement) return;

            if (baseSection != null)
            {
                baseSection.Children.Add(thisSection);
            } else
            {
                BaseSection.Children.Add(thisSection);
            }
        }

        public TextLabelDescriptor (XElement element, int zOrder) : base(element, zOrder)
        {
            processTSpanElement(element, true, false);
        }

        public TextLabelDescriptor(TextLabelDescriptor original) : base(original)
        {
            BaseSection = original.BaseSection;
        }
    }
}
