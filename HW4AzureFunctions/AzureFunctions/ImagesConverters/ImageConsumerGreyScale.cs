using System;
using System.IO;
using HW4AzureFunctions.AzureFunctions.ImagesConverters;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace HW4AzureFunctions
{
    public static class ImageConsumerGreyScale
    {

        const string route = "converttogreyscale/{name}";

        /// <summary>
        /// This function is triggered when a new blob is uploaded to
        /// the converttogreyscale container. An entry is placed in the jobs table
        /// to indicate that the image is obtained. A new image with the applied grey scale
        /// filter is created. The status of the table entry is then updated to reflect that
        /// the image conversion is in progress. Finally, the new image is sent to
        /// the convertedimages container. If an error occurs during this process,
        /// the original image is uploaded to the failedimages container.
        /// </summary>
        /// <param name="myBlob"></param>
        /// <param name="name"></param>
        /// <param name="log"></param>
        [FunctionName("imageconsumergreyscale")]
        public static async void Run([BlobTrigger(route, Connection = ConfigSettings.STORAGE_CONNECTIONSTRING_NAME)]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation("[PENDING] Connecting to blob storage...");
            BlobStorage blobStorage = new BlobStorage();
            log.LogInformation("[SUCCESS] Connected to blob storage");

            string imageSourceURI = $"{Environment.GetEnvironmentVariable(ConfigSettings.STORAGE_DOMAIN_METADATA_NAME)}/{ConfigSettings.TO_GREY_SCALE_CONTAINER_NAME}/{name}";

            string jobId = Guid.NewGuid().ToString();

            JobEntity initialJobEntity = JobEntity.New(jobId, ConversionModeNames.GREY_SCALE, JobStatusCodes.IMAGE_OBTAINED, JobStatusMessages.IMAGE_OBTAINED, imageSourceURI, "");

            log.LogInformation("[PENDING] Adding initial job entry to table...");
            await Shared.UpdateJobTableWithStatus(log, initialJobEntity);
            log.LogInformation("[SUCCESS] Initial job entry added to table");

            string convertedBlobName = $"{Guid.NewGuid()}-{name}";

            try
            {
                log.LogInformation("[PENDING] Applying grey scale filter to image...");
                MemoryStream convertedMemoryStream = ImageConverter.ConvertImageToGreyScale(myBlob);
                log.LogInformation("[SUCCESS] Applied grey scale filter to image");

                JobEntity convertInProgressJobEntity = JobEntity.New(jobId, ConversionModeNames.GREY_SCALE, JobStatusCodes.BEING_CONVERTED, JobStatusMessages.BEING_CONVERTED, imageSourceURI, "");

                log.LogInformation("[PENDING] Updating job status to conversion-in-progress");
                await Shared.UpdateJobTableWithStatus(log, convertInProgressJobEntity);
                log.LogInformation("[SUCCESS] Job status updated to conversion-in-progress");

                log.LogInformation("[PENDING] Uploading converted image to converted images container...");
                blobStorage.UploadConvertedImage(initialJobEntity, convertedBlobName, convertedMemoryStream);
                log.LogInformation("[SUCCESS] Converted image uploaded to converted images container");
            }
            catch (Exception e)
            {
                log.LogError("An error occured while converting the image");

                try
                {
                    log.LogInformation("[PENDING] Uploading original image to failed container...");
                    blobStorage.UploadFailedImage(initialJobEntity, convertedBlobName, myBlob);
                    log.LogInformation("[SUCCESS] Original image uploaded to failed container");
                }
                catch (Exception ex)
                {
                    log.LogError("Failed to upload image to failed container");
                }
            }
        }
    }
}
