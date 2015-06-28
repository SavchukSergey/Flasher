using System.Threading;
using MicroFlasher.IO;

namespace MicroFlasher {
    public class StubChannel : IAvrChannel {

        public void Dispose() {
        }

        public void Open() {
        }

        public void Close() {
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
    }
}
