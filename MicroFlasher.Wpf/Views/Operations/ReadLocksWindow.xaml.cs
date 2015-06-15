using System.Threading.Tasks;

namespace MicroFlasher.Views.Operations {
    /// <summary>
    /// Interaction logic for ReadLocksWindow.xaml
    /// </summary>
    public partial class ReadLocksWindow : BaseDeviceOperationWindow {

        public ReadLocksWindow() {
            InitializeComponent();
        }

        protected override Task<bool> Execute(DeviceOperation op) {
            return Model.ReadLocksAsync(op);
        }
    }
}
