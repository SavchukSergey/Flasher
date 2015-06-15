using System;

namespace MicroFlasher {
    public class ProgressTrackerProgrammer : IProgrammer {

        private readonly IProgrammer _inner;
        private readonly DeviceOperation _deviceOperation;
        private const int BLOCK_SIZE = 256;

        public ProgressTrackerProgrammer(IProgrammer inner, DeviceOperation deviceOperation) {
            _inner = inner;
            _deviceOperation = deviceOperation;
        }

        public DeviceOperation DeviceOperation {
            get { return _deviceOperation; }
        }

        public void Dispose() {
            _inner.Dispose();
        }

        public void Start() {
            _deviceOperation.CurrentState = "Entering programming mode";
            _inner.Start();
        }

        public void Stop() {
            _deviceOperation.CurrentState = "Leaving programming mode";
            _inner.Stop();
        }

        public void ReadPage(int address, AvrMemoryType memType, byte[] data, int dataStart, int dataLength) {
            var offset = address;
            var end = address + dataLength;
            while (offset < end) {
                var cnt = Math.Min(end - offset, BLOCK_SIZE);
                _deviceOperation.CurrentState = string.Format("Reading {0} memory {1}-{2}", memType, offset, offset + cnt - 1);
                _inner.ReadPage(offset, memType, data, offset - address + dataStart, cnt);
                offset += cnt;

                _deviceOperation.IncrementDone(cnt, memType);
                if (_deviceOperation.CancellationToken.IsCancellationRequested) {
                    _deviceOperation.CurrentState = "Operation is cancelled";
                }
                _deviceOperation.CancellationToken.ThrowIfCancellationRequested();
            }
        }

        public void WritePage(int address, AvrMemoryType memType, byte[] data, int dataStart, int dataLength) {
            var offset = address;
            var end = address + dataLength;
            while (offset < end) {
                var cnt = Math.Min(end - offset, BLOCK_SIZE);
                _deviceOperation.CurrentState = string.Format("Writing {0} memory {1}-{2}", memType, offset, offset + cnt - 1);
                _inner.WritePage(offset, memType, data, offset - address + dataStart, cnt);
                offset += cnt;

                _deviceOperation.IncrementDone(cnt, memType);
                if (_deviceOperation.CancellationToken.IsCancellationRequested) {
                    _deviceOperation.CurrentState = "Operation is cancelled";
                }
                _deviceOperation.CancellationToken.ThrowIfCancellationRequested();
            }
        }

        public void EraseDevice() {
            _deviceOperation.CurrentState = "Erasing the device";
            _inner.EraseDevice();
        }
    }
}
