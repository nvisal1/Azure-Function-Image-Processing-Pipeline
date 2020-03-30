namespace HW4AzureFunctions
{
    /// <summary>
    /// DTO for job entity. This object is
    /// not meant to be saved in the jobs table.
    /// 
    /// It is intended for displaying job entity
    /// information to the client.
    /// </summary>
    public class JobEntityResponse
    {
        /// <summary>
        /// The id of the job. 
        /// 
        /// Note: This is not the id assigned to
        /// converted images
        /// </summary>
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
        public string StatusDescription { get; set; }

        /// <summary>
        /// This is the URL to the blob storage entry 
        /// for the image uploaded to be converted
        /// </summary>
        public string ImageSource { get; set; }

        /// <summary>
        /// This is the URL to the blob storage entry 
        /// for the blob that contains the converted
        /// or failed to be converted image
        /// </summary>
        public string ImageResult { get; set; }

        public static JobEntityResponse New(string JobId, string ImageConversionMode, int Status, string StatusDescription, string ImageSource, string ImageResult)
        {
            return new JobEntityResponse()
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
