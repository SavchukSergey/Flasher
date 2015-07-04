using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MicroFlasher.Hex {
    public class HexBoardLine : INotifyPropertyChanged {

        private readonly HexBoardByte[] _bytes = new HexBoardByte[16];

        public int Address { get; set; }

        public HexBoardByte[] Bytes {
            get { return _bytes; }
        }

        public HexBoardLine() {
            for (var i = 0; i < _bytes.Length; i++) {
                var bt = new HexBoardByte();
                bt.PropertyChanged += bt_PropertyChanged;
                _bytes[i] = bt;
            }
        }

        public byte? this[int address] {
            get {
                return Bytes[address % 16].Value;
            }
            set {
                Bytes[address % 16].Value = value;
            }
        }

        private void bt_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            OnPropertyChanged("Characters");
        }


        public string Characters {
            get {
                var sb = new StringBuilder();
                foreach (var bt in _bytes) {
                    sb.Append(bt.Character);
                }
                return sb.ToString();
            }
        }

        public bool HasData {
            get { return _bytes.Any(b => b.Value.HasValue); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null) {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
