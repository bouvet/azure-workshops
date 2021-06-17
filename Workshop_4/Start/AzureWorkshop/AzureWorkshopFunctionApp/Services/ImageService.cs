using AzureWorkshopFunctionApp.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AzureWorkshopFunctionApp.Services
{
    public class ImageService : IImageService
    {

        public Stream FlipHorizontal(Stream image, ImageFormat format)
        {
            var bitmap = new Bitmap(image);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            var ms = new MemoryStream();
            bitmap.Save(ms, format);
            return ms;
        }

        public Stream FlipVertical(Stream image, ImageFormat format)
        {
            var bitmap = new Bitmap(image);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            var ms = new MemoryStream();
            bitmap.Save(ms, format);
            return ms;
        }

        public Stream RotateClockwise(Stream image, ImageFormat format)
        {
            var bitmap = new Bitmap(image);
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var ms = new MemoryStream();
            bitmap.Save(ms, format);
            return ms;
        }

        public Stream RotateAntiClockwise(Stream image, ImageFormat format)
        {
            var bitmap = new Bitmap(image);
            bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
            var ms = new MemoryStream();
            bitmap.Save(ms, format);
            return ms;
        }

        public Stream GreyScale(Stream image, ImageFormat format)
        {
            var bitmap = new Bitmap(image);
            var greyBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
               {
                         new float[] {.3f, .3f, .3f, 0, 0},
                         new float[] {.59f, .59f, .59f, 0, 0},
                         new float[] {.11f, .11f, .11f, 0, 0},
                         new float[] {0, 0, 0, 1, 0},
                         new float[] {0, 0, 0, 0, 1}
               });

            using var graphics = Graphics.FromImage(greyBitmap);
            using var attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);
            graphics.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attributes);

            var ms = new MemoryStream();
            greyBitmap.Save(ms, format);
            return ms;
        }

        public void Something()
        {
            var bitmap = new Bitmap(2, 2);
            var greyBitmap = new Bitmap(2, 2);
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int x = 0; x < bitmap.Height; x++)
                {
                    Color oc = bitmap.GetPixel(i, x);
                    int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                    Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                    greyBitmap.SetPixel(i, x, nc);
                }
            }
        }
    }
}
