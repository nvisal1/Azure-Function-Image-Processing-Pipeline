using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace HW4AzureFunctions
{
    public static class ImageStatusUpdaterFailed
    {
        const string route = "failedimages/{name}";

        /// <summary>
        /// This function is triggered when a new blob is uploaded to the
        /// failedimages container. The corresponding entry in the
        /// jobs table is updated with a failed status. The status will not
        /// be updated if the corresponding table entry does not
        /// contain the necessary metadata properties.
        /// </summary>
        /// <param name="blockBlob"></param>
        /// <param name="name"></param>
        /// <param name="log"></param>
        [FunctionName("ImageStatusUpdaterFailed")]
        public static async void Run([BlobTrigger(route, Connection = ConfigSettings.STORAGE_CONNECTIONSTRING_NAME)]CloudBlockBlob blockBlob, string name, ILogger log)
        {
            await blockBlob.FetchAttributesAsync();

            log.LogInformation("[PENDING] Checking for required metadata on job entry...");
            if (blockBlob.Metadata.ContainsKey(ConfigSettings.JOBID_METADATA_NAME) && blockBlob.Metadata.ContainsKey(ConfigSettings.IMAGE_CONVERSION_MODE_METADATA_NAME) && blockBlob.Metadata.ContainsKey(ConfigSettings.IMAGE_SOURCE_METADATA_NAME))
            {

                log.LogInformation("[SUCCESS] job entry contains required metadata");

                string jobId = blockBlob.Metadata[ConfigSettings.JOBID_METADATA_NAME];

                string imageConversionMode = blockBlob.Metadata[ConfigSettings.IMAGE_CONVERSION_MODE_METADATA_NAME];

                string imageSource = blockBlob.Metadata[ConfigSettings.IMAGE_SOURCE_METADATA_NAME];

                log.LogInformation("[PENDING] Connecting to jobs table...");
                JobTable jobTable = new JobTable(log, ConfigSettings.IMAGEJOBS_PARTITIONKEY);
                log.LogInformation("[SUCCESS] Connected to Jobs Table");

                string imageResult = $"{Environment.GetEnvironmentVariable(ConfigSettings.STORAGE_DOMAIN_METADATA_NAME)}/{ConfigSettings.CONVERTED_IMAGES_CONTAINER_NAME}/{name}";

                JobEntity failedJobEntity = JobEntity.New(jobId, imageConversionMode, JobStatusCodes.CONVERT_FAIL, JobStatusMessages.CONVERT_FAIL, imageSource, imageResult);

                log.LogInformation("[PENDING] Updating job entry with failed status...");
                await jobTable.UpdateJobEntityStatus(failedJobEntity);
                log.LogInformation("[SUCCESS] Job entry updated with failed status");
            }
            else
            {
                log.LogError("The specified job does not contain the required metadata and cannot be updated.");
            }

        }
    }
}
