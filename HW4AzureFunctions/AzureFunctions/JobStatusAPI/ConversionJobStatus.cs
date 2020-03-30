using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace HW4AzureFunctions
{
    public static class ConversionJobStatus
    {
        /// <summary>
        /// This function is triggered when the /jobs endpoint is invoked.
        /// All JobEntities are retrieved from the jobs table. They are then
        /// returned to the client.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("ConversionJobStatus")]
        public static ActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "jobs")] HttpRequest req, ILogger log)
        {
            log.LogInformation("[PENDING] Connecting to jobs table...");
            JobTable jobTable = new JobTable(log, ConfigSettings.IMAGEJOBS_PARTITIONKEY);
            log.LogInformation("[SUCCESS] Connected to Jobs Table");

            log.LogInformation("[PENDING] Getting all job entities...");
            List<JobEntityResponse> jobEntityList = jobTable.RetrieveAllJobEntities();
            log.LogInformation("[SUCCESS] Retrieved all job entities");

            return new OkObjectResult(jobEntityList);
        }
    }
}
