using System;
using System.Drawing;

namespace Ideastrike.Helpers
{
    public static class ImageExtensions
    {
        public static Image ToThumbnail(this Image image, int desiredHeight)
        {
            var targetWidth = (desiredHeight * image.Width) / image.Height;
            return image.GetThumbnailImage(targetWidth, desiredHeight, () => false, IntPtr.Zero);
        }
    }
}