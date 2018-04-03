using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Azure.Devices.Client;
using TransportType = Microsoft.Azure.Devices.Client.TransportType;

namespace SimulateDeviceCore
{
    class Program
    {
        private static DeviceClient _deviceClient;

        static void Main(string[] args)
        {
            // Look for the name in the connectionStrings section.
            const string connectionString =
                            "HostName=GredjaIoT.azure-devices.net;SharedAccessKeyName=GredjaIoT.azure-devices.net;SharedAccessKey=UlU/IIEXHNyk0Spz9bCw2ESyss2CjfR+ZhjGHEv4+78=";

            _deviceClient = DeviceClient.CreateFromConnectionString(connectionString, "GredjaDevice", TransportType.Mqtt);

            Timer timer = new Timer(4000);
            timer.Elapsed += (sender, e) =>  HandleTimer().Wait();
            timer.Start();
            Console.Write("Press any key to exit... ");
            Console.ReadKey();
        }

        private static async Task HandleTimer()
        {
            var resourceFile = CreateLocalFile();

            var watch = System.Diagnostics.Stopwatch.StartNew();

            using (var fs = new FileStream(Path.GetFullPath(resourceFile.LocalFilePath), FileMode.Open, FileAccess.Read))
            {
                _deviceClient.UploadToBlobAsync(resourceFile.LocalFileName, fs).Wait();
            }

            watch.Stop();

            Console.WriteLine("Simulated device\n");
        }

        private static ResourceFile CreateLocalFile()
        {
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string localFileName = "gredja_" + Guid.NewGuid() + ".txt";
            var sourceFile = Path.Combine(localPath, localFileName);
            // Write text to the file.
            File.WriteAllText(sourceFile, "Hello, gredja gredja v gredja gredja!");

            Console.WriteLine("Temp file = {0}", sourceFile);

            return new ResourceFile {LocalFileName = localFileName, LocalFilePath = sourceFile};
        }
    }
}
