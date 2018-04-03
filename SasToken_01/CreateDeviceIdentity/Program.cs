using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;

namespace CreateDeviceIdentity
{
    internal class Program
    {
        private static RegistryManager _registryManager;
        private static string _connectionString = "HostName=GredjaIoT.azure-devices.net;SharedAccessKeyName=GredjaIoT.azure-devices.net;SharedAccessKey=UlU/IIEXHNyk0Spz9bCw2ESyss2CjfR+ZhjGHEv4+78=";

        private static void Main(string[] args)
        {
            _registryManager = RegistryManager.CreateFromConnectionString(_connectionString);
            AddDeviceAsync().Wait();
            Console.ReadLine();
        }

        private static async Task AddDeviceAsync()
        {
            const string deviceId = "GredjaDevice";
            Device device;

            try
            {
                device = await _registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await _registryManager.GetDeviceAsync(deviceId);
            }

            Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);
        }
    }
}
