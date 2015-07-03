using System;
using System.Text;
using System.Threading;
using MicroFlasher.IO;

namespace MicroFlasher.Monitor {


    public class SerialMonitor {

        private readonly Func<IAvrChannel> _channelSource;
        private readonly Action<SerialMonitorMessage> _callback;

        private readonly TimeSpan _threshold = TimeSpan.FromMilliseconds(500);
        private bool _flush;

        public SerialMonitor(Func<IAvrChannel> channelSource, Action<SerialMonitorMessage> callback) {
            _channelSource = channelSource;
            _callback = callback;
        }

        private IAvrChannel GetChannel() {
            return _channelSource();
        }

        private byte? ReadByte() {
            var channel = GetChannel();
            if (channel == null) {
                Thread.Sleep(50);
                return null;
            }
            byte? bt;
            try {
                bt = channel.ReceiveByte();
            } catch (Exception) {
                bt = null;
            }

            return bt;
        }

        private void Notify(SerialMonitorMessage msg) {
            _callback(msg);
        }

        public void ListenLoop(CancellationToken cancellationToken) {
            var msg = new SerialMonitorMessage();
            var receivedTime = DateTime.UtcNow;
            while (!cancellationToken.IsCancellationRequested) {
                var bt = ReadByte();

                var now = DateTime.UtcNow;
                var delta = now - receivedTime;
                if (delta > _threshold || _flush) {
                    _flush = false;
                    if (!msg.Empty) {
                        Notify(msg);
                        msg = new SerialMonitorMessage();
                    }
                }

                if (bt.HasValue) {
                    receivedTime = DateTime.UtcNow;
                    msg.Content.Append((char)bt);
                    Notify(msg);
                }
            }
        }

        public bool SendMessage(string content) {
            _flush = true;

            var channel = GetChannel();
            if (channel == null || !channel.IsOpen) return false;
            try {
                foreach (var ch in content) {
                    channel.SendByte((byte)ch);
                }
                return true;
            } catch {
                return false;
            }
        }
    }
}
