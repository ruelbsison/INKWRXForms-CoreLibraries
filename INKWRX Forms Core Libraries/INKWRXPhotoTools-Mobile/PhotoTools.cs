using System.Collections.Generic;
using System.Threading.Tasks;

namespace INKWRXPhotoTools_Mobile
{
    public class PhotoTools
    {
        public interface IPhotoFileSystem
        {
            Task<ImageEntry> SaveImage(byte[] image, ImageEntry entry, string transactionId);
            Task<List<ImageEntry>> GetGalleryImages();
            Task<List<ImageEntry>> GetCameraImages(string transactionId);
            Task<byte[]> GetImage(ImageEntry entry);
            Task<byte[]> GetImage(ImageEntry entry, int maxX, int maxY);
            Task<ImageEntry> MoveCameraImage(ImageEntry entry, string transactionId);
            void ClearNoTransactionFolder();
        }

    }
}
