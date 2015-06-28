using System;
using System.IO.Ports;
using MicroFlasher.IO;

namespace MicroFlasher {
    public class SpiMaster : IDisposable {
        private readonly SerialPort _port;
        private readonly ComPin _clkPin;
        private readonly ComPin _mosiPin;
        private readonly ComPin _misoPin;

        public SpiMaster(SerialPort port, ComPin clkPin, ComPin mosiPin, ComPin misoPin) {
            _port = port;
            _clkPin = clkPin;
            _mosiPin = mosiPin;
            _misoPin = misoPin;
        }

        public string Name { get { return _port.PortName; } }

        public void Open() {
            _port.Open();
        }

        public void Close() {
            _port.Close();
        }

        public byte SendByte(byte val) {
            var reply = 0;
            for (var i = 0; i < 8; i++) {
                var replyBit = SendBit((val & 0x80) > 0);
                reply <<= 1;
                reply += replyBit ? 1 : 0;
                val <<= 1;
            }
            return (byte)reply;
        }

        public bool SendBit(bool val) {
            SetMosi(val);
            SetClock();
            var res = GetMiso();
            ResetClock();
            return res;
        }

        public bool GetMiso() {
            return _misoPin.Get();
        }

        public void SetMosi(bool val) {
            _mosiPin.Set(val);
        }

        public void SetClock() {
            _clkPin.Set();
            DelayHigh();
        }

        public void ResetClock() {
            _clkPin.Reset();
            DelayLow();
        }


        private void DelayLow() {
            for (var i = 0; i < 100; i++) {
            }
        }

        private void DelayHigh() {
            DelayLow();
            DelayLow();
            DelayLow();
            DelayLow();
        }

        public void Dispose() {
            _port.Dispose();
        }
    }
}
