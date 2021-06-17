namespace AzureWorkshopFunctionApp
{
    public static class Constants
    {
        public static string ImageContainer = "imagecontainer";
        public static string GreyImageContainer = ImageContainer + "-grey";
        public static string SquareImageContainer = ImageContainer + "-square";
        public static string MirrorImageContainer = ImageContainer + "-mirror";
        public static string FlippedImageContainer = ImageContainer + "-flipped";
        public static string ClockwiseImageContainer = ImageContainer + "-clockwise";
        public static string AntiClockwiseImageContainer = ImageContainer + "-anticlockwise";

        public const string GreyImageQueue = "greyimage";
        public const string SquareImageQueue = "squareimage";
        public const string ConnectionString = "AzureWebJobsStorage";
    }
}
