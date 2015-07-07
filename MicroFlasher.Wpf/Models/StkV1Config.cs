using MicroFlasher.Devices;
using MicroFlasher.IO;
using MicroFlasher.STKv1;

namespace MicroFlasher.Models {
    public class StkV1Config : BaseProgrammerConfig {

        private readonly ComPortSettings _comPortSettings;
        private readonly ComBitBangPinConfig _resetPin;

        public StkV1Config(string keyPrefix)
            : base(keyPrefix) {
            _comPortSettings = new ComPortSettings(keyPrefix);
            _resetPin = new ComBitBangPinConfig(keyPrefix + "ResetPin.");
        }

        public ComPortSettings ComPortSettings {
            get { return _comPortSettings; }
        }

        public ComBitBangPinConfig ResetPin { get { return _resetPin; } }

        public bool UseReset { get; set; }

        public override void Save() {
            _comPortSettings.Save();
            _resetPin.Save();
            UpdateConfigBool("UseReset", UseReset);
        }

        public override void ReadFromConfig() {
            _comPortSettings.ReadFromConfig();
            _resetPin.ReadFromConfig();
            UseReset = GetConfigBool(true, "UseReset");
        }

        public override string ConnectionName {
            get { return ComPortSettings.ComPort + (UseReset ? " (with reset)" : ""); }
        }

        public override IProgrammer CreateProgrammer(DeviceInfo device) {
            return new StkV1Programmer(new StkV1Client(CreateChannel()), device, UseReset);
        }

        public override IAvrChannel CreateChannel() {
            var port = ComPortSettings.CreateSerialPort();
            return new SerialPortChannel(port, ResetPin.CreatePin(port));
        }
    }
}
