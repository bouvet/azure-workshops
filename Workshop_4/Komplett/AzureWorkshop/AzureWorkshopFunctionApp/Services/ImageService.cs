using AzureWorkshopFunctionApp.Interfaces;
using System;
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

            ColorMatrix colorMatrix = new ColorMatrix( new float[][]
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

        public Stream SquareImage(Stream image, ImageFormat format)
        {
            var bitmap = new Bitmap(image);

            var minSize = Math.Min(bitmap.Width, bitmap.Height);

            var squareBmp = new Bitmap(minSize, minSize);

            using Graphics graphics = Graphics.FromImage(squareBmp);
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            graphics.DrawImage(bitmap,(minSize / 2) - (bitmap.Width / 2) , (minSize / 2)-(bitmap.Height / 2), bitmap.Width, bitmap.Height);

            var ms = new MemoryStream();
            squareBmp.Save(ms, format);
            ms.Position = 0;
            return ms;
        }


    }
}
