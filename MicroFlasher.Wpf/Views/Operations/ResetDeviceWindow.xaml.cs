using System.Threading.Tasks;

namespace MicroFlasher.Views.Operations {
    /// <summary>
    /// Interaction logic for ResetDeviceWindow.xaml
    /// </summary>
    public partial class ResetDeviceWindow : BaseDeviceOperationWindow {

        public ResetDeviceWindow() {
            InitializeComponent();
        }

        protected override async Task<bool> Execute(DeviceOperation op) {
            await Model.ResetDeviceAsync();
            return true;
        }
    }
}
