using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HW4AzureFunctions
{
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
        /// 
        /// </summary>
        /// <returns></returns>
        public List<JobEntityResponse> RetrieveAllJobEntities()
        {
            TableQuery<JobEntity> tableQuery = new TableQuery<JobEntity>();
            TableContinuationToken token = null;
            List<JobEntity> jobEntityList = new List<JobEntity>();
            List<JobEntityResponse> jobEntityResponseList = new List<JobEntityResponse>();

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
        /// 
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
        /// 
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
        /// 
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
        /// 
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
