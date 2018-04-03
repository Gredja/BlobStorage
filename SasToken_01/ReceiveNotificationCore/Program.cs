using System;
using Microsoft.Azure.Devices;

namespace ReceiveNotificationCore
{
    class Program
    {
        private static ServiceClient _serviceClient;

        static void Main(string[] args)
        {
            const string connectionString =
                            "HostName=GredjaIoT.azure-devices.net;SharedAccessKeyName=GredjaIoT.azure-devices.net;SharedAccessKey=UlU/IIEXHNyk0Spz9bCw2ESyss2CjfR+ZhjGHEv4+78=";

            Console.WriteLine("Receive file upload notifications\n");
            _serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            ReceiveFileUploadNotificationAsync();
            Console.ReadLine();
            Console.ReadKey();
        }

        private async static void ReceiveFileUploadNotificationAsync()
        {
            var notificationReceiver = _serviceClient.GetFileNotificationReceiver();

            Console.WriteLine("\nReceiving file upload notification from service");
            while (true)
            {
                var fileUploadNotification = await notificationReceiver.ReceiveAsync();
                if (fileUploadNotification == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received file upload noticiation: {0}, URL: {1}", string.Join(", ", fileUploadNotification.BlobName), fileUploadNotification.BlobUri);
                Console.ResetColor();

                await notificationReceiver.CompleteAsync(fileUploadNotification);
            }
        }
    }
}
