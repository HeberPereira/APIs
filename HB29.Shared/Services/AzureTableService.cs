using Azure.Data.Tables;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure.Data.Tables.Models;
using System.Collections;
using Azure;
using System.ComponentModel.DataAnnotations.Schema;

namespace hb29.Shared.Services
{
    public class AzureTableService
    {
        private readonly ILogger<AzureQueueStorage> _logger;
        private readonly IConfiguration _configuration;
        
        //private string _partitionKey = "ActivityStatus";
        //private string _tableName = "TunnelingConcurrencyState";

        private string ConnectionString
        {
            get { return _configuration["ConnectionStrings-BlobStorageConnection"]; }
        }

        private TableServiceClient tableServiceClient;
        
        public AzureTableService(IConfiguration configuration, ILogger<AzureQueueStorage> logger)
        {
            _logger = logger;
            _configuration = configuration;
            tableServiceClient = new TableServiceClient(this.ConnectionString);
        }

        //public AzureTableService(string tableName, string partitionKey)
        //{
        //_partitionKey = partitionKey;
        //_tableName = tableName;

        //// New instance of TableClient class referencing the server-side table
        //tableClient = tableServiceClient.GetTableClient(
        //    tableName: _tableName
        //);

        //var table = tableClient.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        //}

        public TableClient GetTable(string tableName)
        {
            //New instance of TableClient class referencing the server-side table
            TableClient tableClient = tableServiceClient.GetTableClient(tableName: tableName);

            var table = tableClient.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            return tableClient;
        }

        public async Task<string> InsertRow(string tableName, ITableEntity entity)
        {
            try
            {
                TableClient tableClient = this.GetTable(tableName);

                if (string.IsNullOrEmpty(entity.RowKey))
                {
                    string id = Guid.NewGuid().ToString();
                    entity.RowKey = id;
                }
                entity.PartitionKey = entity.PartitionKey;
                // Add new item to server-side table
                var row = await tableClient.AddEntityAsync<ITableEntity>(entity);

                if (row.Status != 204)
                {
                    _logger.LogInformation("Error occurred while saving record");
                    throw new Exception("Error occurred while saving record");
                }
                else
                    return entity.RowKey;
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"Error occurred while saving record, Message: {ex.Message} ");
                throw;
            }
        }

        public async Task<T> GetById<T>(string tableName, string rowKey, string partitionKey) where T : class, Azure.Data.Tables.ITableEntity, new()
        {
            try
            {
                TableClient tableClient = this.GetTable(tableName);
                // Read a single item from container
                var product = await tableClient.GetEntityAsync<T>(rowKey: rowKey, partitionKey: partitionKey);
                
                return product;
            }
            catch (Azure.RequestFailedException ex)
            {
                if (ex.Status == 404)
                {
                    //_logger.LogInformation($"Row not found key:{rowKey} ");
                }
                else
                {
                    _logger.LogInformation(ex.Message);
                }
                return null;
            }
        }

        public  List<T> GetAll<T>(string tableName, string partitionKey) where T : class, Azure.Data.Tables.ITableEntity, new()
        {
            try
            {
                TableClient tableClient = this.GetTable(tableName);

                var rows = tableClient.Query<T>(x => ((ITableEntity)x).PartitionKey == partitionKey);
               
                return rows.ToList();
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return null;
            }
        }

        public List<T> GetAll<T>(string tableName) where T : class, Azure.Data.Tables.ITableEntity, new()
        {
            try
            {
                TableClient tableClient = this.GetTable(tableName);

                var rows =  tableClient.Query<T>();

                return rows.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Query by string
        /// </summary>
        /// <typeparam name="T">Entity implemented interface ITableEntity</typeparam>
        /// <param name="partitionKey">Name Partition Key</param>
        /// <param name="query">ex.: $"PartitionKey eq '{partitionKey}'" </param>
        /// <returns></returns>
        public List<T> GetAllByQuery<T>(string tableName, string partitionKey, string query) where T : class, Azure.Data.Tables.ITableEntity, new()
        {
            try
            {
                TableClient tableClient = this.GetTable(tableName);

                var products = tableClient.Query<T>(filter: query);

                return products.ToList();
            }
            catch (Azure.RequestFailedException ex)
            {
                if (ex.Status == 404)
                {
                    _logger.LogInformation($"Row not found to query:{query} ");
                }
                else
                {
                    _logger.LogInformation(ex.Message);
                }
                return null;
            }
        }

        public async Task<Azure.Response> DeleteRow(string tableName, string rowKey, string partitionKey)
        {
            try
            {
                TableClient tableClient = this.GetTable(tableName);

                // Delete the entity given the partition and row key.
                var response = await tableClient.DeleteEntityAsync(partitionKey, rowKey);
                return response;
            }
            catch (Azure.RequestFailedException ex)
            {
                if (ex.Status == 404)
                {
                    _logger.LogInformation($"Row not found to key:{rowKey} ");
                }
                else
                {
                    _logger.LogInformation(ex.Message);
                }
                return null;
            }
        }
    }

    [NotMapped]
    public class EntityGenericAzureTable : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Entity { get; set; }
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
}