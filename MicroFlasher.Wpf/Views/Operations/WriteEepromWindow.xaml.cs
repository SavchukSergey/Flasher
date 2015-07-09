using System.Threading.Tasks;

namespace MicroFlasher.Views.Operations {
    /// <summary>
    /// Interaction logic for WriteEepromWindow.xaml
    /// </summary>
    public partial class WriteEepromWindow : BaseDeviceOperationWindow {

        public WriteEepromWindow() {
            InitializeComponent();
        }

        protected override Task<bool> Execute(DeviceOperation op) {
            return Model.WriteEepromAsync(op);
        }
    }
}
