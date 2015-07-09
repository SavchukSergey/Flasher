using System.Threading.Tasks;

namespace MicroFlasher.Views.Operations {
    /// <summary>
    /// Interaction logic for VerifyEepromWindow.xaml
    /// </summary>
    public partial class VerifyEepromWindow : BaseDeviceOperationWindow {

        public VerifyEepromWindow() {
            InitializeComponent();
        }

        protected override Task<bool> Execute(DeviceOperation op) {
            return Model.VerifyEepromAsync(op);
        }
    }
}
