using System.Threading;

namespace MicroFlasher.Models {
    public class FlasherOperationModel {

        private readonly FlasherModel _flasher;
        private readonly ObservableDeviceOperation _deviceOperation;

        public FlasherOperationModel(FlasherModel flasher) {
            _flasher = flasher;
            _deviceOperation = new ObservableDeviceOperation(new CancellationTokenSource());
        }

        public FlasherModel Flasher {
            get { return _flasher; }
        }

        public DeviceOperation DeviceOperation {
            get { return _deviceOperation; }
        }
    }
}
