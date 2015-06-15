using System.IO.Ports;
using MicroFlasher.IO;

namespace MicroFlasher.Models {
    public class ComBitBangPinConfig : BaseConfig {
        private ComPinType _pin;
        private bool _invert;

        public ComBitBangPinConfig(string keyPrefix)
            : base(keyPrefix) {
        }

        public ComPinType Pin {
            get { return _pin; }
            set {
                if (value != _pin) {
                    _pin = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Invert {
            get { return _invert; }
            set {
                if (value != _invert) {
                    _invert = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void Save() {
            UpdateConfigEnum("Pin", Pin);
            UpdateConfigBool("Invert", Invert);
        }

        public override void ReadFromConfig() {
            Pin = GetConfigEnum(ComPinType.None, "Pin");
            Invert = GetConfigBool(false, "Invert");
        }

        public ComPin CreatePin(SerialPort port) {
            return new ComPin(port, GetPinType(), Invert);
        }

        private ComPinType GetPinType() {
            return GetConfigEnum(ComPinType.None, "Pin");
        }

    }
}
