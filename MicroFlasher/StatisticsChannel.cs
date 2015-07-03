using System.Threading;
using MicroFlasher.IO;

namespace MicroFlasher {
    public class StatisticsChannel : IAvrChannel {

        private readonly IAvrChannel _inner;
        private int _bytesReceived;
        private int _bytesSent;

        public StatisticsChannel(IAvrChannel inner) {
            _inner = inner;
        }

        public void Dispose() {
            _inner.Dispose();
        }

        public void Open() {
            _inner.Open();
        }

        public void Close() {
            _inner.Close();
        }

        public void ToggleReset(bool val) {
            _inner.ToggleReset(val);
        }

        public void SendByte(byte bt) {
            _inner.SendByte(bt);
            Interlocked.Increment(ref _bytesSent);
        }

        public byte ReceiveByte() {
            var res = _inner.ReceiveByte();
            Interlocked.Increment(ref _bytesReceived);
            return res;
        }

        public string Name { get { return _inner.Name; } }

        public bool IsOpen { get { return _inner.IsOpen; } }

        public int BytesReceived { get { return _bytesReceived; } }
        
        public int BytesSent { get { return _bytesSent; } }
    }

}
