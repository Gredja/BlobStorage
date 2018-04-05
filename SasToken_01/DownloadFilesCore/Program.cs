using System;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DownloadFilesCore
{
    class Program
    {
        static void Main(string[] args)
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                "DefaultEndpointsProtocol=https;AccountName=gredjastorage;AccountKey=pzA6quapsMnNfsM95k6wP64tS4O3nrXs8Il6CVs3fd" +
                "QQi9A/IgRwwD0f+QOAda/MXeli+mVt3zy8imNpbmtosA==;EndpointSuffix=core.windows.net");

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("gredjacontainer");

            MemoryStream memstream = new MemoryStream();
            CloudBlockBlob blockBlob =
                            container.GetBlockBlobReference("GredjaDevice/gredja_0ff3593d-cfd7-4310-9d53-6a64e708827a.txt");
            blockBlob.DownloadToStreamAsync(memstream).Wait();

            File.WriteAllBytes("e:/1.txt", memstream.ToArray());
        }

        private static string GetContainerSasUri(CloudBlobContainer container)
        {
            //Set the expiry time and permissions for the container.
            //In this case no start time is specified, so the shared access signature becomes valid immediately.
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(24);
            sasConstraints.Permissions = SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Read;

            //Generate the shared access signature on the container, setting the constraints directly on the signature.
            string sasContainerToken = container.GetSharedAccessSignature(sasConstraints);

            //Return the URI string for the container, including the SAS token.
            return container.Uri + sasContainerToken;
        }
    }
}
