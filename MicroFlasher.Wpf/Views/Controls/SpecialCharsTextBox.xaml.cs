using System.Windows.Controls;
using System.Windows.Input;

namespace MicroFlasher.Views.Controls {
    /// <summary>
    /// Interaction logic for SpecialCharsTextBox.xaml
    /// </summary>
    public partial class SpecialCharsTextBox : TextBox {

        private bool _altMode;
        private int? _altChar;

        public SpecialCharsTextBox() {
            InitializeComponent();
        }

        private void MessageToSend_OnPreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.System) {
                var digit = KeyUtils.GetDigit(e.SystemKey);
                if (digit.HasValue) {
                    _altMode = true;
                    e.Handled = true;
                    _altChar = (_altChar ?? 0) * 10 + digit.Value;
                }
            }
        }


        private void MessageToSend_OnPreviewKeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.LeftAlt || e.Key == Key.RightAlt) {
                if (_altMode) {
                    _altMode = false;
                    if (_altChar.HasValue) {
                        var caretIndex = CaretIndex;
                        Text = Text.Insert(caretIndex, ((char)_altChar.Value).ToString());
                        CaretIndex = caretIndex + 1;
                        _altChar = null;
                    }
                }
            }
        }

    }
}
