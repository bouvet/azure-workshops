using System.Drawing.Imaging;
using System.IO;

namespace AzureWorkshopFunctionApp.Interfaces
{
    public interface IImageService
    {
        bool SoftBlurImage { get; set; }
        Stream FlipHorizontal(Stream image);

        Stream FlipVertical(Stream image);

        Stream RotateClockwise(Stream image);

        Stream RotateAntiClockwise(Stream image);

        Stream GreyScale(Stream image);

        Stream Square(Stream image);

        Stream Blur(Stream image);
    }
}
