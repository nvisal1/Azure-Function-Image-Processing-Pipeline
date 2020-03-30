using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HW4AzureFunctions.AzureFunctions.ImagesConverters
{
    public class Shared
    {
        /// <summary>
        /// Creates a connection to the jobs table and
        /// upserts the given job entity.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="jobEntity"></param>
        /// <returns></returns>
        public static async Task UpdateJobTableWithStatus(ILogger log, JobEntity jobEntity)
        {
            log.LogInformation("[PENDING] Connecting to jobs table...");
            JobTable jobTable = new JobTable(log, ConfigSettings.IMAGEJOBS_PARTITIONKEY);
            log.LogInformation("[SUCCESS] Connected to Jobs Table");

            await jobTable.InsertOrReplaceJobEntity(jobEntity);
        }
    }
}
