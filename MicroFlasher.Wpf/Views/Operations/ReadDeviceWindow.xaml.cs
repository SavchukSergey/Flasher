using System.Threading.Tasks;

namespace MicroFlasher.Views.Operations {
    /// <summary>
    /// Interaction logic for ReadDeviceWindow.xaml
    /// </summary>
    public partial class ReadDeviceWindow : BaseDeviceOperationWindow {

        public ReadDeviceWindow() {
            InitializeComponent();
        }

        protected override Task<bool> Execute(DeviceOperation op) {
            return Model.ReadDeviceAsync(op);
        }
    }
}
