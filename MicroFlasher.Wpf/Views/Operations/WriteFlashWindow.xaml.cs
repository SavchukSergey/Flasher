using System.Threading.Tasks;

namespace MicroFlasher.Views.Operations {
    /// <summary>
    /// Interaction logic for WriteFlashWindow.xaml
    /// </summary>
    public partial class WriteFlashWindow : BaseDeviceOperationWindow {

        public WriteFlashWindow() {
            InitializeComponent();
        }

        protected override Task<bool> Execute(DeviceOperation op) {
            return Model.WriteFlashAsync(op);
        }
    }
}
