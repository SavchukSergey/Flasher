using System.Windows;
using System.Windows.Controls;
using MicroFlasher.Devices;
using MicroFlasher.Hex;

namespace MicroFlasher.Views {
    /// <summary>
    /// Interaction logic for DeviceBitsView.xaml
    /// </summary>
    public partial class DeviceBitsView : UserControl {

        public DeviceBitsView() {
            InitializeComponent();
        }

        private void RefreshBoard() {
            var deviceBits = DataContext as DeviceBits;
            if (deviceBits != null) {
                var board = new HexBoard();
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
    }
}
