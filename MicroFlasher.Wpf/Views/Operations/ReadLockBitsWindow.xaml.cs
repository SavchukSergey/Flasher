using System.Threading.Tasks;

namespace MicroFlasher.Views.Operations {
    /// <summary>
    /// Interaction logic for ReadLockBitsWindow.xaml
    /// </summary>
    public partial class ReadLockBitsWindow : BaseDeviceOperationWindow {

        public ReadLockBitsWindow() {
            InitializeComponent();
        }

        protected override Task<bool> Execute(DeviceOperation op) {
            return Model.ReadLocksAsync(op);
        }
    }
}
