using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HW4AzureFunctions
{
    public static class ConversionJobStatusById
    {
        [FunctionName("ConversionJobStatusById")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "jobs/{id}")] HttpRequest req, string id, ILogger log)
        {      
            JobTable jobTable = new JobTable(log, ConfigSettings.IMAGEJOBS_PARTITIONKEY);

            JobEntity jobEntity = await jobTable.RetrieveJobEntity(id);

            if (jobEntity == null)
            {
                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorNumber = 3,
                    ParameterName = "jobId",
                    ParameterValue = id,
                    ErrorDescription = "The entity could not be founc"
                };

                return new NotFoundObjectResult(errorResponse);
            }

            JobEntityResponse jobEntityResponse = new JobEntityResponse()
            {
                JobId = jobEntity.RowKey,
                ImageConversionMode = jobEntity.ImageConversionMode,
                Status = jobEntity.Status,
                StatusDescription = jobEntity.StatusDescription,
                ImageSource = jobEntity.ImageSource,
                ImageResult = jobEntity.ImageResult,
            };

            return new OkObjectResult(jobEntityResponse);
        }

    }
}
