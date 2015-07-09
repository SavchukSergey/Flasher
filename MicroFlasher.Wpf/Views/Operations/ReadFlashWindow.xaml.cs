using System.Threading.Tasks;

namespace MicroFlasher.Views.Operations {
    /// <summary>
    /// Interaction logic for ReadFlashWindow.xaml
    /// </summary>
    public partial class ReadFlashWindow : BaseDeviceOperationWindow {

        public ReadFlashWindow() {
            InitializeComponent();
        }

        protected override Task<bool> Execute(DeviceOperation op) {
            return Model.ReadFlashAsync(op);
        }
    }
}
