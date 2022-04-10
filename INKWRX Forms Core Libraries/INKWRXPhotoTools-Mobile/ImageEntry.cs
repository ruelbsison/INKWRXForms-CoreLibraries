using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRXPhotoTools_Mobile
{
    public class ImageEntry
    {
        public enum ImageEntryType
        {
            Camera,
            Gallery
        }

        public ImageEntryType ImageType { get; set; }
        public string ImageReference { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Orientation { get; set; }
    }
}
