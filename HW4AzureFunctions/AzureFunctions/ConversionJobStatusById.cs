using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using HW4AzureFunctions.Constants;

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
                ErrorResponse errorResponse = ErrorResponse.New(ErrorResponseCodes.NOT_FOUND, "JobId", id, ErrorResponseMessages.NOT_FOUND);

                return new NotFoundObjectResult(errorResponse);
            }

            JobEntityResponse jobEntityResponse = JobEntityResponse.New(
                jobEntity.RowKey, jobEntity.ImageConversionMode, jobEntity.Status, jobEntity.StatusDescription, jobEntity.ImageSource, jobEntity.ImageResult);

            return new OkObjectResult(jobEntityResponse);
        }

    }
}
