using Google.Cloud.BigQuery.V2;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Test_Walmart_Buybox_notification.Services
{
    public interface IBigQueryRepo
    {
        Task<BigQueryPage> GetQueryResultsByString(string qry = null, string nextPageToken = null, string jobId = null);
        Task<BigQueryResults> GetQueryResultsByString(string qry);
        Task<BigQueryResults> GetQueryResultsByString(string qry, BigQueryParameter[] bigQueryParameters);

        Task CreateTempTable(string datasetName, string tempTableName, string refTable);

        Task<BigQueryInsertResults> InsertRows(string datasetName, string tableName,
            ConcurrentQueue<BigQueryInsertRow> bqRows);

        Task UpsertQueryResults(string qry);

        Task DeleteTempTable(string dataset, string tempTable);
        Task ExecuteQuery(string sql, BigQueryParameter[] parameters);

    }
}
