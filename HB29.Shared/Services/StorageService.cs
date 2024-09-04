using Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace hb29.Shared.Services
{
    public class StorageService : IStorageService
    {
        private readonly AzureBlobStorage _blobStorage;
        private readonly ContainerNameSettings _containerSettings;
        private readonly ILogger<StorageService> _logger;

        public StorageService(AzureBlobStorage blobStorage, IOptions<ContainerNameSettings> settings, ILogger<StorageService> logger)
        {
            _blobStorage = blobStorage;
            _containerSettings = settings.Value;
            _logger = logger;
        }

        public async Task<bool> Delete(string blobName)
        {
            _blobStorage.ContainerName = _containerSettings.Container1;
            var response = await _blobStorage.Delete(blobName);

            return response.Status == 200;
        }

        public async Task<string> SaveUpload(Stream content, string blobName)
        {
            _blobStorage.ContainerName = _containerSettings.Container1;
            await _blobStorage.Upload(blobName, content);

            return blobName;
        }

        public async Task<Stream> GetStream(string blobName)
        {
            _blobStorage.ContainerName = _containerSettings.Container1;
            var downloadInfo = await _blobStorage.Download(blobName);

            return downloadInfo.Content;
        }


        public async Task<bool> Append(string message, string blobName)
        {
            _blobStorage.ContainerName = _containerSettings.Container1;
            
            _ = _blobStorage.AppendToFile(blobName, message);
            return await Task.FromResult<bool>(true);
        }
    }
}