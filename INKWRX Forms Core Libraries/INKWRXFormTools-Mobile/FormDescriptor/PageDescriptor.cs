using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class PageDescriptor
    {
        #region Private Consts
        private const string ID = "id";
        private const string PAGE_USE = "fdtpageuse";
        private const string PAGE_TYPE = "fdtpagetype";
        private const string TYPE = "fdtType";
        private const string PAGE_CONDITIONAL_FIELDS = "fdtpageconditionalfields";
        private const string SIGNATURE_OPTS = "fdtSignatureOptions";
        private const string RECT = "rect";
        private const string ROUNDED_RECTANGLE = "shapeRectRounded";
        private const string ISO_FIELD = "iso";
        private const string DROP_DOWN = "dropdown";
        private const string TICK_BOX = "tickBox";
        private const string RADIO = "radioList";
        private const string NOTES = "cursiveNotes";
        private const string NOTES2 = "cursiveStandard";
        private const string SIGNATURE_FIELD = "cursiveSignature";
        private const string SKETCHBOX = "cursiveSketchBox";
        private const string RECTANGLE = "shapeRect";
        private const string HEADER = "header";
        #endregion

        #region Properties/Fields
        public XElement Source { get; set; }

        public int PageNumber { get; set; }
        public int RealPageNumber { get; set; }

        public List<FieldDescriptor> FieldDescriptors = new List<FieldDescriptor>();
        public List<ImageDescriptor> ImageDescriptors = new List<ImageDescriptor>();
        public List<TextLabelDescriptor> TextLabelDescriptors = new List<TextLabelDescriptor>();
        public List<ShapeDescriptor> ShapeDescriptors = new List<ShapeDescriptor>();

        public Dictionary<string, List<RadioButtonDescriptor>> RadioGroups = new Dictionary<string, List<RadioButtonDescriptor>>();
        public Dictionary<string, List<List<RadioButtonDescriptor>>> RepeatingRadioGroups = new Dictionary<string, List<List<RadioButtonDescriptor>>>();

        public Dictionary<string, bool> PageTriggers = new Dictionary<string, bool>();

        public bool AndTriggers { get; set; }
        public bool Visible { get; set; }
        public List<DynamicPanelDescriptor> Panels = new List<DynamicPanelDescriptor>();

        public Dictionary<string, string> AllFieldsIds = new Dictionary<string, string>();

        public double PageWidth { get; set; }
        public double PageHeight { get; set; }

        public bool PageOk { get; set; }

        private int zOrderCount { get; set; }

        public Dictionary<string, List<string>> MandatoryCheckBoxGroups = new Dictionary<string, List<string>>();

        public List<FieldDescriptor> MandatoryFields = new List<FieldDescriptor>();
        public List<string> MandatoryRadioGroups = new List<string>();

        public List<CalcList> PageCalcFields = new List<CalcList>();

        public bool Output { get; set; }
        #endregion

        public PageDescriptor()
        {
            AndTriggers = false;
            Visible = true;
        }

        public PageDescriptor(XElement source, int pageNumber) : this()
        {
            Source = source;
            PageNumber = pageNumber;
            Output = false;

            foreach (var att in Source.Attributes().ToList())
            {
                switch (att.Name.ToString())
                {
                    case ID:
                        RealPageNumber = Int32.Parse(att.Value.Replace("formSide", ""));
                        break;
                    case PAGE_USE:
                        if (att.Value.ToLower() == "output")
                        {
                            Visible = false;
                            Output = true;
                        }
                        break;
                    case PAGE_TYPE:
                        if (att.Value.ToLower() == "conditional")
                        {
                            Visible = false;
                        }
                        break;
                    case PAGE_CONDITIONAL_FIELDS:
                        var s = att.Value.Replace(" and ", "&").Replace(" or ", "|").Replace(";", "&").Replace(",", "|").Replace(" ", "");
                        if (s.Contains("|") || s.Contains("&"))
                        {
                            AndTriggers = s.Contains("&");
                            var split = AndTriggers ? s.Split('&') : s.Split('|');
                            foreach (var st in split)
                            {
                                PageTriggers.Add(st, false);
                            }
                        } else
                        {
                            PageTriggers.Add(s, false);
                        }
                        break;
                }
            }

            PageOk = false;
            var firstSvg = Source.Elements().FirstOrDefault();
            if (firstSvg == null) return;
            var secondSvg = firstSvg.Elements().FirstOrDefault();
            if (secondSvg == null) return;

            foreach (var att in secondSvg.Attributes().ToList().Where(x=>x.Name == "viewBox"))
            {
                var viewBoxVals = att.Value.Split(' ');
                PageWidth = double.Parse(viewBoxVals[2], CultureInfo.InvariantCulture);
                PageHeight = double.Parse(viewBoxVals[3], CultureInfo.InvariantCulture);
            }

            XElement pageNode = null;
            foreach (var pageNode1 in secondSvg.Elements().ToList())
            {
                var rubberBandBox = false;
                var mainNode = false;
                if (pageNode1.Name.LocalName != "g") continue;

                foreach (var nodeAtt in pageNode1.Attributes().ToList())
                {
                    if (nodeAtt.Name == "class")
                    {
                        if (nodeAtt.Value == "rubberBandBox")
                        {
                            rubberBandBox = true;
                            continue;
                        }
                    }
                    if (nodeAtt.Name == "id")
                    {
                        if (nodeAtt.Value.Contains("Main"))
                        {
                            mainNode = true;
                            pageNode = pageNode1;
                            break;
                        }
                    }

                }

                if (mainNode && !rubberBandBox)
                {
                    break;
                }
            }
            if (pageNode == null) return;
            zOrderCount = 0;
            foreach (var element in pageNode.Elements().ToList())
            {
                processElement(element);
            }

            foreach (var panel in Panels)
            {
                var r = panel.RectArea;
                foreach (var fd in FieldDescriptors)
                {
                    if (fd.Origin.Y > r.Y + r.Height)
                    {
                        panel.FieldsBelowPanel.Add(fd);
                        continue;
                    }

                    if (fd.Origin.Y >= r.Y)
                    {
                        panel.ShouldMoveFieldsBelow = false;
                        continue;
                    }

                    if (fd.Origin.Y + fd.Height > r.Y)
                    {
                        panel.ShouldMoveFieldsBelow = false;
                    }
                }

                foreach (var s in RadioGroups.Keys.ToList())
                {
                    var list = RadioGroups[s];
                    foreach (var fd in list)
                    {
                        if (fd.Origin.Y > r.Y + r.Height)
                        {
                            panel.FieldsBelowPanel.Add(fd);
                            continue;
                        }

                        if (fd.Origin.Y >= r.Y)
                        {
                            panel.ShouldMoveFieldsBelow = false;
                            continue;
                        }

                        if (fd.Origin.Y + fd.Height > r.Y)
                        {
                            panel.ShouldMoveFieldsBelow = false;
                        }
                    }
                }

                foreach (var dp in Panels)
                {
                    if (dp == panel) continue;
                    var fd = dp.RectArea;
                    if (fd.Y > r.Y + r.Height)
                    {
                        panel.FieldsBelowPanel.Add(dp);
                        continue;
                    }

                    if (fd.Y >= r.Y)
                    {
                        panel.ShouldMoveFieldsBelow = false;
                        continue;
                    }

                    if (fd.Y + fd.Height > r.Y)
                    {
                        panel.ShouldMoveFieldsBelow = false;
                    }
                }

            }

            PageOk = true;
        }
        

        private void processElement(XElement element)
        {
            processElement(element, null);
        }

        private void processElement(XElement element, List<object> list)
        {
            if (element.Name.LocalName == RECT)
            {
                var tabletImage = false;
                var roundedRect = false;
                foreach (var att in element.Attributes().ToList())
                {
                    if (att.Name == SIGNATURE_OPTS)
                    {
                        if (att.Value == "Tablet Image")
                        {
                            tabletImage = true;
                        }
                    }
                    else if (att.Name == TYPE)
                    {
                        if (att.Value == ROUNDED_RECTANGLE)
                        {
                            roundedRect = true;
                        }
                    }
                }

                if (tabletImage)
                {
                    if (list == null)
                    {
                        FieldDescriptors.Add(new TabletImageDescriptor(element, zOrderCount));
                    }
                    else
                    {
                        list.Add(new TabletImageDescriptor(element, zOrderCount));
                    }
                }
                else
                {
                    if (list == null)
                    {
                        ShapeDescriptors.Add(roundedRect ? new RoundedRectangleDescriptor(element, zOrderCount) : new RectangleDescriptor(element, zOrderCount));
                    } else
                    {
                        list.Add(roundedRect ? new RoundedRectangleDescriptor(element, zOrderCount) : new RectangleDescriptor(element, zOrderCount));
                    }
                }

                zOrderCount++;
                return;
            }

            #region Display Elements

            if (element.Name.LocalName == "image")
            {
                if (list == null)
                {
                    ImageDescriptors.Add(new ImageDescriptor(element, zOrderCount));
                }
                else
                {
                    list.Add(new ImageDescriptor(element, zOrderCount));
                }
                zOrderCount++;
                return;
            }

            if (element.Name.LocalName == "line")
            {
                if (list == null)
                {
                    ShapeDescriptors.Add(new LineDescriptor(element, zOrderCount));
                }
                else
                {
                    list.Add(new LineDescriptor(element, zOrderCount));
                }
                zOrderCount++;
                return;
            }

            if (element.Name.LocalName == "text")
            {
                if (list == null)
                {
                    TextLabelDescriptors.Add(new TextLabelDescriptor(element, zOrderCount));
                }
                else
                {
                    list.Add(new TextLabelDescriptor(element, zOrderCount));
                }
                zOrderCount++;
                return;
            }

            if (element.Name.LocalName == "circle")
            {
                if (list == null)
                {
                    ShapeDescriptors.Add(new CircleDescriptor(element, zOrderCount));
                }
                else
                {
                    list.Add(new CircleDescriptor(element, zOrderCount));
                }
                zOrderCount++;
                return;
            }

            #endregion

            //at this point we determine it must be a "g" element.
            //"g" can be either a group or a field - a field would have a "fdtType" attribute so we determine that here.

            var gTypeAtt = element.Attributes().FirstOrDefault(x=>x.Name == TYPE);
            if (gTypeAtt != null && gTypeAtt.Value != HEADER)
            {
                //field...
                var fdtType = gTypeAtt.Value;
                switch (fdtType)
                {
                    case ISO_FIELD:
                        var testIsoDescriptor = new ISOFieldDescriptor(element, zOrderCount);
                        ISOFieldDescriptor thisIso = null;
                        switch (testIsoDescriptor.LexiconId)
                        {
                            case 13:
                            case 14:
                            case 20:
                            case 21:
                            case 22:
                            case 23:
                            case 15:
                                thisIso = new DateTimeFieldDescriptor(element, zOrderCount);
                                break;
                            case 18:
                                thisIso = new DecimalFieldDescriptor(element, zOrderCount);
                                break;
                            default:
                                if (testIsoDescriptor.FdtFormat.ToLower() == "decimal")
                                {
                                    thisIso = new DecimalFieldDescriptor(element, zOrderCount);
                                }
                                else
                                {
                                    thisIso = testIsoDescriptor;
                                }
                                break;
                        }

                        addItem(thisIso, list);

                        if (thisIso.IsCalcField)
                        {
                            var calcItem = new CalcList();
                            calcItem.Descriptor = thisIso;
                            calcItem.FieldName = thisIso.FdtFieldName;
                            calcItem.Inputs = CalcList.FieldList(thisIso.Calc);
                            PageCalcFields.Add(calcItem);
                        }

                        if (thisIso.Mandatory)
                        {
                            if (!MandatoryFields.Contains(thisIso))
                            {
                                MandatoryFields.Add(thisIso);
                            }
                        }
                        AllFieldsIds[thisIso.FieldId] = thisIso.FdtFieldName;
                        break;
                    case TICK_BOX:
                        var tbd = new TickBoxDescriptor(element, zOrderCount);

                        addItem(tbd, list);

                        if (tbd.Mandatory)
                        {
                            if (!string.IsNullOrEmpty(tbd.GroupName))
                            {
                                if (!MandatoryCheckBoxGroups.ContainsKey(tbd.GroupName))
                                {
                                    MandatoryCheckBoxGroups[tbd.GroupName] = new List<string>();
                                }
                                MandatoryCheckBoxGroups[tbd.GroupName].Add(tbd.FdtFieldName);
                            }
                            else
                            {
                                MandatoryFields.Add(tbd);
                            }
                        }

                        AllFieldsIds[tbd.FieldId] = tbd.FdtFieldName;
                        break;
                    case RADIO:
                        var rbd = new RadioButtonDescriptor(element, zOrderCount);
                        var radioList = RadioGroups.ContainsKey(rbd.GroupName)
                                ? RadioGroups[rbd.GroupName]
                                : new List<RadioButtonDescriptor>();
                        radioList.Add(rbd);
                        RadioGroups[rbd.GroupName] = radioList;
                        if (list != null)
                        {
                            if (!list.Contains(radioList))
                            {
                                list.Add(radioList);
                            }
                        }

                        if (rbd.Mandatory)
                        {
                            if(!MandatoryRadioGroups.Contains(rbd.GroupName))
                            {
                                MandatoryRadioGroups.Add(rbd.GroupName);
                            }
                        }
                        AllFieldsIds[rbd.FieldId] = rbd.FdtFieldName;
                        break;
                    case NOTES:
                    case NOTES2:
                        var nfd = new NotesFieldDescriptor(element, zOrderCount);

                        addItem(nfd, list);

                        if (nfd.Mandatory)
                        {
                            if (!MandatoryFields.Contains(nfd))
                            {
                                MandatoryFields.Add(nfd);
                            }
                        }
                        AllFieldsIds[nfd.FieldId] = nfd.FdtFieldName;
                        break;
                    case SIGNATURE_FIELD:
                    case SKETCHBOX:
                        var dfd = new DrawingFieldDescriptor(element, zOrderCount);
                        addItem(dfd, list);
                        if (dfd.Mandatory)
                        {
                            if (!MandatoryFields.Contains(dfd))
                            {
                                MandatoryFields.Add(dfd);
                            }
                        }

                        AllFieldsIds[dfd.FieldId] = dfd.FdtFieldName;
                        break;
                    case DROP_DOWN:
                        var ddfd = new DropdownDescriptor(element, zOrderCount);
                        addItem(ddfd, list);
                        if (ddfd.Mandatory)
                        {
                            if (!MandatoryFields.Contains(ddfd))
                            {
                                MandatoryFields.Add(ddfd);
                            }
                        }
                        AllFieldsIds[ddfd.FieldId] = ddfd.FdtFieldName;
                        break;
                    case RECTANGLE:
                    case ROUNDED_RECTANGLE:
                        var tabletImage = false;
                        foreach (var rectAtt in element.Attributes().ToList())
                        {
                            if (rectAtt.Name == SIGNATURE_OPTS)
                            {
                                if (rectAtt.Value == "Tablet Image")
                                {
                                    tabletImage = true;
                                    break;
                                }
                            }
                        }
                        if (tabletImage)
                        {
                            addItem(new TabletImageDescriptor(element, zOrderCount), list);
                        } else
                        {
                            addItem(gTypeAtt.Value == RECTANGLE 
                                ? new RectangleDescriptor(element, zOrderCount) 
                                : new RoundedRectangleDescriptor(element, zOrderCount), 
                                list);
                        }
                        break;
                }

                if (fdtType == "panelPanel") return;
                zOrderCount++;
                return;
            }

            if (gTypeAtt != null)
            {
                // must be a header panel...
                var header = new HeaderPanelDescriptor();

                var headerRect = element.Elements()
                    .FirstOrDefault(x => x.Attributes().FirstOrDefault(y=>y.Name == "id") != null 
                        && x.Attributes().FirstOrDefault(y => y.Name == "id").Value.Contains("HeaderBg"));
                if (headerRect == null) return;
                var headerLabel = element.Elements()
                    .FirstOrDefault(x => x.Attributes().FirstOrDefault(y => y.Name == "id") != null
                        && x.Attributes().FirstOrDefault(y => y.Name == "id").Value.Contains("Label"));
                if (headerLabel == null) return;
                var headerMain = element.Elements()
                    .FirstOrDefault(x => x.Attributes().FirstOrDefault(y => y.Name == "id") != null
                        && x.Attributes().FirstOrDefault(y => y.Name == "id").Value.Contains("HeaderBody"));
                if (headerMain == null) return;
                header.FieldId = headerRect.Attribute("id").Value;
                header.HeaderLabel = new TextLabelDescriptor(headerLabel, 0);
                header.RectArea = new RectElement(headerMain);

                var headerBg = headerRect.Attributes().FirstOrDefault(x => x.Name == "fill");
                var headerStroke = headerRect.Attributes().FirstOrDefault(x => x.Name == "stroke");
                if (headerBg != null)
                {
                    header.HeaderBackground = ElementDescriptor.ColourFromString(headerBg.Value, ElementDescriptor.BlackColour);
                }
                if (headerStroke != null)
                {
                    header.HeaderStroke = ElementDescriptor.ColourFromString(headerStroke.Value, ElementDescriptor.BlackColour);
                }
                
                var group = element.Elements().FirstOrDefault(g => g.Name.LocalName == "g");
                var panelGroup = new List<object>();
                if (group != null)
                {
                    foreach (var child in group.Elements())
                    {
                        processElement(child, panelGroup);
                    }
                }
                header.Children = panelGroup;
                if (list == null)
                {
                    Panels.Add(header);
                } else
                {
                    list.Add(header);
                }
                return;
            }

            var panel = element.Attributes().FirstOrDefault(x => x.Name == "panelContainer");
            if (panel != null && panel.Value == "true")
            {
                var firstRect = element.Elements().FirstOrDefault();
                var dynPanel = new DynamicPanelDescriptor();
                if (list != null)
                {
                    dynPanel.ShouldMoveFieldsBelow = false;
                }
                var fieldId = element.Attributes().FirstOrDefault(x => x.Name == "id");
                if (fieldId != null)
                {
                    dynPanel.FieldId = fieldId.Value;
                }
                var gVisible = firstRect.Attributes().FirstOrDefault(x => x.Name == "fdtpaneltype");
                var repeating = firstRect.Attributes().FirstOrDefault(x => x.Name == "fdtrepeating");
                dynPanel.InitiallyVisible = gVisible == null || gVisible.Value != "conditional";
                setPanelElementTriggers(firstRect, dynPanel);
                dynPanel.RepeatingPanel = repeating != null && repeating.Value == "true";
                var r = new RectElement(firstRect);
                var panelList = new List<object>();
                dynPanel.RectArea = r;

                foreach (var elementChild in element.Elements().ToList())
                {
                    processElement(elementChild, panelList);
                }

                dynPanel.Children = panelList;
                if (list == null)
                {
                    Panels.Add(dynPanel);
                } else
                {
                    list.Add(dynPanel);
                }

                return;
            }

            //group
            foreach (var elementChild in element.Elements().ToList())
            {
                processElement(elementChild, list);
            }

        }

        private void setPanelElementTriggers(XElement element, DynamicPanelDescriptor panel)
        {
            var triggers = new Dictionary<string, bool>();
            var panelFields = element.Attributes().FirstOrDefault(x => x.Name == "fdtconditionalfields");
            if (panelFields != null)
            {
                var s = panelFields.Value
                    .Replace(" and ", "&")
                    .Replace(" or ", "|")
                    .Replace(";", "&")
                    .Replace(",", "|")
                    .Replace(" ", "");
                if (s.Contains("|") || s.Contains("&"))
                {
                    panel.AndTriggers = s.Contains("&");
                    var split = s.Split(panel.AndTriggers ? '&' : '|');
                    foreach (var st in split)
                    {
                        triggers[st] = false;
                    }
                }
                else
                {
                    if (s == "") return;
                    triggers[s] = false;
                }
                
            }
            panel.panelTriggers = triggers;
            if (panel.TriggersNegated == null)
            {
                panel.TriggersNegated = new Dictionary<string, bool>();
            }
            foreach (var key in panel.panelTriggers.Keys)
            {
                panel.TriggersNegated[key] = false;
            }
        }

        public bool ShouldShow ()
        {
            if (!PageTriggers.Any())
            {
                return Visible;
            }

            if (AndTriggers)
            {
                foreach (var s in PageTriggers.Keys.ToList())
                {
                    var b = PageTriggers[s];
                    if (!b && s.Contains("!"))
                    {
                        return false;
                    }
                    if (b && !s.Contains("!"))
                    {
                        return false;
                    }
                }
                return true;
            }

            // 'Or' Triggers
            foreach (var s in PageTriggers.Keys.ToList())
            {
                var b = PageTriggers[s];
                if (b && !s.Contains("!"))
                {
                    return true;
                }
                if (!b && s.Contains("!"))
                {
                    return true;
                }
            }
            return false;
        }

        public bool FieldIsOnPage(string fieldId)
        {
            var flds = FieldDescriptors.Any(x => x.FieldId == fieldId);
            if (flds) return true;
            return RadioGroups.Keys.Any(x => x == fieldId);
        }

        public void NullAll ()
        {
            FieldDescriptors = null;
        }

        private void addItem(object item, List<object> list)
        {
            if (list == null)
            {
                FieldDescriptors.Add((FieldDescriptor)item);
            } else
            {
                list.Add(item);
            }
        }
        
    }
}
