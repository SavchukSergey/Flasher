using System.Windows;
using System.Windows.Input;
using MicroFlasher.Commands;
using MicroFlasher.Devices;
using MicroFlasher.Models;
using MicroFlasher.Views.Operations;

namespace MicroFlasher.Views {
    /// <summary>
    /// Interaction logic for FuseBitsWindow.xaml
    /// </summary>
    public partial class FuseBitsWindow : Window {
        public FuseBitsWindow() {
            InitializeComponent();
        }

        private void CloseCommand(object sender, ExecutedRoutedEventArgs e) {
            Close();
        }

        private void SaveCommand(object sender, ExecutedRoutedEventArgs e) {
            var deviceBits = (DeviceBits)DeviceBitsView.DataContext;
            deviceBits.ApplyTo(Model.FusesHexBoard);

            FlasherCommands.WriteFuseBits.Execute(null, Owner);
            Close();
        }

        private void FuseBitsWindow_OnLoaded(object sender, RoutedEventArgs e) {
            var dlg = new ReadFusesWindow {
                DataContext = new FlasherOperationModel(Model),
                Owner = this
            };
            dlg.ShowDialog();

            var settings = Model.Config;
            var fuseBits = settings.Device.FuseBits;

            var fusesData = Model.FusesHexBoard;
            fuseBits.ApplyFrom(fusesData);

            DeviceBitsView.DataContext = new DeviceBitsModel {
                HexBoard = Model.FusesHexBoard,
                DeviceBits = fuseBits
            };
        }

        protected FlasherModel Model {
            get { return ((FlasherModel)DataContext); }
        }
    }
}
