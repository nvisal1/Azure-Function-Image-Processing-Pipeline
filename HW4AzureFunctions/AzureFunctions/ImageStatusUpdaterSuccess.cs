using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace HW4AzureFunctions
{
    public static class ImageStatusUpdaterSuccess
    {
        const string route = "convertedimages/{name}";

        /// <summary>
        /// This function is triggered when a new blob is uploaded to the
        /// convertedimages container. The corresponding entry in the
        /// jobs table is updated with a success status. The status will not
        /// be updated if the corresponding table entry does not
        /// contain the necessary metadata properties.
        /// </summary>
        /// <param name="blockBlob"></param>
        /// <param name="name"></param>
        /// <param name="log"></param>
        [FunctionName("ImageStatusUpdaterSuccess")]
        public static async void Run([BlobTrigger(route, Connection = ConfigSettings.STORAGE_CONNECTIONSTRING_NAME)] CloudBlockBlob blockBlob, string name, ILogger log)
        {
            await blockBlob.FetchAttributesAsync();

            if (blockBlob.Metadata.ContainsKey(ConfigSettings.JOBID_METADATA_NAME) && blockBlob.Metadata.ContainsKey(ConfigSettings.IMAGE_CONVERSION_MODE_METADATA_NAME) && blockBlob.Metadata.ContainsKey(ConfigSettings.IMAGE_SOURCE_METADATA_NAME))
            {
                string jobId = blockBlob.Metadata[ConfigSettings.JOBID_METADATA_NAME];

                string imageConversionMode = blockBlob.Metadata[ConfigSettings.IMAGE_CONVERSION_MODE_METADATA_NAME];

                string imageSource = blockBlob.Metadata[ConfigSettings.IMAGE_SOURCE_METADATA_NAME];

                JobTable jobTable = new JobTable(log, ConfigSettings.IMAGEJOBS_PARTITIONKEY);

                string imageResult = $"{Environment.GetEnvironmentVariable(ConfigSettings.STORAGE_DOMAIN_METADATA_NAME)}/{ConfigSettings.CONVERTED_IMAGES_CONTAINER_NAME}/{name}";

                JobEntity successJobEntity = JobEntity.New(jobId, imageConversionMode, JobStatusCodes.CONVERT_SUCCESS, JobStatusMessages.CONVERT_SUCCESS, imageSource, imageResult);

                await jobTable.UpdateJobEntityStatus(successJobEntity);
            }
            else
            {
                log.LogError("The specified job does not contain the required metadata and cannot be updated.");
            }
        }
    }
}
