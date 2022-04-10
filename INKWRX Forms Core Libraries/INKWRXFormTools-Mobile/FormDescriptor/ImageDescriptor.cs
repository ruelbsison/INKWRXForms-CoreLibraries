using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FormTools.FormDescriptor
{
    public class ImageDescriptor : ElementDescriptor
    {
        private static string IMAGE_ID = "href";

        public string ImageId { get; set; }

        public ImageDescriptor(XElement element, int zOrder) : base(element, zOrder)
        {
            foreach (var att in element.Attributes().ToList().Where(x => x.Name.LocalName == IMAGE_ID))
            {
                var startIndex = att.Value.IndexOf("?id=");
                var endindex = att.Value.IndexOf("&amp", startIndex);

                if (startIndex == -1)
                {
                    this.ImageId = "";
                }

                if (endindex == -1)
                {
                    this.ImageId = att.Value.Substring(startIndex + 4);
                }
                else
                {
                    this.ImageId = att.Value.Substring(startIndex + 4, (endindex - (startIndex + 4)));
                }
            }
        }
    }
}
