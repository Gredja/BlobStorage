using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;

namespace UploadFIles
{
   internal class Program
    {
        const string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=gredjastorage;AccountKey=rlRaoD086zalQ8cCpcggO97NOOSO4HUSz/" +
                                               "1BRew3sddE1LOxHIQKE3QdBjGzcEJSL/R4wOV/5kDCuxBWFXQxdw==;EndpointSuffix=core.windows.net";

        static void Main(string[] args)
        {
            ProcessAsync().GetAwaiter().GetResult();
        }

        static string GetAccountSASToken()
        {
            // To create the account SAS, you need to use your shared key credentials. Modify for your account.

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Create a new access policy for the account.
            SharedAccessAccountPolicy policy = new SharedAccessAccountPolicy()
            {
                Permissions = SharedAccessAccountPermissions.Read | SharedAccessAccountPermissions.Write | SharedAccessAccountPermissions.List,
                Services = SharedAccessAccountServices.Blob | SharedAccessAccountServices.File,
                ResourceTypes = SharedAccessAccountResourceTypes.Service,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                Protocols = SharedAccessProtocol.HttpsOnly
            };

            // Return the SAS token.
            return storageAccount.GetSharedAccessSignature(policy);
        }

        private static async Task ProcessAsync()
        {
            var sasToken = GetAccountSASToken();

            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;
            string sourceFile = null;
            string destinationFile = null;

            StorageCredentials accountSAS = new StorageCredentials(sasToken);

            CloudStorageAccount accountWithSAS = new CloudStorageAccount(accountSAS, "account-name", endpointSuffix: null, useHttps: true);
            CloudBlobClient blobClientWithSAS = accountWithSAS.CreateCloudBlobClient();

            cloudBlobContainer = blobClientWithSAS.GetContainerReference("gredjacontainer");

            //await blobClientWithSAS.SetServicePropertiesAsync(new ServiceProperties()
            //{
            //    HourMetrics = new MetricsProperties()
            //    {
            //        MetricsLevel = MetricsLevel.ServiceAndApi,
            //        RetentionDays = 7,
            //        Version = "1.0"
            //    },
            //    MinuteMetrics = new MetricsProperties()
            //    {
            //        MetricsLevel = MetricsLevel.ServiceAndApi,
            //        RetentionDays = 7,
            //        Version = "1.0"
            //    },
            //    Logging = new LoggingProperties()
            //    {
            //        LoggingOperations = LoggingOperations.All,
            //        RetentionDays = 14,
            //        Version = "1.0"
            //    }
            //});

            // Create a file in your local MyDocuments folder to upload to a blob.
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string localFileName = "gredja_" + Guid.NewGuid().ToString() + ".txt";
            sourceFile = Path.Combine(localPath, localFileName);
            // Write text to the file.
            File.WriteAllText(sourceFile, "Hello, All!");

            Console.WriteLine("Temp file = {0}", sourceFile);
            Console.WriteLine("Uploading to Blob storage as blob '{0}'", localFileName);
            Console.WriteLine();

            // Get a reference to the blob address, then upload the file to the blob.
            // Use the value of localFileName for the blob name.
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(localFileName);
            await cloudBlockBlob.UploadFromFileAsync(sourceFile);
        }
    }
}
