using System.ComponentModel;

namespace MicroFlasher.Hex {
    public class HexBoardByte : INotifyPropertyChanged {
        private byte? _value;

        public byte? Value {
            get { return _value; }
            set {
                if (_value != value) {
                    _value = value;
                    OnPropertyChanged("Value");
                    OnPropertyChanged("Character");
                }
            }
        }

        public char Character {
            get {
                var val = Value;
                if (val.HasValue && val.Value >= 32 && val.Value < 128) {
                    var ch = (char)val;
                    return ch;
                }
                return ' ';
            }
        }

        public static implicit operator HexBoardByte(byte? val) {
            return new HexBoardByte { Value = val };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null) {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() {
            return Value.HasValue ? Value.Value.ToString("x2") : "??";
        }
    }
}
