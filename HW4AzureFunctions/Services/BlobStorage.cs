
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;

namespace HW4AzureFunctions
{
    /// <summary>
    /// Contains all logic for managing blobs
    /// in Azure Blob Storage
    /// </summary>
    public class BlobStorage
    {
        private CloudStorageAccount _storageAccount;

        private CloudBlobClient _blobClient;

        public BlobStorage()
        {
            string storageConnectionString = Environment.GetEnvironmentVariable(ConfigSettings.STORAGE_CONNECTIONSTRING_NAME);
            _storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            _blobClient = _storageAccount.CreateCloudBlobClient();
        }

        /// <summary>
        /// Accepts a list of Job Entities. Each entity
        /// is deleted from its origin container.
        /// 
        /// The given list should only contain job entities that
        /// are completed
        /// </summary>
        /// <param name="jobEntityList"></param>
        public async void DeleteConvertedImages(List<JobEntity> jobEntityList)
        {
            CloudBlobContainer convertToGreyScaleContainer = _blobClient.GetContainerReference(ConfigSettings.TO_GREY_SCALE_CONTAINER_NAME);
            CloudBlobContainer convertToSepiaContainer = _blobClient.GetContainerReference(ConfigSettings.TO_SEPIA_CONTAINER_NAME);

            foreach (JobEntity jobEntity in jobEntityList)
            {

                string[] imageSourceArray = jobEntity.ImageSource.Split("/");

                string imageName = imageSourceArray[imageSourceArray.Length - 1];

                if (jobEntity.ImageConversionMode.Equals(ConversionModeNames.SEPIA))
                {
                    CloudBlockBlob blob = convertToSepiaContainer.GetBlockBlobReference(imageName);
                    await blob.DeleteIfExistsAsync();
                }
                else if (jobEntity.ImageConversionMode.Equals(ConversionModeNames.GREY_SCALE))
                {
                    CloudBlockBlob blob = convertToGreyScaleContainer.GetBlockBlobReference(imageName);
                    await blob.DeleteIfExistsAsync();
                }
            }
        }

        /// <summary>
        /// Uploads a given image to the converted images container
        /// 
        /// The given image should already have the desired filter applied
        /// </summary>
        /// <param name="jobEntity"></param>
        /// <param name="blobName"></param>
        /// <param name="convertedImage"></param>
        public async void UploadConvertedImage(JobEntity jobEntity, string blobName, MemoryStream convertedImage)
        {
            CloudBlobContainer convertedImagesContainer = _blobClient.GetContainerReference(ConfigSettings.CONVERTED_IMAGES_CONTAINER_NAME);
            CloudBlockBlob convertedBlockBlob = convertedImagesContainer.GetBlockBlobReference(blobName);

            convertedBlockBlob.Metadata.Add(ConfigSettings.JOBID_METADATA_NAME, jobEntity.JobId);
            convertedBlockBlob.Metadata.Add(ConfigSettings.IMAGE_CONVERSION_MODE_METADATA_NAME, jobEntity.ImageConversionMode);
            convertedBlockBlob.Metadata.Add(ConfigSettings.IMAGE_SOURCE_METADATA_NAME, jobEntity.ImageSource);
            convertedBlockBlob.Properties.ContentType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
            await convertedBlockBlob.UploadFromStreamAsync(convertedImage);
        }

        /// <summary>
        /// Uploads a given image to the failed images container
        /// 
        /// The given image is expected to be the original source
        /// </summary>
        /// <param name="jobEntity"></param>
        /// <param name="blobName"></param>
        /// <param name="originalImage"></param>
        public async void UploadFailedImage(JobEntity jobEntity, string blobName, Stream originalImage)
        {
            CloudBlobContainer failedImagesContainer = _blobClient.GetContainerReference(ConfigSettings.FAILED_IMAGES_CONTAINER_NAME);
            CloudBlockBlob failedBlobBlock = failedImagesContainer.GetBlockBlobReference(blobName);

            failedBlobBlock.Metadata.Add(ConfigSettings.JOBID_METADATA_NAME, jobEntity.JobId);
            failedBlobBlock.Metadata.Add(ConfigSettings.IMAGE_CONVERSION_MODE_METADATA_NAME, jobEntity.ImageConversionMode);
            failedBlobBlock.Metadata.Add(ConfigSettings.IMAGE_SOURCE_METADATA_NAME, jobEntity.ImageSource);
            
            originalImage.Seek(0, SeekOrigin.Begin);
            await failedBlobBlock.UploadFromStreamAsync(originalImage);
        }
    }
}
