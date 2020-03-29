using System.Threading.Tasks;
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
        [FunctionName("ConversionJobStatus")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "jobs")] HttpRequest req, ILogger log)
        {
            JobTable jobTable = new JobTable(log, ConfigSettings.IMAGEJOBS_PARTITIONKEY);

            List<JobEntityResponse> jobEntityList = jobTable.RetrieveAllJobEntities();

            return new OkObjectResult(jobEntityList);
        }
    }
}
