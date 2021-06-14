using System.Drawing.Imaging;
using System.IO;

namespace AzureWorkshopFunctionApp.Interfaces
{
    public interface IImageService
    {
        Stream FlipHorizontal(Stream image, ImageFormat format);

        Stream FlipVertical(Stream image, ImageFormat format);

        Stream RotateClockwise(Stream image, ImageFormat format);

        Stream RotateAntiClockwise(Stream image, ImageFormat format);

        Stream GreyScale(Stream image, ImageFormat format);

        Stream SquareImage(Stream image, ImageFormat format);
    }
}
