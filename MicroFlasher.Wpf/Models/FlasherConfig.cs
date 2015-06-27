using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MicroFlasher.Devices;

namespace MicroFlasher.Models {
    public class FlasherConfig : BaseConfig {

        private DeviceInfo _device;
        private ProgrammerType _programmerType;
        private readonly ObservableCollection<KeyValuePair<ProgrammerType, string>> _programmerTypes = new ObservableCollection<KeyValuePair<ProgrammerType, string>>();
        private readonly ObservableCollection<DeviceInfo> _devices = new ObservableCollection<DeviceInfo>();
        private readonly StkV1Config _stkv1;
        private readonly ComBitBangConfig _comBitBang;
        private readonly StubConfig _stub;

        public StkV1Config Stkv1 {
            get { return _stkv1; }
        }

        public ComBitBangConfig ComBitBang {
            get { return _comBitBang; }
        }

        public bool AutoVerify { get; set; }

        public FlasherConfig()
            : base(string.Empty) {
            _stkv1 = new StkV1Config("StkV1.");
            _comBitBang = new ComBitBangConfig("ComBitBang.");
            _stub = new StubConfig("Stub.");
        }

        public ProgrammerType ProgrammerType {
            get { return _programmerType; }
            set {
                if (_programmerType != value) {
                    _programmerType = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<KeyValuePair<ProgrammerType, string>> ProgrammerTypes {
            get { return _programmerTypes; }
        }

        public ObservableCollection<DeviceInfo> Devices {
            get {
                return _devices;
            }
        }

        public DeviceInfo Device {
            get { return _device; }
            set {
                if (_device != value) {
                    _device = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void ReadFromConfig() {
            _stkv1.ReadFromConfig();
            _comBitBang.ReadFromConfig();
            _stub.ReadFromConfig();
            AutoVerify = GetConfigBool(false, "Verify");

            ProgrammerTypes.Clear();
            ProgrammerTypes.Add(new KeyValuePair<ProgrammerType, string>(ProgrammerType.StkV1, "STK v1"));
            ProgrammerTypes.Add(new KeyValuePair<ProgrammerType, string>(ProgrammerType.ComBitBang, "Com Bit Bang"));
            ProgrammerTypes.Add(new KeyValuePair<ProgrammerType, string>(ProgrammerType.Stub, "Stub"));
            ProgrammerType = GetConfigEnum(ProgrammerType.StkV1, "ProgrammerType");

            _devices.Clear();
            DeviceInfo.List().ToList().ForEach(_devices.Add);
            var deviceName = GetConfigString("atmega328p", "DeviceName");
            Device = _devices.FirstOrDefault(item => item.Name.ToLowerInvariant() == deviceName.ToLowerInvariant());
        }

        public static FlasherConfig Read() {
            var res = new FlasherConfig();
            res.ReadFromConfig();
            return res;
        }

        public override void Save() {
            UpdateConfig(ProgrammerType.ToString(), "ProgrammerType");
            UpdateConfig(Device.Name, "DeviceName");
            UpdateConfigBool("Verify", AutoVerify);
            _stkv1.Save();
            _comBitBang.Save();
            _stub.Save();
        }

        public BaseProgrammerConfig GetProgrammerConfig() {
            switch (ProgrammerType) {
                case ProgrammerType.StkV1:
                    return _stkv1;
                case ProgrammerType.ComBitBang:
                    return _comBitBang;
                case ProgrammerType.Stub:
                    return _stub;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
