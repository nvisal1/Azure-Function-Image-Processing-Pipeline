using Microsoft.WindowsAzure.Storage.Table;
using System.ComponentModel.DataAnnotations;

namespace HW4AzureFunctions
{
    /// <summary>
    /// Table entity used for saving data in the
    /// job table
    /// </summary>
    public class JobEntity : TableEntity
    {
        /// <summary>
        /// The id of the job. 
        /// 
        /// Note: This is not the id assigned to
        /// converted images
        /// </summary>
        [MaxLength(36)]
        public string JobId { get; set; }

        /// <summary>
        /// This is the image conversion mode that the job
        /// will perform
        /// 
        /// Valid values are:
        ///     GreyScale
        ///     Sepia
        /// </summary>
        public string ImageConversionMode { get; set; }

        /// <summary>
        /// A number indicating the status of the job
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// If no error occured this value contains
        /// the text associated with the status defined
        /// above
        /// 
        /// Ex: "Conversion completed with success"
        /// 
        /// If an error occurred, a human readable
        /// description of the problem should be
        /// provided.
        /// 
        /// Always prefix this with "Job failed:"
        /// 
        /// This value is not intended to contain a stack trace
        /// </summary>
        [MaxLength(512)]
        public string StatusDescription { get; set; }

        /// <summary>
        /// This is the URL to the blob storage entry 
        /// for the image uploaded to be converted
        /// </summary>
        [MaxLength(512)]
        public string ImageSource { get; set; }

        /// <summary>
        /// This is the URL to the blob storage entry 
        /// for the blob that contains the converted
        /// or failed to be converted image
        /// </summary>
        [MaxLength(512)]
        public string ImageResult { get; set; }

        public static JobEntity New(string JobId, string ImageConversionMode, int Status, string StatusDescription, string ImageSource, string ImageResult)
        {
            return new JobEntity()
            {
                JobId = JobId,
                ImageConversionMode = ImageConversionMode,
                Status = Status,
                StatusDescription = StatusDescription,
                ImageSource = ImageSource,
                ImageResult = ImageResult,
            };
        }
    }
}
