namespace AzureWorkshopFunctionApp
{
    public static class Constants
    {
        public const string ImageContainer = "imagecontainer";
        public const string BlurImageContainer = ImageContainer + "-blur";
        public const string GreyImageContainer = ImageContainer + "-grey";
        public const string SquareImageContainer = ImageContainer + "-square";
        public const string MirrorImageContainer = ImageContainer + "-mirror";
        public const string FlippedImageContainer = ImageContainer + "-flipped";
        public const string ClockwiseImageContainer = ImageContainer + "-clockwise";
        public const string AntiClockwiseImageContainer = ImageContainer + "-anticlockwise";

        public const string ImageQueue = "imagequeue";
        public const string GreyImageQueue = "greyimage";
        public const string SquareImageQueue = "squareimage";
        public const string ConnectionString = "AzureWebJobsStorage";
        public const string SBConnectionString = "ServiceBus";
    }
}
