using System.Threading.Tasks;

namespace MicroFlasher.Views.Operations {
    /// <summary>
    /// Interaction logic for WriteFusesWindow.xaml
    /// </summary>
    public partial class WriteFusesWindow : BaseDeviceOperationWindow {

        public WriteFusesWindow() {
            InitializeComponent();
        }

        protected override Task<bool> Execute(DeviceOperation op) {
            return Model.WriteFusesAsync(op);
        }
    }
}
