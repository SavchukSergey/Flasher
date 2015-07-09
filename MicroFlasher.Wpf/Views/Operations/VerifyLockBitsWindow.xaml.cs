using System.Threading.Tasks;

namespace MicroFlasher.Views.Operations {
    /// <summary>
    /// Interaction logic for VerifyLockBitsWindow.xaml
    /// </summary>
    public partial class VerifyLockBitsWindow : BaseDeviceOperationWindow {

        public VerifyLockBitsWindow() {
            InitializeComponent();
        }

        protected override Task<bool> Execute(DeviceOperation op) {
            return Model.VerifyLockBitsAsync(op);
        }
    }
}
