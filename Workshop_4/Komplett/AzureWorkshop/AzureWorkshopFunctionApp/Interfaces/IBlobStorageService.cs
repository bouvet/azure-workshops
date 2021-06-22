using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzureWorkshopFunctionApp.Interfaces
{
    public interface IBlobStorageService
    {
        Task UploadStreamToBlob(string container, string blobName, Stream stream);

        Task<Stream> GetBlobAsStream(string container, string blobName);

        Task<IEnumerable<string>> GetFileNamesInContainer(string container);
    }
}
