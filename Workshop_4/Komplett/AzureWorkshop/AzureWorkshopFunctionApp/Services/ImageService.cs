using AzureWorkshopFunctionApp.Interfaces;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace AzureWorkshopFunctionApp.Services
{
    public class ImageService : IImageService
    {
        //Mirror on left or right side
        public ImageFormat Format { get { return ImageFormat.Jpeg; } }
        public bool SoftBlurImage { get; set; } = true;

        public Stream FlipHorizontal(Stream image)
        {
            var bitmap = new Bitmap(image);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            var ms = new MemoryStream();
            bitmap.Save(ms, Format);
            ms.Position = 0;
            return ms;
        }

        //Mirror on top/bottom
        public Stream FlipVertical(Stream image)
        {
            var bitmap = new Bitmap(image);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            var ms = new MemoryStream();
            bitmap.Save(ms, Format);
            ms.Position = 0;
            return ms;
        }

        public Stream RotateClockwise(Stream image)
        {
            var bitmap = new Bitmap(image);
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var ms = new MemoryStream();
            bitmap.Save(ms, Format);
            ms.Position = 0;
            return ms;
        }

        public Stream RotateAntiClockwise(Stream image)
        {
            var bitmap = new Bitmap(image);
            bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
            var ms = new MemoryStream();
            bitmap.Save(ms, Format);
            ms.Position = 0;
            return ms;
        }

        public Stream GreyScale(Stream image)
        {
            var bitmap = new Bitmap(image);
            var greyBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
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
            greyBitmap.Save(ms, Format);
            ms.Position = 0;
            return ms;
        }

        //Makes the image square, loses some information in the process
        public Stream Square(Stream image)
        {
            var bitmap = new Bitmap(image);

            var minSize = Math.Min(bitmap.Width, bitmap.Height);

            var squareBmp = new Bitmap(minSize, minSize);

            using Graphics graphics = Graphics.FromImage(squareBmp);
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            graphics.DrawImage(bitmap, (minSize / 2) - (bitmap.Width / 2), (minSize / 2) - (bitmap.Height / 2), bitmap.Width, bitmap.Height);

            var ms = new MemoryStream();
            squareBmp.Save(ms, Format);
            ms.Position = 0;
            return ms;
        }

        public Stream Blur(Stream image)
        {
            return SoftBlurImage ? SoftBlur(image) : HardBlur(image);
        }

        private Stream SoftBlur(Stream image)
        {
            var bitmap = new Bitmap(image);
            var reduced = new Bitmap(bitmap.Width / 5, bitmap.Height / 5);
            var blurred = new Bitmap(bitmap.Width, bitmap.Height);

            using Graphics gr = Graphics.FromImage(reduced);
            gr.SmoothingMode = SmoothingMode.HighQuality;
            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gr.DrawImage(bitmap, new Rectangle(0, 0, reduced.Width, reduced.Height));

            using Graphics br = Graphics.FromImage(blurred);
            br.SmoothingMode = SmoothingMode.HighQuality;
            br.InterpolationMode = InterpolationMode.HighQualityBicubic;
            br.PixelOffsetMode = PixelOffsetMode.HighQuality;
            br.DrawImage(reduced, new Rectangle(0, 0, blurred.Width, blurred.Height));

            var ms = new MemoryStream();
            blurred.Save(ms, Format);
            ms.Position = 0;
            return ms;
        }

        private Stream HardBlur(Stream image)
        {
            var bitmap = new Bitmap(image);
            var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var blurSize = 3;
            Bitmap blurred = new Bitmap(bitmap.Width, bitmap.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);

            // look at every pixel in the blur rectangle
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the
                    // blur size while making sure you don't go outside the image bounds
                    for (int x = xx; (x < xx + blurSize && x < bitmap.Width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < bitmap.Height); y++)
                        {
                            Color pixel = blurred.GetPixel(x, y);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                        }
                    }

                    avgR /= blurPixelCount;
                    avgG /= blurPixelCount;
                    avgB /= blurPixelCount;

                    // now that we know the average for the blur size, set each pixel to that color
                    for (int x = xx; x < xx + blurSize && x < bitmap.Width && x < rectangle.Width; x++)
                        for (int y = yy; y < yy + blurSize && y < bitmap.Height && y < rectangle.Height; y++)
                            blurred.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                }
            }

            var ms = new MemoryStream();
            blurred.Save(ms, Format);
            ms.Position = 0;
            return ms;
        }
    }
}
