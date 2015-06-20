using System.Windows;
using System.Windows.Input;
using MicroFlasher.Models;
using MicroFlasher.Views;
using MicroFlasher.Views.Operations;
using MicroFlasher.Views.SerialMonitor;
using Atmega.Hex;
using Microsoft.Win32;

namespace MicroFlasher {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private readonly FlasherModel _model = new FlasherModel();

        public FlasherModel Model { get { return _model; } }

        public MainWindow() {
            InitializeComponent();
            DataContext = _model;
        }

        private void OpenFlashCommand(object sender, ExecutedRoutedEventArgs e) {
            var dlg = new OpenFileDialog {
                Filter = "Intel Hex File|*.hex;*.eep|All Files|*.*"
            };

            var result = dlg.ShowDialog();

            if (result == true) {
                try {
                    _model.OpenFlash(dlg.FileName);
                } catch (HexFileException exc) {
                    MessageBox.Show(this, exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenEepromCommand(object sender, ExecutedRoutedEventArgs e) {
            var dlg = new OpenFileDialog {
                Filter = "Intel Hex File|*.hex;*.eep|All Files|*.*"
            };

            var result = dlg.ShowDialog();

            if (result == true) {
                try {
                    _model.OpenEeprom(dlg.FileName);
                } catch (HexFileException exc) {
                    MessageBox.Show(this, exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveCommand(object sender, ExecutedRoutedEventArgs e) {
            var dlg = new SaveFileDialog {
                Filter = "Intel Hex File|*.hex;*.eep|All Files|*.*"
            };

            var result = dlg.ShowDialog();

            if (result == true) {
                _model.SaveFile(dlg.FileName);
            }
        }

        private void ReadDeviceCommand(object sender, ExecutedRoutedEventArgs e) {
            var dlg = new ReadDeviceWindow {
                DataContext = new FlasherOperationModel(_model),
                Owner = this
            };
            dlg.ShowDialog();
        }

        private void WriteDeviceCommand(object sender, ExecutedRoutedEventArgs e) {
            var msgResult = MessageBox.Show("Are you sure you want start writing to the device. All previous data will be lost", "Write confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (msgResult == MessageBoxResult.Yes) {
                var dlg = new WriteDeviceWindow {
                    DataContext = new FlasherOperationModel(_model),
                    Owner = this
                };
                dlg.ShowDialog();
            }
        }

        private void VerifyDeviceCommand(object sender, ExecutedRoutedEventArgs e) {
            var dlg = new VerifyDeviceWindow {
                DataContext = new FlasherOperationModel(_model),
                Owner = this
            };
            dlg.ShowDialog();
        }

        private void SettingsCommand(object sender, ExecutedRoutedEventArgs e) {
            var settings = FlasherConfig.Read();
            var dlg = new SettingsWindow {
                DataContext = settings,
                Owner = this
            };
            if (dlg.ShowDialog() ?? false) {
                settings.Save();
                Model.ReloadConfig();
            }
        }

        private void LockBitsCommand(object sender, ExecutedRoutedEventArgs e) {
            var dlg = new LockBitsWindow {
                DataContext = _model,
                Owner = this
            };
            dlg.ShowDialog();
        }

        private void FuseBitsCommand(object sender, ExecutedRoutedEventArgs e) {
            var dlg = new FuseBitsWindow {
                DataContext = _model,
                Owner = this
            };
            dlg.ShowDialog();
        }

        private void EraseDeviceCommand(object sender, ExecutedRoutedEventArgs e) {
            var msgResult = MessageBox.Show("Are you sure you want start erasing the device. All previous data will be lost", "Erase confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (msgResult == MessageBoxResult.Yes) {
                var dlg = new EraseDeviceWindow {
                    DataContext = new FlasherOperationModel(_model),
                    Owner = this
                };
                dlg.ShowDialog();
            }
        }

        private void WriteLockBitsCommand(object sender, ExecutedRoutedEventArgs e) {
            var msgResult = MessageBox.Show("Are you sure you want start writing lock bits. Data may become unreadable", "Lock bits confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (msgResult == MessageBoxResult.Yes) {
                var dlg = new WriteLocksWindow {
                    DataContext = new FlasherOperationModel(Model),
                    Owner = this
                };
                dlg.ShowDialog();
            }
        }

        private void WriteFuseBitsCommand(object sender, ExecutedRoutedEventArgs e) {
            var msgResult = MessageBox.Show("Are you sure you want start writing fuse bits. Device may become unoperable", "Fuse bits confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (msgResult == MessageBoxResult.Yes) {
                var dlg = new WriteFusesWindow {
                    DataContext = new FlasherOperationModel(Model),
                    Owner = this
                };
                dlg.ShowDialog();
            }
        }

        private void ResetDevice(object sender, ExecutedRoutedEventArgs e) {
            var dlg = new ResetDeviceWindow {
                DataContext = new FlasherOperationModel(_model),
                Owner = this
            };
            dlg.ShowDialog();
        }

        private void SerialMonitor(object sender, ExecutedRoutedEventArgs e) {
            var dlg = new SerialMonitorWindow {
                DataContext = _model,
                Owner = this
            };
            dlg.ShowDialog();
        }
    }
}
