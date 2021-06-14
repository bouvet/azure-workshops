using AzureWorkshopFunctionApp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

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
            ms.Position = 0;
            return ms;
        }

        public Stream FlipVertical(Stream image, ImageFormat format)
        {
            var bitmap = new Bitmap(image);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            var ms = new MemoryStream();
            bitmap.Save(ms, format);
            ms.Position = 0;
            return ms;
        }

        public Stream RotateClockwise(Stream image, ImageFormat format)
        {
            var bitmap = new Bitmap(image);
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var ms = new MemoryStream();
            bitmap.Save(ms, format);
            ms.Position = 0;
            return ms;
        }

        public Stream RotateAntiClockwise(Stream image, ImageFormat format)
        {
            var bitmap = new Bitmap(image);
            bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
            var ms = new MemoryStream();
            bitmap.Save(ms, format);
            ms.Position = 0;
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
            ms.Position = 0;
            return ms;
        }

    }
}
