using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace HW4AzureFunctions
{
    public static class ImageConsumerSepia
    {
        const string route = "converttosepia/{name}";

        /// <summary>
        /// This function is triggered when a new blob is uploaded to
        /// the converttosepia container. An entry is placed in the jobs table
        /// to indicate that the image is obtained. A new image with the applied sepia
        /// filter is created. The status of the table entry is then updated to reflect that
        /// the image conversion is in progress. Finally, the new image is sent to
        /// the convertedimages container. If an error occurs during this process,
        /// the original image is uploaded to the failedimages container.
        /// </summary>
        /// <param name="myBlob"></param>
        /// <param name="name"></param>
        /// <param name="log"></param>
        [FunctionName("ImageConsumerSepia")]
        public static async void Run([BlobTrigger(route, Connection = ConfigSettings.STORAGE_CONNECTIONSTRING_NAME)]Stream myBlob, string name, ILogger log)
        {
            BlobStorage blobStorage = new BlobStorage();

            string imageSourceURI = $"{Environment.GetEnvironmentVariable(ConfigSettings.STORAGE_DOMAIN_METADATA_NAME)}/{ConfigSettings.TO_SEPIA_CONTAINER_NAME}/{name}";

            string jobId = Guid.NewGuid().ToString();

            JobEntity initialJobEntity = JobEntity.New(jobId, ConversionModeNames.SEPIA, JobStatusCodes.IMAGE_OBTAINED, JobStatusMessages.IMAGE_OBTAINED, imageSourceURI, "");

            await UpdateJobTableWithStatus(log, initialJobEntity);

            string convertedBlobName = $"{Guid.NewGuid()}-{name}";

            try
            {
                MemoryStream convertedMemoryStream = ImageConverter.ConvertImageToSepia(myBlob);

                JobEntity convertInProgressJobEntity = JobEntity.New(jobId, ConversionModeNames.SEPIA, JobStatusCodes.BEING_CONVERTED, JobStatusMessages.BEING_CONVERTED, imageSourceURI, "");
          
                await UpdateJobTableWithStatus(log, convertInProgressJobEntity);

                blobStorage.UploadConvertedImage(initialJobEntity, convertedBlobName, convertedMemoryStream);
            }
            catch (Exception e)
            {
                try
                {
                    blobStorage.UploadFailedImage(initialJobEntity, convertedBlobName, myBlob);
                }
                catch (Exception ex)
                {
                    log.LogError("Failed to upload image to failed container");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <param name="jobEntity"></param>
        /// <returns></returns>
        private static async Task UpdateJobTableWithStatus(ILogger log, JobEntity jobEntity)
        {
            JobTable jobTable = new JobTable(log, ConfigSettings.IMAGEJOBS_PARTITIONKEY);
            await jobTable.InsertOrReplaceJobEntity(jobEntity);
        }
    }
}
