using System.Threading.Tasks;

namespace MicroFlasher.Views.Operations {
    /// <summary>
    /// Interaction logic for WriteLocksWindow.xaml
    /// </summary>
    public partial class WriteLocksWindow : BaseDeviceOperationWindow {

        public WriteLocksWindow() {
            InitializeComponent();
        }

        protected override Task<bool> Execute(DeviceOperation op) {
            return Model.WriteLocksAsync(op);
        }
    }
}
