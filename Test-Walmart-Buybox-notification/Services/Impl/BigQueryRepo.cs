using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Test_Walmart_Buybox_notification.Services;

namespace Test_Walmart_Buybox_notificationg.Services.Impl
{
    public class BigQueryRepo : IBigQueryRepo
    {
        private readonly string _projectId;
        private readonly string _pageSzie;
        public BigQueryRepo()
        {
            _projectId = ConfigurationManager.AppSettings["ProjectId"];
            _pageSzie = ConfigurationManager.AppSettings["PageSize"];

        }
        public async Task<BigQueryPage> GetQueryResultsByString(string qry = null, string nextPageToken = null, string jobId = null)
        {
            BigQueryPage results = null;
            BigQueryClient client = await BigQueryClient.CreateAsync(_projectId);

            if (string.IsNullOrEmpty(nextPageToken))
            {
                BigQueryParameter[] parameters = null;
                results = await (await client.ExecuteQueryAsync(qry, parameters)).ReadPageAsync(int.Parse(_pageSzie));
            }
            else
            {
                results = await (await client.GetQueryResultsAsync(jobId,
                       new GetQueryResultsOptions() { PageToken = nextPageToken })).ReadPageAsync(int.Parse(_pageSzie));
            }

            return results;
        }


        public async Task<BigQueryResults> GetQueryResultsByString(string qry = null)
        {
            try
            {
                BigQueryResults results = null;
                BigQueryClient client = await BigQueryClient.CreateAsync(_projectId);

                BigQueryParameter[] parameters = null;
                results = await client.ExecuteQueryAsync(qry, parameters, null,
                   new GetQueryResultsOptions() { Timeout = TimeSpan.FromMilliseconds(1000 * 300) });
                return results;
            }
            catch (Exception) { throw; }
        }
        public async Task<BigQueryResults> GetQueryResultsByString(string qry, BigQueryParameter[] parameters)
        {
            try
            {
                BigQueryResults results = null;
                BigQueryClient client = await BigQueryClient.CreateAsync(_projectId);
                results = await client.ExecuteQueryAsync(qry, parameters, null,
                   new GetQueryResultsOptions() { Timeout = TimeSpan.FromMilliseconds(1000 * 300) });
                return results;
            }
            catch (Exception) { throw; }
        }

        public async Task CreateTempTable(string datasetName, string tempTableName, string refTable)
        {
            BigQueryClient client = await BigQueryClient.CreateAsync(_projectId);
            var dataset = await client.GetDatasetAsync(datasetName);
            TableSchema refTableSchema = (await client.GetTableAsync(_projectId, datasetName, refTable)).Schema;
            await dataset.CreateTableAsync(tableId: tempTableName, schema: refTableSchema);
        }

        public async Task<BigQueryInsertResults> InsertRows(string datasetName, string tableName,
            ConcurrentQueue<BigQueryInsertRow> bqRows)
        {
            try
            {
                BigQueryInsertResults bigQueryInsertResults = null;
                BigQueryClient client = await BigQueryClient.CreateAsync(_projectId);

                if (bqRows.Count > 1)
                {
                    bigQueryInsertResults = await client.InsertRowsAsync(_projectId, datasetName, tableName, bqRows);
                }
                else if (bqRows.Count == 1)
                {
                    bigQueryInsertResults = await client.InsertRowAsync(_projectId, datasetName, tableName, bqRows.First());
                }

                return bigQueryInsertResults;
            }
            catch (Exception) { throw; }
        }

        public async Task UpsertQueryResults(string qry)
        {
            BigQueryClient client = await BigQueryClient.CreateAsync(_projectId);
            BigQueryParameter[] parameters = null;
            await client.ExecuteQueryAsync(qry, parameters);
        }

        public async Task DeleteTempTable(string dataset, string tempTable)
        {
            BigQueryClient client = await BigQueryClient.CreateAsync(_projectId);
            await client.DeleteTableAsync(dataset, tempTable);
        }
        public async Task ExecuteQuery(string sql, BigQueryParameter[] parameters)
        {
            try
            {
                using BigQueryClient client = await BigQueryClient.CreateAsync(_projectId);
                await client.ExecuteQueryAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

    }
}
