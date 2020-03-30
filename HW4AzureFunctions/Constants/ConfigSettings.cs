using System;
using System.Collections.Generic;
using System.Text;

namespace HW4AzureFunctions
{
    class ConfigSettings
    {
        public const string STORAGE_CONNECTIONSTRING_NAME = "AzureWebJobsStorage";

        public const string TO_GREY_SCALE_CONTAINER_NAME = "converttogreyscale";

        public const string TO_SEPIA_CONTAINER_NAME = "converttosepia";

        public const string CONVERTED_IMAGES_CONTAINER_NAME = "convertedimages";

        public const string FAILED_IMAGES_CONTAINER_NAME = "failedimages";

        public const string JOBID_METADATA_NAME = "JobId";

        public const string IMAGE_CONVERSION_MODE_METADATA_NAME = "ImageConversionMode";

        public const string IMAGE_SOURCE_METADATA_NAME = "ImageSource";

        public const string IMAGEJOBS_PARTITIONKEY = "ImageJobs";

        public const string JOBS_TABLENAME = "jobs";

        public const string STORAGE_DOMAIN_METADATA_NAME = "STORAGE_DOMAIN";

    }
}
