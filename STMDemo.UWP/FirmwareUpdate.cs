using Microsoft.Extensions.Logging;
using SharpCubeProgrammer;
using SharpCubeProgrammer.Enum;
using SharpCubeProgrammer.Struct;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(STMDemo.UWP.FirmwareUpdateService))]
namespace STMDemo.UWP
{
    public class FirmwareUpdateService : IFirmwareService
    {
        ILogger<CubeProgrammerApi> _logger;

        public FirmwareUpdateService()
        {
            ILoggerFactory factory = LoggerFactory.Create((builder) => builder.AddDebug());
            _logger = factory.CreateLogger<CubeProgrammerApi>();
        }
        public Task<bool> UploadFirmware()
        {
            return Task.Run(() =>
            {
                var cubeProgrammerApi = new SharpCubeProgrammer.CubeProgrammerApi(_logger);
                System.Diagnostics.Debug.WriteLine("+++ TSV Flashing service [STM32MP15] +++");

                var connectionResult = cubeProgrammerApi.ConnectDfuBootloader("USB1");

                List<DfuDeviceInfo> dfuDevices = new List<DfuDeviceInfo>();
                int devicesFound = cubeProgrammerApi.GetDfuDeviceList(ref dfuDevices);
                List<UsartConnectParameters> usartDevices;
                if (GetEnumeratedDevices(cubeProgrammerApi, out usartDevices))
                {
                    System.Diagnostics.Debug.WriteLine($"unable to enumerate uart devices");
                    return false;
                }

                /* Target connect, choose the adequate USB port by indicating its index that is already mentioned in USB DFU List above */
                List<UsartConnectParameters> devices = usartDevices.FindAll((usartDevice) =>
                {
                    return usartDevice.portName == "usb1";
                });

                if (devices.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"no device matching connected device: {"usb1"}");
                    return false;
                }

                UsartConnectParameters device = (UsartConnectParameters)devices.First();
                CubeProgrammerError error = cubeProgrammerApi.ConnectUsartBootloader(device);
                if (error != CubeProgrammerError.CubeprogrammerNoError)
                {
                    System.Diagnostics.Debug.WriteLine($"Connection failed: {error}");
                    cubeProgrammerApi.Disconnect();
                    return false;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Connection succeed");
                }

                /* Display device information */
                GeneralInf? genInfo = cubeProgrammerApi.GetDeviceGeneralInf();
                if (genInfo is null)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to get device information");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"Device name: {genInfo?.Name}");
                System.Diagnostics.Debug.WriteLine($"Device type: {genInfo?.Type}");
                System.Diagnostics.Debug.WriteLine($"Device CPU: {genInfo?.Cpu}");


                /* Download binaries */
                //var binFilePath = "../test file/STM32MP/";

                //uint isVerify = 0;
                //uint isSkipErase = 1;


                //error = cubeProgrammerApi.DownloadFile(binFilePath, "0x08000000", isSkipErase, isVerify);
                //if (error != CubeProgrammerError.CubeprogrammerNoError)
                //{
                //    System.Diagnostics.Debug.WriteLine($"Device flash failed: {error}");
                //    cubeProgrammerApi.Disconnect();
                //    return false;
                //}

                /* Process successfully Done */
                cubeProgrammerApi.Disconnect();
                cubeProgrammerApi.DeleteInterfaceList();
                return true;
            });
        }

        private static bool GetEnumeratedDevices(CubeProgrammerApi cubeProgrammerApi, out List<UsartConnectParameters> usartDevices)
        {
            usartDevices = (List<UsartConnectParameters>)cubeProgrammerApi.GetUsartList();
            return (usartDevices?.Count ?? 0) == 0;
        }
    }
}
