using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HW4AzureFunctions
{
    /// <summary>
    /// Contains all logic for managing
    /// job entities within the jobs table
    /// </summary>
    public class JobTable
    {
        private CloudTableClient _tableClient;
        private CloudTable _table;
        private string _partitionKey;
        private ILogger _log;

        public JobTable(ILogger log, string partitionKey)
        {
            string storageConnectionString = Environment.GetEnvironmentVariable(ConfigSettings.STORAGE_CONNECTIONSTRING_NAME);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            _tableClient = storageAccount.CreateCloudTableClient();

            _table = _tableClient.GetTableReference(ConfigSettings.JOBS_TABLENAME);

            _table.CreateIfNotExistsAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            _partitionKey = partitionKey;
        }

        /// <summary>
        /// Returns a JobEntityResponse DTO for
        /// every job entity in the jobs table.
        ///
        /// This is intended to be used by the
        /// job status API
        /// </summary>
        /// <returns></returns>
        public List<JobEntityResponse> RetrieveAllJobEntities()
        {
            TableQuery<JobEntity> tableQuery = new TableQuery<JobEntity>();
            TableContinuationToken token = null;
            List<JobEntity> jobEntityList = new List<JobEntity>();
            List<JobEntityResponse> jobEntityResponseList = new List<JobEntityResponse>();

            // Continue to make requests until no continuation token is found
            // The table API will respond with up to 1000 results
            do
            {
                var result = _table.ExecuteQuerySegmentedAsync(tableQuery, token).GetAwaiter().GetResult();
                jobEntityList.AddRange(result.Results);
                token = result.ContinuationToken;
            }
            while (token != null);

            foreach (JobEntity jobEntity in jobEntityList)
            {
                JobEntityResponse jobEntityResponse = new JobEntityResponse()
                {
                    JobId = jobEntity.RowKey,
                    ImageConversionMode = jobEntity.ImageConversionMode,
                    Status = jobEntity.Status,
                    StatusDescription = jobEntity.StatusDescription,
                    ImageSource = jobEntity.ImageSource,
                    ImageResult = jobEntity.ImageResult,
                };

                jobEntityResponseList.Add(jobEntityResponse);
            }

            return jobEntityResponseList;
        }

        /// <summary>
        /// Return all job entites that have a status of success
        /// </summary>
        /// <returns></returns>
        public List<JobEntity> RetrieveAllSuccessJobEntities()
        {
            string filter = TableQuery.GenerateFilterCondition("StatusDescription", QueryComparisons.Equal, "Image Converted with Success");
            TableQuery <JobEntity> tableQuery = new TableQuery<JobEntity>().Where(filter);
            TableContinuationToken token = null;
            List<JobEntity> jobEntityList = new List<JobEntity>();

            do
            {
                var result = _table.ExecuteQuerySegmentedAsync(tableQuery, token).GetAwaiter().GetResult();
                jobEntityList.AddRange(result.Results);
                token = result.ContinuationToken;
            }
            while (token != null);

            return jobEntityList;



        }

        /// <summary>
        /// Returns the job entity that has the given jobId
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<JobEntity> RetrieveJobEntity(string jobId)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<JobEntity>(_partitionKey, jobId);
            TableResult retrievedResult = await _table.ExecuteAsync(retrieveOperation);

            return retrievedResult.Result as JobEntity;
        }

        /// <summary>
        /// Replace the corresponding job entity with the
        /// given job entity
        /// </summary>
        /// <param name="jobEntity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateJobEntity(JobEntity jobEntity)
        {
            TableOperation replaceOperation = TableOperation.Replace(jobEntity);
            TableResult result = await _table.ExecuteAsync(replaceOperation);

            if (result.HttpStatusCode > 199 && result.HttpStatusCode < 300)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates the status of a job entity
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task UpdateJobEntityStatus(JobEntity jobEntity)
        {
            JobEntity jobEntityToReplace = await RetrieveJobEntity(jobEntity.JobId);
            if (jobEntityToReplace != null)
            {
                jobEntityToReplace.ImageConversionMode = jobEntity.ImageConversionMode;
                jobEntityToReplace.Status = jobEntity.Status;
                jobEntityToReplace.StatusDescription = jobEntity.StatusDescription;
                jobEntityToReplace.ImageSource = jobEntity.ImageSource;
                jobEntityToReplace.ImageResult = jobEntity.ImageResult;
                await UpdateJobEntity(jobEntityToReplace);
            }
        }

        /// <summary>
        /// Upserts the given job entity
        /// </summary>
        /// <param name="jobEntity"></param>
        /// <returns></returns>
        public async Task InsertOrReplaceJobEntity(JobEntity jobEntity)
        {
            JobEntity jobEntityToInsertOrReplace = new JobEntity()
            {
                RowKey = jobEntity.JobId,
                ImageConversionMode = jobEntity.ImageConversionMode,
                Status = jobEntity.Status,
                StatusDescription = jobEntity.StatusDescription,
                ImageSource = jobEntity.ImageSource,
                ImageResult = jobEntity.ImageResult,
                PartitionKey = _partitionKey,
            };

            TableOperation insertReplaceOperation = TableOperation.InsertOrReplace(jobEntityToInsertOrReplace);
            TableResult result = await _table.ExecuteAsync(insertReplaceOperation);
        }
    }
}
