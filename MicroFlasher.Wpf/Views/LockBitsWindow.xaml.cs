using System.Windows;
using System.Windows.Input;
using MicroFlasher.Commands;
using MicroFlasher.Models;
using MicroFlasher.Views.Operations;

namespace MicroFlasher.Views {
    /// <summary>
    /// Interaction logic for LockBitsWindow.xaml
    /// </summary>
    public partial class LockBitsWindow : Window {

        public LockBitsWindow() {
            InitializeComponent();
        }

        private void CloseCommand(object sender, ExecutedRoutedEventArgs e) {
            Close();
        }

        private void SaveCommand(object sender, ExecutedRoutedEventArgs e) {
            Model.LocksHexBoard.LoadFrom(((DeviceBitsModel)DeviceBitsView.DataContext).HexBoard);

            FlasherCommands.WriteLockBits.Execute(null, Owner);
            Close(); 
        }

        private void LockBitsWindow_OnLoaded(object sender, RoutedEventArgs e) {
            var dlg = new ReadLockBitsWindow {
                DataContext = new FlasherOperationModel(Model),
                Owner = this
            };
            dlg.ShowDialog();

            var settings = Model.Config;
            var lockBits = settings.Device.LockBits;

            var locksData = Model.LocksHexBoard;
            lockBits.ApplyFrom(locksData);

            DeviceBitsView.DataContext = new DeviceBitsModel {
                HexBoard = Model.LocksHexBoard.Clone(),
                DeviceBits = lockBits
            };
        }

        protected FlasherModel Model {
            get { return ((FlasherModel)DataContext); }
        }
    }
}
