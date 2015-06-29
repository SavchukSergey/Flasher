using System;
using System.IO.Ports;

namespace MicroFlasher.IO {
    public class SerialPortChannel : IAvrChannel {

        private readonly SerialPort _port;
        private readonly ComPin _resetPin;

        public SerialPortChannel(SerialPort port, ComPin resetPin) {
            _port = port;
            _resetPin = resetPin;
            port.ReadTimeout = 1000;
        }

        public void Dispose() {
            _port.Dispose();
        }

        public void Open() {
            _port.Open();
        }

        public void Close() {
            _port.Close();
        }

        public void ToggleReset(bool val) {
            _resetPin.Set(val);
        }

        public void SendByte(byte bt) {
            _port.Write(new[] { bt }, 0, 1);
            Console.Write("{1} [{0:x2}] ", bt, (bt >= 32 && bt <= 127) ? (char)bt : '.');
        }

        public byte ReceiveByte() {
            return (byte)_port.ReadByte();
        }

        public string Name { get { return _port.PortName; } }

        public bool IsOpen { get { return _port.IsOpen; } }

    }
}
