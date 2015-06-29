using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace MicroFlasher.Views {
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window {
        
        public AboutWindow() {
            InitializeComponent();
        }

        private void hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            if (e.Uri != null && string.IsNullOrEmpty(e.Uri.OriginalString) == false) {
                string uri = e.Uri.AbsoluteUri;
                Process.Start(new ProcessStartInfo(uri));

                e.Handled = true;
            }
        }
    }
}
