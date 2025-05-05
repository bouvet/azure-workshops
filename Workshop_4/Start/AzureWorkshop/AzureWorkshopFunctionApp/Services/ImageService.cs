using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.IO;
using System;
using AzureWorkshopFunctionApp.Interfaces;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Services
{
    public class ImageService : IImageService
    {
        public bool SoftBlurImage { get; set; } = true;

        public Stream FlipHorizontal(Stream image)
        {
            return ProcessImage(image, ctx => ctx.Flip(FlipMode.Horizontal));
        }

        public Stream FlipVertical(Stream image)
        {
            return ProcessImage(image, ctx => ctx.Flip(FlipMode.Vertical));
        }

        public Stream RotateClockwise(Stream image)
        {
            return ProcessImage(image, ctx => ctx.Rotate(RotateMode.Rotate90));
        }

        public Stream RotateAntiClockwise(Stream image)
        {
            return ProcessImage(image, ctx => ctx.Rotate(RotateMode.Rotate270));
        }

        public Stream GreyScale(Stream image)
        {
            return ProcessImage(image, ctx => ctx.Grayscale());
        }

        public Stream Square(Stream image)
        {
            return ProcessImage(image, ctx =>
            {
                var size = Math.Min(ctx.GetCurrentSize().Width, ctx.GetCurrentSize().Height);
                return ctx.Crop(new Rectangle(
                    (ctx.GetCurrentSize().Width - size) / 2,
                    (ctx.GetCurrentSize().Height - size) / 2,
                    size,
                    size
                ));
            });
        }

        public Stream Blur(Stream image)
        {
            return ProcessImage(image, ctx =>
            {
                var amount = SoftBlurImage ? 5f : 10f;
                return ctx.GaussianBlur(amount);
            });
        }

        private Stream ProcessImage(Stream inputStream, Func<IImageProcessingContext, IImageProcessingContext> operation)
        {
            var outputStream = new MemoryStream();

            try
            {
                using (var image = Image.Load(inputStream))
                {
                    image.Mutate(x => operation(x));
                    image.Save(outputStream, new JpegEncoder());
                }

                outputStream.Position = 0;
                return outputStream;
            }
            catch
            {
                outputStream.Dispose();
                throw;
            }
        }

        // Async version
        private async Task<Stream> ProcessImageAsync(Stream inputStream, Func<IImageProcessingContext, IImageProcessingContext> operation)
        {
            var outputStream = new MemoryStream();

            try
            {
                using (var image = await Image.LoadAsync(inputStream))
                {
                    image.Mutate(x => operation(x));
                    await image.SaveAsJpegAsync(outputStream);
                }

                outputStream.Position = 0;
                return outputStream;
            }
            catch
            {
                await outputStream.DisposeAsync();
                throw;
            }
        }
    }
}