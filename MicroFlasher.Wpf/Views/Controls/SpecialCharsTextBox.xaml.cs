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
                var digit = GetDigit(e.SystemKey);
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

        private static int? GetDigit(Key key) {
            switch (key) {
                case Key.D0:
                case Key.NumPad0:
                    return 0;
                case Key.D1:
                case Key.NumPad1:
                    return 1;
                case Key.D2:
                case Key.NumPad2:
                    return 2;
                case Key.D3:
                case Key.NumPad3:
                    return 3;
                case Key.D4:
                case Key.NumPad4:
                    return 4;
                case Key.D5:
                case Key.NumPad5:
                    return 5;
                case Key.D6:
                case Key.NumPad6:
                    return 6;
                case Key.D7:
                case Key.NumPad7:
                    return 7;
                case Key.D8:
                case Key.NumPad8:
                    return 8;
                case Key.D9:
                case Key.NumPad9:
                    return 9;
                default:
                    return null;
            }
        }


    }
}
