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
    }
}
