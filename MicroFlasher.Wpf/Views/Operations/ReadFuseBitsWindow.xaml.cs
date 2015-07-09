using System.Threading.Tasks;

namespace MicroFlasher.Views.Operations {
    /// <summary>
    /// Interaction logic for ReadFuseBitsWindow.xaml
    /// </summary>
    public partial class ReadFuseBitsWindow : BaseDeviceOperationWindow {

        public ReadFuseBitsWindow() {
            InitializeComponent();
        }

        protected override Task<bool> Execute(DeviceOperation op) {
            return Model.ReadFusesAsync(op);
        }
    }
}
