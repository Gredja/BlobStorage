
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SasToken_01
{
    class Program
    {
        static void Main(string[] args)
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("gredjacontainer");

            container.CreateIfNotExists();

            var containerSas = GetContainerSasUri(container);

            Console.WriteLine("Container SAS URI: " + GetContainerSasUri(container));

            var resourceFile = CreateLocalFile();
            Console.WriteLine($"local file name = {resourceFile.LocalFileName}");
            Console.WriteLine();
            Console.WriteLine($"local file path = {resourceFile.LocalFilePath}");

            UseContainerSAS(containerSas, resourceFile);

            Console.ReadLine();
        }

        static void UseContainerSAS(string sas, ResourceFile resourceFile)
        {
            CloudBlobContainer container = new CloudBlobContainer(new Uri(sas));

            try
            {
                CloudBlockBlob blob = container.GetBlockBlobReference(resourceFile.LocalFileName);


                //using (var sourceData = new FileStream(resourceFile.LocalFilePath, FileMode.Open))
                //{
                //    blob.UploadFromStreamAsync(sourceData);
                //}

                //  blob.UploadText(blobContent);

                //using (MemoryStream randomDataForPut = RandomData(33 * 1024 * 1024))
                //{
                //    blob.UploadFromStream(randomDataForPut);
                //}

                MemoryStream inMemoryCopy = new MemoryStream();
                using (FileStream fs = File.OpenRead(resourceFile.LocalFilePath))
                {
                    fs.CopyTo(inMemoryCopy);
                }

                Console.WriteLine("Write operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("Write operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
            }

            List<ICloudBlob> blobList = new List<ICloudBlob>();

            try
            {
                foreach (ICloudBlob blob in container.ListBlobs())
                {
                    blobList.Add(blob);
                }
                Console.WriteLine("List operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("List operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
            }

            Console.WriteLine();
        }


        private static string GetContainerSasUri(CloudBlobContainer container)
        {
            //Set the expiry time and permissions for the container.
            //In this case no start time is specified, so the shared access signature becomes valid immediately.
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(24);
            sasConstraints.Permissions = SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Read;

            //Generate the shared access signature on the container, setting the constraints directly on the signature.
            string sasContainerToken = container.GetSharedAccessSignature(sasConstraints);

            //Return the URI string for the container, including the SAS token.
            return container.Uri + sasContainerToken;
        }

        public static MemoryStream RandomData(long length)
        {

            var result = new MemoryStream();

            Random r = new Random();

            for (long i = 0; i < length; i++)

                result.WriteByte((byte)(r.Next(256)));

            result.Position = 0;

            return result;

        }


        private static ResourceFile CreateLocalFile()
        {
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string localFileName = "gredja_" + Guid.NewGuid() + ".txt";
            var sourceFile = Path.Combine(localPath, localFileName);
            // Write text to the file.
            File.WriteAllText(sourceFile, "Hello, gredja gredja v gredja gredja!");

            Console.WriteLine("Temp file = {0}", sourceFile);

            return new ResourceFile { LocalFileName = localFileName, LocalFilePath = sourceFile };
        }

    }



}
