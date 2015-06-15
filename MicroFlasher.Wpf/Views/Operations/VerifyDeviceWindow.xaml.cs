using System.Threading.Tasks;

namespace MicroFlasher.Views.Operations {
    /// <summary>
    /// Interaction logic for VerifyDeviceWindow.xaml
    /// </summary>
    public partial class VerifyDeviceWindow : BaseDeviceOperationWindow {

        public VerifyDeviceWindow() {
            InitializeComponent();
        }

        protected override Task<bool> Execute(DeviceOperation op) {
            return Model.VerifyDeviceAsync(op);
        }
    }
}
