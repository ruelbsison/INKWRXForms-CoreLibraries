// <copyright file="FormDescriptor.cs" company="Destiny Wireless">
// Copyright (c) 2017 All Rights Reserved
// <author>Jamie Duggan</author>
// </copyright>

namespace FormTools.FormDescriptor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class FormDescriptor
    {
        private XElement source;
        public long FormId { get; }
        public List<PageDescriptor> PageDescriptors = new List<PageDescriptor>();
        public double FormHeight { get; set; }
        public double FormWidth { get; set; }

        public Dictionary<string, List<string>> MandatoryCheckBoxGroups = new Dictionary<string, List<string>>();

        public List<FieldDescriptor> MandatoryFields = new List<FieldDescriptor>();
        public List<string> MandatoryRadioGroups = new List<string>();

        public List<CalcList> FormCalcFields = new List<CalcList>();

        public Dictionary<string, string> AllFieldsIds = new Dictionary<string, string>();

        public FormDescriptor (XElement src, bool onePage, long formId)
        {
            this.source = src;
            this.FormId = formId;
            var pageNumber = 1;
            var realPageNumber = 1;
            for ( ; pageNumber <= source.Elements().Count(); pageNumber++ )
            {
                var element = source.Elements().ToList()[pageNumber - 1];
                var page = new PageDescriptor(element, realPageNumber);
                if (page.Output) continue;

                AllFieldsIds.Concat(page.AllFieldsIds);
                PageDescriptors.Add(page);
                FormCalcFields.Concat(page.PageCalcFields);

                MandatoryFields.Concat(page.MandatoryFields);
                MandatoryRadioGroups.Concat(page.MandatoryRadioGroups);
                MandatoryCheckBoxGroups.Concat(page.MandatoryCheckBoxGroups);

                if (PageDescriptors.Count == 1)
                {
                    FormWidth = PageDescriptors.First().PageWidth;
                    FormHeight = PageDescriptors.First().PageHeight;
                }

                realPageNumber++;
                if (onePage) break;
            }
            FormCalcFields = CalcList.GetOrderedList(FormCalcFields.ToList());
        }

        public FormDescriptor(XDocument srcDoc, bool onePage, long formId) : this(srcDoc.Root, onePage, formId)
        {

        }

        public FormDescriptor(string xmlString, long formId) : this(XDocument.Parse(xmlString).Root, false, formId)
        {

        }

        public void NullAll()
        {
            foreach (var page in PageDescriptors)
            {
                page.NullAll();
            }
            PageDescriptors = null;
        }
    }
}
