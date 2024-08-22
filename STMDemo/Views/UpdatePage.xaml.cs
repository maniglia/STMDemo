using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace STMDemo.Views
{
    public partial class UpdatePage : ContentPage
    {
        private static readonly Lazy<IFirmwareService> _lazyFirmwareService = new Lazy<IFirmwareService>(() => {
            return DependencyService.Get<IFirmwareService>();
        });
        static readonly IFirmwareService FirmwareService = _lazyFirmwareService.Value;
        static bool FirmwareUpdateSupported => FirmwareService != null;
        public UpdatePage()
        {
            InitializeComponent();
        }

        public void PerformUpdate(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Clicked");
            PerformUpdate();
        }
        public void PerformUpdate()
        {
            if (FirmwareUpdateSupported)
            {
                System.Diagnostics.Debug.WriteLine($"Update Started");

                FirmwareService.UploadFirmware().ContinueWith((task) =>
                {
                    System.Diagnostics.Debug.WriteLine($"Update Operation Terminated. Successful: {task.Result}");
                });
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Platform Unsupported");
            }
        }
    }
}