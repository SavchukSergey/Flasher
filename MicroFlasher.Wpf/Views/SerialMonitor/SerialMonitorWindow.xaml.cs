using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using MicroFlasher.IO;
using MicroFlasher.Models;
using MicroFlasher.Views.Operations;

namespace MicroFlasher.Views.SerialMonitor {
    /// <summary>
    /// Interaction logic for SerialMonitorWindow.xaml
    /// </summary>
    public partial class SerialMonitorWindow : Window {

        private IAvrChannel _channel;
        private readonly CancellationTokenSource _source = new CancellationTokenSource();

        private Run _receivedRun;

        private DateTime _receivedDateTime;
        private readonly TimeSpan _threshold = TimeSpan.FromMilliseconds(500);
        private Style _myStyle;
        private Style _otherStyle;

        public SerialMonitorWindow() {
            InitializeComponent();
        }

        private void MessageToSend_OnKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                SendMessage(MessageToSend.Text);
                MessageToSend.Text = "";
            }
        }

        private void SendMessage(string content) {
            var para = new Paragraph(new Run(content)) { Style = _myStyle };
            MessageLog.Document.Blocks.Add(para);
            foreach (var ch in content) {
                _channel.SendByte((byte)ch);
            }
            FlushReceived();
        }

        private void FlushReceived() {
            if (_receivedRun != null && _receivedRun.Text.Length > 0) {
                MessageLog.ScrollToEnd();
                _receivedRun = null;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            SendMessage(MessageToSend.Text);
            MessageToSend.Text = "";
        }

        private async void SerialMonitorWindow_OnLoaded(object sender, RoutedEventArgs e) {
            _channel = FlasherConfig.Read().GetProgrammerConfig().CreateChannel();
            _channel.Open();
            _myStyle = (Style)Resources["MyMessage"];
            _otherStyle = (Style)Resources["OtherMessage"];
            var disp = Dispatcher;
            await Task.Run(() => Listen(disp));
        }

        private void Listen(Dispatcher disp) {
            while (!_source.IsCancellationRequested) {
                byte? bt;
                try {
                    bt = _channel.ReceiveByte();
                } catch (Exception) {
                    bt = null;
                }

                var ts = DateTime.UtcNow;
                var delta = ts - _receivedDateTime;
                _receivedDateTime = ts;

                disp.Invoke(() => {
                    if (delta > _threshold) {
                        FlushReceived();
                    }

                    if (bt.HasValue) {
                        if (_receivedRun == null) {
                            _receivedRun = new Run();
                            var para = new Paragraph(_receivedRun) { Style = _otherStyle };
                            MessageLog.Document.Blocks.Add(para);
                        }
                        _receivedRun.Text += (char)bt;
                    }
                });
            }
        }

        protected override void OnClosed(EventArgs e) {
            _channel.Dispose();
            _source.Cancel();
            base.OnClosed(e);
        }

        private void ClearLog(object sender, RoutedEventArgs e) {
            MessageLog.Document.Blocks.Clear();
        }


        private bool _altMode;
        private int? _altChar;

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
                        var caretIndex = MessageToSend.CaretIndex;
                        MessageToSend.Text = MessageToSend.Text.Insert(caretIndex, ((char)_altChar.Value).ToString());
                        MessageToSend.CaretIndex = caretIndex + 1;
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

        private void ResetDevice(object sender, ExecutedRoutedEventArgs e) {
            try {
                _channel.Close();
                var dlg = new ResetDeviceWindow {
                    DataContext = new FlasherOperationModel(Model),
                    Owner = this
                };
                dlg.ShowDialog();
            } finally {
                _channel.Open();
            }
        }

        protected FlasherModel Model {
            get { return (FlasherModel)DataContext; }
        }

        private void MessageToSend_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            if (e.NewFocus == null || !e.NewFocus.Equals(MessageLog)) {
                ((FrameworkElement)sender).Focus();
            }
        }
    }
}
