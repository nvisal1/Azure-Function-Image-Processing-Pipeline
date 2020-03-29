using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace HW4AzureFunctions
{
    public static class ConvertedImageRemover
    {
        [FunctionName("ConvertedImageRemover")]
        public static async void Run([TimerTrigger("*/2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            // Get all successful jobs from table
            JobTable jobTable = new JobTable(log, ConfigSettings.IMAGEJOBS_PARTITIONKEY);

            List<JobEntity> jobEntityList = jobTable.RetrieveAllSuccessJobEntities();

            string storageConnectionString = Environment.GetEnvironmentVariable(ConfigSettings.STORAGE_CONNECTIONSTRING_NAME);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer convertToGreyScaleContainer = blobClient.GetContainerReference(ConfigSettings.TO_GREY_SCALE_CONTAINER_NAME);
            CloudBlobContainer convertToSepiaContainer = blobClient.GetContainerReference(ConfigSettings.TO_SEPIA_CONTAINER_NAME);

            foreach (JobEntity jobEntity in jobEntityList)
            {

                string[] imageSourceArray = jobEntity.ImageSource.Split("/");

                string imageName = imageSourceArray[imageSourceArray.Length - 1];

                if (jobEntity.ImageConversionMode.Equals("Sepia"))
                {
                    CloudBlockBlob blob = convertToSepiaContainer.GetBlockBlobReference(imageName);
                    await blob.DeleteIfExistsAsync();
                }
                else if (jobEntity.ImageConversionMode.Equals("GreyScale"))
                {
                    CloudBlockBlob blob = convertToGreyScaleContainer.GetBlockBlobReference(imageName);
                    await blob.DeleteIfExistsAsync();
                }
            }
        }
    }
}
