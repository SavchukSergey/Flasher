using System.Threading.Tasks;

namespace MicroFlasher.Views.Operations {
    /// <summary>
    /// Interaction logic for VerifyFuseBitsWindow.xaml
    /// </summary>
    public partial class VerifyFuseBitsWindow : BaseDeviceOperationWindow {

        public VerifyFuseBitsWindow() {
            InitializeComponent();
        }

        protected override Task<bool> Execute(DeviceOperation op) {
            return Model.VerifyFuseBitsAsync(op);
        }
    }
}
