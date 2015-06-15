using System.Threading;
using System.Threading.Tasks;

namespace MicroFlasher.Views.Operations {
    /// <summary>
    /// Interaction logic for ResetDeviceWindow.xaml
    /// </summary>
    public partial class ResetDeviceWindow : BaseDeviceOperationWindow {

        public ResetDeviceWindow() {
            InitializeComponent();
        }

        protected override Task<bool> Execute(DeviceOperation op) {
            var settings = Model.Config;
            using (var channel = settings.GetProgrammerConfig().CreateChannel()) {
                channel.Open();
                channel.ToggleReset(true);
                Thread.Sleep(100);
                channel.ToggleReset(false);
                Thread.Sleep(100);
                channel.Close();
            }
            return Task.FromResult(true);
        }
    }
}
