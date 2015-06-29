using System.Threading;
using MicroFlasher.IO;

namespace MicroFlasher {
    public class StubChannel : IAvrChannel {

        private bool _isOpen;

        public void Dispose() {
        }

        public void Open() {
            _isOpen = true;
        }

        public void Close() {
            _isOpen = false;
        }

        public void ToggleReset(bool val) {
        }

        public void SendByte(byte bt) {
        }

        public byte ReceiveByte() {
            Thread.Sleep(50);
            return 0;
        }

        public string Name { get { return "Stub"; } }

        public bool IsOpen { get { return _isOpen; } }

    }
}
