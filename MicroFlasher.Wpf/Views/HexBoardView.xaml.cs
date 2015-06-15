using System.Windows;
using System.Windows.Controls;
using MicroFlasher.Hex;

namespace MicroFlasher.Views {
    /// <summary>
    /// Interaction logic for HexBoardView.xaml
    /// </summary>
    public partial class HexBoardView : Grid {

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(HexBoardView), new PropertyMetadata(""));

        public HexBoardView() {
            InitializeComponent();
        }

        public string Title {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            var context = DataContext as HexBoard;
            if (context != null) {
                context.Clear();
            }
        }
    }
}
