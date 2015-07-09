using System.Threading.Tasks;

namespace MicroFlasher.Views.Operations {
    /// <summary>
    /// Interaction logic for VerifyFlashWindow.xaml
    /// </summary>
    public partial class VerifyFlashWindow : BaseDeviceOperationWindow {

        public VerifyFlashWindow() {
            InitializeComponent();
        }

        protected override Task<bool> Execute(DeviceOperation op) {
            return Model.VerifyFlashAsync(op);
        }
    }
}
