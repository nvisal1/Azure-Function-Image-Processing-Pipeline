using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace HW4AzureFunctions
{
    public static class ConvertedImageRemover
    {
        /// <summary>
        /// This function is triggered every 2 minutes.
        /// All JobEntities with a status of 3 are retrieved from
        /// the jobs table. The entities are then used to delete
        /// the corresponding user-uploaded file in converttogreyscale or
        /// converttosepia.
        /// </summary>
        /// <param name="myTimer"></param>
        /// <param name="log"></param>
        [FunctionName("ConvertedImageRemover")]
        public static void Run([TimerTrigger("*/2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("[PENDING] Connecting to jobs table...");
            JobTable jobTable = new JobTable(log, ConfigSettings.IMAGEJOBS_PARTITIONKEY);
            log.LogInformation("[SUCCESS] Connected to Jobs Table");


            log.LogInformation("[PENDING] Searching for successful jobs...");
            List<JobEntity> jobEntityList = jobTable.RetrieveAllSuccessJobEntities();
            log.LogInformation("[SUCCESS] Successful jobs found");


            log.LogInformation("[PENDING] Connecting to blob storage...");
            BlobStorage blobStorage = new BlobStorage();
            log.LogInformation("[SUCCESS] Connected to blob storage");


            log.LogInformation("[PENDING] Deleting successful jobs...");
            blobStorage.DeleteConvertedImages(jobEntityList);
            log.LogInformation("[SUCCESS] Successful jobs deleted");
        }
    }
}
