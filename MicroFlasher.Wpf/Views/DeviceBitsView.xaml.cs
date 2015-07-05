using System.Windows;
using System.Windows.Controls;
using MicroFlasher.Devices;
using MicroFlasher.Hex;
using MicroFlasher.Models;

namespace MicroFlasher.Views {
    /// <summary>
    /// Interaction logic for DeviceBitsView.xaml
    /// </summary>
    public partial class DeviceBitsView : UserControl {

        public DeviceBitsView() {
            InitializeComponent();
        }

        private void RefreshBoard() {
            var deviceBits = DeviceBits;
            if (deviceBits != null) {
                var board = Board ?? new HexBoard();
                deviceBits.ApplyTo(board);
                BoardView.DataContext = board;
            }
        }

        private void OnCheckChanged(object sender, RoutedEventArgs e) {
            RefreshBoard();
        }

        private void DeviceBitsView_OnLoaded(object sender, RoutedEventArgs e) {
            RefreshBoard();
        }

        private void DeviceBitsView_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            RefreshBoard();
        }

        private DeviceBitsModel Model {
            get { return DataContext as DeviceBitsModel; }
        }

        private HexBoard Board {
            get {
                var m = Model;
                return m != null ? m.HexBoard : null;
            }
        }


        private DeviceBits DeviceBits {
            get {
                var m = Model;
                return m != null ? m.DeviceBits : null;
            }
        }
    }
}
