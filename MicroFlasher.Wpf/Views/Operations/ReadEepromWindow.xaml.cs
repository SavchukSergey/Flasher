using System.Threading.Tasks;

namespace MicroFlasher.Views.Operations {
    /// <summary>
    /// Interaction logic for ReadEepromWindow.xaml
    /// </summary>
    public partial class ReadEepromWindow : BaseDeviceOperationWindow {

        public ReadEepromWindow() {
            InitializeComponent();
        }

        protected override Task<bool> Execute(DeviceOperation op) {
            return Model.ReadEepromAsync(op);
        }
    }
}
