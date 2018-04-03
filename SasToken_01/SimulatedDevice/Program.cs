using System;
using System.IO;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using SasToken_01;

namespace SimulatedDevice
{

    public class Program
    {
        private static DeviceClient _deviceClient;
        private const string IotHubUri = "GredjaIoT.azure-devices.net";
        private const string DeviceKey = "y6xYf+VoH2RAqceofwuDJU5NMDmSIx26hTCQBcrBUC8=";

        private  static void Main(string[] args)
        {
            var connectionString = Helpers.GetConnectionStringByName("IoTConnectionString");

            Console.WriteLine("Simulated device\n");
            // _deviceClient = DeviceClient.Create(IotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("GredjaDevice", DeviceKey));


             var resourceFile = CreateLocalFile();
            if (resourceFile != null)
            {
                SendToBlobAsync(resourceFile).Wait();
            }

            //_deviceClient.ProductInfo = "HappyPath_Simulated-CSharp";
            //SendDeviceToCloudMessagesAsync();

            Console.ReadLine();
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

      private static async Task SendToBlobAsync(ResourceFile resourceFile)
        {
            Console.WriteLine("Uploading file: {0}", resourceFile.LocalFileName);
            var dd  = DeviceClient.CreateFromConnectionString("HostName=GredjaIoT.azure-devices.net;DeviceId=GredjaDevice;SharedAccessKey=y6xYf+VoH2RAqceofwuDJU5NMDmSIx26hTCQBcrBUC8=", TransportType.Mqtt);
            var watch = System.Diagnostics.Stopwatch.StartNew();

            using (var fs = new FileStream(@"e:\Work\BlobStorage\SasToken_01\SimulatedDevice\gredjaimg.jpg", FileMode.Open, FileAccess.Read))
            {
                dd.UploadToBlobAsync("dfgdfs", fs).Wait();
            }

            watch.Stop();
            Console.WriteLine("Time to upload file: {0}ms\n", watch.ElapsedMilliseconds);
        }

        private static async void SendDeviceToCloudMessagesAsync()
        {
            const double minTemperature = 20;
            const double minHumidity = 60;
            int messageId = 1;

            Random rand = new Random();

            while (true)
            {
                double currentTemperature = minTemperature + rand.NextDouble() * 15;
                double currentHumidity = minHumidity + rand.NextDouble() * 20;

                var telemetryDataPoint = new
                {
                    messageId = messageId++,
                    deviceId = "myFirstDevice",
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };

                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                message.Properties.Add("temperatureAlert", currentTemperature > 30 ? "true" : "false");

                await _deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(1000);
            }
        }
    }
}
