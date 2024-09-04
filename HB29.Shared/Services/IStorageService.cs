using System.IO;
using System.Threading.Tasks;

namespace hb29.Shared.Services
{
    public interface IStorageService
    {
        /// <summary>
        /// Updates Node Template uploaded file into storage for later processing.
        /// </summary>
        public Task<string> SaveUpload(Stream content, string blobName);

        public Task<Stream> GetStream(string blobName);

        public Task<bool> Delete(string blobName);

        public Task<bool> Append(string message, string blobName);
    }
}