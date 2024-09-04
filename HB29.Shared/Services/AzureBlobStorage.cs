using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace hb29.Shared.Services
{
    public class AzureBlobStorage
    {
        private readonly ILogger<AzureBlobStorage> _logger;
        private readonly IConfiguration _configuration;

        public string ContainerName { get; set; }

        private BlobServiceClient _client = null; //do not use it directly

        private BlobServiceClient Client
        {
            get
            {
                if (_client == null)
                {
                    var blobClientOptions = new BlobClientOptions()
                    {
                        Retry = { MaxRetries = 2, NetworkTimeout = TimeSpan.FromSeconds(60) }
                    };

                    _client = new(this.ConnectionString, blobClientOptions);
                }

                return _client;
            }
        }
        private BlobContainerClient _container = null; //do not use it directly

        private BlobContainerClient Container
        {
            get
            {
                if ((_container == null) || (_container.Name != this.ContainerName))
                {
                    if (string.IsNullOrEmpty(this.ContainerName))
                    {
                        //throw new ArgumentException($"ContainerName is not set in Configuration.");
                        _logger.LogError("ContainerName is not set in Configuration.");                        
                    }

                    try
                    {
                        _container = this.Client.GetBlobContainerClient(this.ContainerName);
                        if(_container.Exists() == false)
                        {
                            _container.CreateIfNotExists();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                    //get client
                    _container = this.Client.GetBlobContainerClient(this.ContainerName);
                    
                }

                return _container;
            }
        }

        private string ConnectionString
        {
            get { return _configuration["ConnectionStrings-BlobStorageConnection"]; }
        }

        public AzureBlobStorage(IConfiguration configuration, ILogger<AzureBlobStorage> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public BlobContentInfo UploadSync(string filename, System.IO.Stream content)
        {
            // Get a reference to a blob
            BlobClient blob = this.Container.GetBlobClient(filename);

            // Upload data from the local file, overwriting by default
            return blob.Upload(content, true);
        }

        public async Task<BlobContentInfo> Upload(string filename, System.IO.Stream content)
        {
            // Get a reference to a blob
            BlobClient blob = this.Container.GetBlobClient(filename);

            // Upload data from the local file, overwriting by default
            return await blob.UploadAsync(content, true);
        }

        public async Task<BlobContentInfo> Upload(string filename, string content)
        {
            // Get a reference to a blob
            BlobClient blob = this.Container.GetBlobClient(filename);

            // Upload data from the local file, overwriting by default
            return await blob.UploadAsync(new BinaryData(content), true);
        }

        public async Task<bool> Exists(string filename)
        {
            // Get a reference to a blob
            BlobClient blob = this.Container.GetBlobClient(filename);

            //check if exists
            return await blob.ExistsAsync();
        }


        public async Task<BlobDownloadInfo> Download(string filename)
        {
            // Get a reference to a blob
            _logger.LogInformation($"Attempting to download {filename} from {this.ContainerName}.");
            try
            {
                BlobClient blob = this.Container.GetBlobClient(filename);
                var file = await blob.DownloadAsync();
                _logger.LogInformation($"Completed to download {filename} from {this.ContainerName}.");
                return file;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"File {filename} not found in Blog Storage Container {this.ContainerName}.\n{ex.Message}");

                throw new ArgumentException($"File {filename} not found in Blog Storage Container {this.ContainerName}.", filename);
            }
        }

        public async Task<Azure.Response> Delete(string filename)
        {
            // Get a reference to a blob
            _logger.LogInformation($"Attempting to delete {filename} from {this.ContainerName}.");

            BlobClient blob = this.Container.GetBlobClient(filename);

            //check if exists
            if (!(await blob.ExistsAsync()))
                throw new ArgumentException($"File {filename} not found in Blog Storage Container {this.ContainerName}.", filename);

            return await blob.DeleteAsync();
        }

        public async Task<BlobAppendInfo> AppendToFile(string nameLogFile, string textToLog)
        {
            var blob = Container.GetAppendBlobClient(nameLogFile);

            await blob.CreateIfNotExistsAsync();

            var bytes = Encoding.UTF8.GetBytes($"{textToLog}\n");
            var stream = new MemoryStream(bytes);

            return await blob.AppendBlockAsync(stream);
        }
    }
}