using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace HW4AzureFunctions
{
    public static class ImageStatusUpdaterFailed
    {
        const string route = "failedimages/{name}";

        [FunctionName("ImageStatusUpdaterFailed")]
        public static async void Run([BlobTrigger(route, Connection = ConfigSettings.STORAGE_CONNECTIONSTRING_NAME)]CloudBlockBlob blockBlob, string name, ILogger log)
        {
            await blockBlob.FetchAttributesAsync();
            if (blockBlob.Metadata.ContainsKey(ConfigSettings.JOBID_METADATA_NAME))
            {
                string jobId = blockBlob.Metadata[ConfigSettings.JOBID_METADATA_NAME];

                string imageConversionMode = blockBlob.Metadata[ConfigSettings.IMAGE_CONVERSION_MODE_METADATA_NAME];

                string imageSource = blockBlob.Metadata[ConfigSettings.IMAGE_SOURCE_METADATA_NAME];

                JobTable jobTable = new JobTable(log, ConfigSettings.IMAGEJOBS_PARTITIONKEY);

                JobEntity failedJobEntity = new JobEntity()
                {
                    JobId = jobId,
                    ImageConversionMode = imageConversionMode,
                    Status = 4,
                    StatusDescription = "Image Failed Conversion",
                    ImageSource = imageSource,
                    ImageResult = $"{Environment.GetEnvironmentVariable(ConfigSettings.STORAGE_DOMAIN_METADATA_NAME)}/{ConfigSettings.CONVERTED_IMAGES_CONTAINER_NAME}/{name}",
                };

                await jobTable.UpdateJobEntityStatus(failedJobEntity);
            }
            else
            {
                log.LogError("can't update the specified job");
            }

        }
    }
}
