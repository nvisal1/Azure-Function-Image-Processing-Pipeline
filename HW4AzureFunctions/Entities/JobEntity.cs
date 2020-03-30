using Microsoft.WindowsAzure.Storage.Table;

namespace HW4AzureFunctions
{
    public class JobEntity : TableEntity
    {
        public string JobId { get; set; }

        public string ImageConversionMode { get; set; }

        public int Status { get; set; }

        public string StatusDescription { get; set; }

        public string ImageSource { get; set; }

        public string ImageResult { get; set; }

        public static JobEntity New(string JobId, string ImageConversionMode, int Status, string StatusDescription, string ImageSource, string ImageResult)
        {
            return new JobEntity()
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
