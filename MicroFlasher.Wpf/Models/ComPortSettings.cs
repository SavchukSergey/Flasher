using System.Collections.ObjectModel;
using System.IO.Ports;

namespace MicroFlasher.Models {
    public class ComPortSettings : BaseConfig {

        private string _comPort;
        private int _baudRate;
        private readonly ObservableCollection<string> _comPorts = new ObservableCollection<string>();
        private readonly ObservableCollection<int> _baudRates = new ObservableCollection<int>();

        public ComPortSettings(string keyPrefix)
            : base(keyPrefix) {
        }

        public string ComPort {
            get { return _comPort; }
            set {
                if (_comPort != value) {
                    _comPort = value;
                    OnPropertyChanged();
                }
            }
        }

        public int BaudRate {
            get {
                return _baudRate;
            }
            set {
                if (_baudRate != value) {
                    _baudRate = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> ComPorts { get { return _comPorts; } }

        public ObservableCollection<int> BaudRates { get { return _baudRates; } }

        public override void Save() {
            UpdateConfig(ComPort, "ComPort");
            UpdateConfig(BaudRate.ToString(), "BaudRate");
        }

        public override void ReadFromConfig() {
            ComPort = GetConfigString("COM1", "ComPort");
            BaudRate = GetConfigInt(57600, "BaudRate");

            ComPorts.Clear();
            foreach (var port in SerialPort.GetPortNames()) {
                ComPorts.Add(port);
            }
            if (ComPorts.Count == 0) {
                ComPorts.Add("COM1");
            }

            BaudRates.Clear();
            BaudRates.Add(300);
            BaudRates.Add(1200);
            BaudRates.Add(2400);
            BaudRates.Add(4800);
            BaudRates.Add(9600);
            BaudRates.Add(14400);
            BaudRates.Add(19200);
            BaudRates.Add(28800);
            BaudRates.Add(38400);
            BaudRates.Add(57600);
            BaudRates.Add(115200);
        }
        public SerialPort CreateSerialPort() {
            return new SerialPort(ComPort) {
                BaudRate = BaudRate,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One
            };
        }
    }
}
