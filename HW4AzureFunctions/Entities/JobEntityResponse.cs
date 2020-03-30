namespace HW4AzureFunctions
{
    public class JobEntityResponse
    {
        public string JobId { get; set; }

        public string ImageConversionMode { get; set; }

        public int Status { get; set; }

        public string StatusDescription { get; set; }

        public string ImageSource { get; set; }

        public string ImageResult { get; set; }

        public static JobEntityResponse New(string JobId, string ImageConversionMode, int Status, string StatusDescription, string ImageSource, string ImageResult)
        {
            return new JobEntityResponse()
            {
                JobId = JobId,
                ImageConversionMode = ImageConversionMode,
                Status = Status,
                StatusDescription = StatusDescription,
                ImageSource = ImageSource,
                ImageResult = ImageResult,
            };
        }
    }
}
