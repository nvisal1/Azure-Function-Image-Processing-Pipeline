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
        /// <summary>
        /// This function is triggered when the /jobs/{id} endpoint
        /// is invoked. The specified job entity is retrieved and returned
        /// to the client. A 404 status is returned to the client if the
        /// specified job entity is not found.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("ConversionJobStatusById")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "jobs/{id}")] HttpRequest req, string id, ILogger log)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult("The query string id is required");
            }

            if (id.Length > 36)
            {
                return new BadRequestObjectResult("An id cannot be greater than 36 characters");
            }

            log.LogInformation("[PENDING] Connecting to jobs table...");
            JobTable jobTable = new JobTable(log, ConfigSettings.IMAGEJOBS_PARTITIONKEY);
            log.LogInformation("[SUCCESS] Connected to Jobs Table");

            log.LogInformation("[PENDING] Searching for Job Entity {id}...");
            JobEntity jobEntity = await jobTable.RetrieveJobEntity(id);

            if (jobEntity == null)
            {
                log.LogError("Job Entity {id} was not found");

                ErrorResponse errorResponse = ErrorResponse.New(ErrorResponseCodes.NOT_FOUND, "JobId", id, ErrorResponseMessages.NOT_FOUND);

                return new NotFoundObjectResult(errorResponse);
            }

            log.LogInformation("[SUCCESS] Job Entity {id} was found");

            JobEntityResponse jobEntityResponse = JobEntityResponse.New(
                jobEntity.RowKey, jobEntity.ImageConversionMode, jobEntity.Status, jobEntity.StatusDescription, jobEntity.ImageSource, jobEntity.ImageResult);

            return new OkObjectResult(jobEntityResponse);
        }

    }
}
