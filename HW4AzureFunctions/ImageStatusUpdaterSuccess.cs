using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;


namespace HW4AzureFunctions
{
    public static class ImageStatusUpdaterSuccess
    {
        const string route = "convertedimages/{name}";

        [FunctionName("ImageStatusUpdaterSuccess")]
        public static async void Run([BlobTrigger(route, Connection = ConfigSettings.STORAGE_CONNECTIONSTRING_NAME)]CloudBlockBlob blockBlob, string name, ILogger log)
        {
            await blockBlob.FetchAttributesAsync();
         
            string jobId = blockBlob.Metadata[ConfigSettings.JOBID_METADATA_NAME];

            string imageConversionMode = blockBlob.Metadata[ConfigSettings.IMAGE_CONVERSION_MODE_METADATA_NAME];

            string imageSource = blockBlob.Metadata[ConfigSettings.IMAGE_SOURCE_METADATA_NAME];

            JobTable jobTable = new JobTable(log, ConfigSettings.IMAGEJOBS_PARTITIONKEY);

            JobEntity successJobEntity = new JobEntity()
            {
                JobId = jobId,
                ImageConversionMode = imageConversionMode,
                Status = 3,
                StatusDescription = "Image Converted with Success",
                ImageSource = imageSource,
                ImageResult = $"{Environment.GetEnvironmentVariable(ConfigSettings.STORAGE_DOMAIN_METADATA_NAME)}/{ConfigSettings.CONVERTED_IMAGES_CONTAINER_NAME}/{name}",
            };

            await jobTable.UpdateJobEntityStatus(successJobEntity);
            

            
        }
    }
}
