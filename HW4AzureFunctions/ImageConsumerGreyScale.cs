using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace HW4AzureFunctions
{
    public static class ImageConsumerGreyScale
    {

        const string route = "converttogreyscale/{name}";

        [FunctionName("imageconsumergreyscale")]
        public static async void Run([BlobTrigger(route, Connection = ConfigSettings.STORAGE_CONNECTIONSTRING_NAME)]Stream myBlob, string name, ILogger log)
        {
            string storageConnectionString = Environment.GetEnvironmentVariable(ConfigSettings.STORAGE_CONNECTIONSTRING_NAME);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            JobEntity initialJobEntity = new JobEntity()
            {
                JobId = Guid.NewGuid().ToString(),
                ImageConversionMode = "GreyScale",
                Status = 1,
                StatusDescription = "Image Obtained",
                ImageSource = $"{Environment.GetEnvironmentVariable(ConfigSettings.STORAGE_DOMAIN_METADATA_NAME)}/{ConfigSettings.TO_GREY_SCALE_CONTAINER_NAME}/{name}",
                ImageResult = "",
            };

            await UpdateJobTableWithStatus(log, initialJobEntity);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer convertedImagesContainer = blobClient.GetContainerReference(ConfigSettings.CONVERTED_IMAGES_CONTAINER_NAME);

            CloudBlobContainer failedImagesContainer = blobClient.GetContainerReference(ConfigSettings.FAILED_IMAGES_CONTAINER_NAME);

            string convertedBlobName = $"{Guid.NewGuid()}-{name}";

            try
            {
                myBlob.Seek(0, SeekOrigin.Begin);

                using (MemoryStream convertedMemoryStream = new MemoryStream())
                using (Image<Rgba32> image = (Image<Rgba32>)Image.Load(myBlob))
                {
                    image.Mutate(x => x.Grayscale());
                    image.SaveAsJpeg(convertedMemoryStream);

                    convertedMemoryStream.Seek(0, SeekOrigin.Begin);

                  
                    CloudBlockBlob convertedBlockBlob = convertedImagesContainer.GetBlockBlobReference(convertedBlobName);

                    JobEntity convertInProgressJobEntity = new JobEntity()
                    {
                        JobId = initialJobEntity.JobId,
                        ImageConversionMode = "GreyScale",
                        Status = 2,
                        StatusDescription = "Image Being Converted",
                        ImageSource = initialJobEntity.ImageSource,
                        ImageResult = "",
                    };

                    await UpdateJobTableWithStatus(log, convertInProgressJobEntity);

                    convertedBlockBlob.Metadata.Add(ConfigSettings.JOBID_METADATA_NAME, initialJobEntity.JobId);
                    convertedBlockBlob.Metadata.Add(ConfigSettings.IMAGE_CONVERSION_MODE_METADATA_NAME, initialJobEntity.ImageConversionMode);
                    convertedBlockBlob.Metadata.Add(ConfigSettings.IMAGE_SOURCE_METADATA_NAME, initialJobEntity.ImageSource);
                    convertedBlockBlob.Properties.ContentType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                    await convertedBlockBlob.UploadFromStreamAsync(convertedMemoryStream);
                }
            }
            catch (Exception e)
            {
                try
                {
                    CloudBlockBlob failedBlobBlock = failedImagesContainer.GetBlockBlobReference(convertedBlobName);

                    failedBlobBlock.Metadata.Add(ConfigSettings.JOBID_METADATA_NAME, initialJobEntity.JobId);
                    failedBlobBlock.Metadata.Add(ConfigSettings.IMAGE_CONVERSION_MODE_METADATA_NAME, initialJobEntity.ImageConversionMode);
                    failedBlobBlock.Metadata.Add(ConfigSettings.IMAGE_SOURCE_METADATA_NAME, initialJobEntity.ImageSource);
                    myBlob.Seek(0, SeekOrigin.Begin);
                    await failedBlobBlock.UploadFromStreamAsync(myBlob);
                }
                catch (Exception ex)
                {
                    log.LogError("Failed to upload image to failed container");
                }
            }
        }

        private static async Task UpdateJobTableWithStatus(ILogger log, JobEntity jobEntity)
        {
            JobTable jobTable = new JobTable(log, ConfigSettings.IMAGEJOBS_PARTITIONKEY);
            await jobTable.InsertOrReplaceJobEntity(jobEntity);
        }
    }
}
