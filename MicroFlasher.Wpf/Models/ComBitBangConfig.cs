using MicroFlasher.AvrSpi;
using MicroFlasher.Devices;
using MicroFlasher.IO;

namespace MicroFlasher.Models {
    public class ComBitBangConfig : BaseProgrammerConfig {

        private readonly ComPortSettings _comPortSettings;
        private readonly ComBitBangPinConfig _resetPin;
        private readonly ComBitBangPinConfig _clkPin;
        private readonly ComBitBangPinConfig _mosiPin;
        private readonly ComBitBangPinConfig _misoPin;

        public ComBitBangConfig(string keyPrefix)
            : base(keyPrefix) {
            _comPortSettings = new ComPortSettings(keyPrefix);
            _resetPin = new ComBitBangPinConfig(keyPrefix + "ResetPin.");
            _clkPin = new ComBitBangPinConfig(keyPrefix + "ClkPin.");
            _mosiPin = new ComBitBangPinConfig(keyPrefix + "MosiPin.");
            _misoPin = new ComBitBangPinConfig(keyPrefix + "MisoPin.");
        }

        public ComBitBangPinConfig ResetPin { get { return _resetPin; } }

        public ComBitBangPinConfig ClkPin { get { return _clkPin; } }

        public ComBitBangPinConfig MosiPin { get { return _mosiPin; } }

        public ComBitBangPinConfig MisoPin { get { return _misoPin; } }

        public ComPortSettings ComPortSettings {
            get { return _comPortSettings; }
        }

        public override void Save() {
            _comPortSettings.Save();
            _resetPin.Save();
            _clkPin.Save();
            _mosiPin.Save();
            _misoPin.Save();
        }

        public override void ReadFromConfig() {
            _comPortSettings.ReadFromConfig();
            _resetPin.ReadFromConfig();
            _clkPin.ReadFromConfig();
            _mosiPin.ReadFromConfig();
            _misoPin.ReadFromConfig();
        }

        public override IProgrammer CreateProgrammer(DeviceInfo device) {
            return new AvrSpiProgrammer(new AvrSpiClient(CreateChannel()));
        }

        public override IAvrChannel CreateChannel() {
            var port = ComPortSettings.CreateSerialPort();
            var spiMaster = new SpiMaster(port, ClkPin.CreatePin(port), MosiPin.CreatePin(port), MisoPin.CreatePin(port));
            return new SpiMasterChannel(spiMaster, ResetPin.CreatePin(port));
        }
    }
}
