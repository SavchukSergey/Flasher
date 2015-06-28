using MicroFlasher.IO;

namespace MicroFlasher {
    public class SpiMasterChannel : IAvrChannel {
        private readonly SpiMaster _master;
        private readonly ComPin _resetPin;
        private byte _received;

        public SpiMasterChannel(SpiMaster master, ComPin resetPin) {
            _master = master;
            _resetPin = resetPin;
        }

        public void Dispose() {
            _master.Dispose();
        }

        public void Open() {
            _master.Open();
            _master.ResetClock();
        }

        public void Close() {
            _master.Close();
        }

        public void ToggleReset(bool val) {
            _resetPin.Set(val);
        }

        public void SendByte(byte bt) {
            _received = _master.SendByte(bt);
        }

        public byte ReceiveByte() {
            return _received;
        }

        public string Name { get { return string.Format("Spi at {0}", _master.Name); } }
    }
}
