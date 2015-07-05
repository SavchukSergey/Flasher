using System;
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
            DataContextChanged += HexBoardView_DataContextChanged;
        }

        private void HexBoardView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            var oldValue = e.OldValue as DeviceBitsModel;
            if (oldValue != null) {
                oldValue.HexBoard.DataChanged += HexBoardOnDataChanged;
            }
            var newValue = e.NewValue as DeviceBitsModel;
            if (newValue != null) {
                newValue.HexBoard.DataChanged += HexBoardOnDataChanged;
            }
        }

        private void HexBoardOnDataChanged(object sender, EventArgs eventArgs) {
            RefreshBits();
        }

        private bool _silence;

        private void RefreshBits() {
            try {
                _silence = true;

                var bits = DeviceBits;
                var board = Board;
                if (bits != null && board != null) {
                    bits.ApplyFrom(board);
                }
            } finally {
                _silence = false;
            }
        }

        private void RefreshBoard() {
            if (_silence) return;
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
