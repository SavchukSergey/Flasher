using System.Windows;
using System.Windows.Input;
using MicroFlasher.Models;

namespace MicroFlasher.Views {
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window {
        
        public SettingsWindow() {
            InitializeComponent();
        }

        private void CloseCommand(object sender, ExecutedRoutedEventArgs e) {
            Close();
        }

        private void SaveCommand(object sender, ExecutedRoutedEventArgs e) {
            DialogResult = true;
            Close();
        }

        protected FlasherConfig Model {
            get { return DataContext as FlasherConfig; }
        }

    }
}
