using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using MicroFlasher.Annotations;

namespace MicroFlasher {
    public class ObservableDeviceOperation : DeviceOperation, INotifyPropertyChanged {

        private int _eepromDone;
        private int _flashSize;
        private int _eepromSize;
        private int _flashDone;
        private string _currentState;
        private DeviceOperationStatus _status;

        public ObservableDeviceOperation(CancellationTokenSource cancellationTokenSource)
            : base(cancellationTokenSource) {
        }

        public override int EepromDone {
            get { return _eepromDone; }
            set {
                if (value != _eepromDone) {
                    _eepromDone = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Progress");
                    OnPropertyChanged("ProgressFraction");
                }
            }
        }

        public override int FlashDone {
            get { return _flashDone; }
            set {
                if (value != _flashDone) {
                    _flashDone = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Progress");
                    OnPropertyChanged("ProgressFraction");
                }
            }
        }

        public override int FlashSize {
            get { return _flashSize; }
            set {
                if (value != _flashSize) {
                    _flashSize = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Progress");
                    OnPropertyChanged("ProgressFraction");
                }
            }
        }

        public override int EepromSize {
            get { return _eepromSize; }
            set {
                if (value != _eepromSize) {
                    _eepromSize = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Progress");
                    OnPropertyChanged("ProgressFraction");
                }
            }
        }

        public override string CurrentState {
            get { return _currentState; }
            set {
                if (value == _currentState) return;
                _currentState = value;
                OnPropertyChanged();
            }
        }

        public override DeviceOperationStatus Status {
            get { return _status; }
            set {
                if (value != _status) {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
