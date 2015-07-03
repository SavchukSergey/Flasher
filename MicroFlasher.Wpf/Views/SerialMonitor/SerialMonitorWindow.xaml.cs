using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using MicroFlasher.Models;
using MicroFlasher.Monitor;
using MicroFlasher.Views.Operations;

namespace MicroFlasher.Views.SerialMonitor {
    /// <summary>
    /// Interaction logic for SerialMonitorWindow.xaml
    /// </summary>
    public partial class SerialMonitorWindow : Window {

        private StatisticsChannel _channel;

        private readonly CancellationTokenSource _source = new CancellationTokenSource();

        private Run _receivedRun;
        private Run _runToFocus;


        private Style _myStyle;
        private Style _otherStyle;
        private Style _noneStyle;

        private int _bytesReceived;
        private int _bytesSent;

        private Task _listenTask;

        private DispatcherTimer _statusTimer;
        private DispatcherTimer _navigateTimer;

        private SerialMonitorMessage _receivedMessage;

        private readonly Monitor.SerialMonitor _serialMonitor;

        private readonly object _channelSync = new object();

        private void OnMessageChanged(SerialMonitorMessage msg) {
            var prev = _receivedMessage;
            if (prev != msg) {
                _receivedRun = new Run();
                var para = new Paragraph(_receivedRun) { Style = _otherStyle };
                MessageLog.Document.Blocks.Add(para);
            }
            _receivedRun = _receivedRun ?? new Run();
            if (_receivedRun.Text.Length != msg.Length) {
                _receivedRun.Text = msg.Content.ToString();
            }
            _receivedMessage = msg;
            _runToFocus = _receivedRun;
        }


        public SerialMonitorWindow() {
            InitializeComponent();
            _serialMonitor = new Monitor.SerialMonitor(() => _channel, msg => {
                Dispatcher.Invoke(() => {
                    OnMessageChanged(msg);
                });
            });
        }

        private void MessageToSend_OnKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                SendMessage(MessageToSend.Text);
                MessageToSend.Text = "";
            }
        }

        private void SendMessage(string content) {
            var run = new Run(content);
            var para = new Paragraph(run);
            MessageLog.Document.Blocks.Add(para);
            var res = _serialMonitor.SendMessage(content);
            var style = res ? _myStyle : _noneStyle;
            para.Style = style;

            _runToFocus = run;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            SendMessage(MessageToSend.Text);
            MessageToSend.Text = "";
        }

        private void SerialMonitorWindow_OnLoaded(object sender, RoutedEventArgs e) {
            _myStyle = (Style)Resources["MyMessage"];
            _otherStyle = (Style)Resources["OtherMessage"];
            _noneStyle = (Style)Resources["NoneMessage"];
            OpenChannel();
            _listenTask = Task.Run(() => ListenLoop());

            _statusTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            _statusTimer.Tick += StatusTimerTick;
            _statusTimer.Start();

            _navigateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            _navigateTimer.Tick += NavigateTimerTick;
            _navigateTimer.Start();

        }

        private void ListenLoop() {
            _serialMonitor.ListenLoop(_source.Token);
        }

        protected override void OnClosed(EventArgs e) {
            _source.Cancel();

            _statusTimer.Tick -= StatusTimerTick;
            _statusTimer.Stop();

            _navigateTimer.Tick -= NavigateTimerTick;
            _navigateTimer.Stop();

            _listenTask.Wait();

            CloseChannel();

            base.OnClosed(e);
        }

        private void ClearLog(object sender, RoutedEventArgs e) {
            MessageLog.Document.Blocks.Clear();
            MessageToSend.Focus();
        }

        private void ResetDevice(object sender, ExecutedRoutedEventArgs e) {
            lock (_channelSync) {
                CloseChannel();
                var dlg = new ResetDeviceWindow {
                    DataContext = new FlasherOperationModel(Model),
                    Owner = this
                };
                dlg.ShowDialog();
            }
        }

        protected FlasherModel Model {
            get { return (FlasherModel)DataContext; }
        }

        private void MessageToSend_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            if (e.NewFocus is ContextMenu) {
                return;
            }
            if (e.NewFocus == null || !e.NewFocus.Equals(MessageLog)) {
                ((FrameworkElement)sender).Focus();
            }
        }

        private void StatusTimerTick(object sender, EventArgs e) {
            var ch = OpenChannel();
            ChannelName.Text = ch != null && ch.IsOpen ? ch.Name : "---";
            var sent = _bytesSent;
            var received = _bytesReceived;
            if (ch != null && ch.IsOpen) {
                sent += ch.BytesSent;
                received += ch.BytesReceived;
            }
            BytesReceived.Text = received.ToString();
            BytesSent.Text = sent.ToString();
        }

        private void NavigateTimerTick(object sender, EventArgs e) {
            var run = _runToFocus;
            _runToFocus = null;
            if (run != null && run.Text.Length > 0) {
                run.BringIntoView();
            }
        }

        #region channel operations

        private StatisticsChannel OpenChannel() {
            lock (_channelSync) {
                try {
                    if (_channel == null) {
                        _channel = new StatisticsChannel(FlasherConfig.Read().GetProgrammerConfig().CreateChannel());
                    }
                    if (!_channel.IsOpen) {
                        _channel.Open();
                    }
                    return _channel;
                } catch (Exception) {
                    CloseChannel();
                    return null;
                }
            }
        }

        private void CloseChannel() {
            lock (_channelSync) {
                var ch = _channel;
                _channel = null;
                if (ch != null) {
                    _bytesReceived += ch.BytesReceived;
                    _bytesSent += ch.BytesSent;
                    if (ch.IsOpen) {
                        ch.Close();
                    }
                    ch.Dispose();
                }
            }
        }

        #endregion
    }
}
