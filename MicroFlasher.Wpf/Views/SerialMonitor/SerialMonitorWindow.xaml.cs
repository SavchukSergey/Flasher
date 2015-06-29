using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        private Run _runToFocus;
        private StringBuilder _receivedBuffer;

        private DateTime _receivedDateTime;
        private readonly TimeSpan _threshold = TimeSpan.FromMilliseconds(500);

        private Style _myStyle;
        private Style _otherStyle;
        private Style _noneStyle;

        private int _bytesReceived;
        private int _bytesSent;

        private Task _listenTask;

        private DispatcherTimer _statusTimer;
        private DispatcherTimer _navigateTimer;
        private DispatcherTimer _flushTimer;

        private object _channelSync = new object();

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
            var channel = GetChannel();
            var style = channel != null ? _myStyle : _noneStyle;
            var run = new Run(content);
            var para = new Paragraph(run) { Style = style };
            MessageLog.Document.Blocks.Add(para);
            if (channel != null) {
                foreach (var ch in content) {
                    channel.SendByte((byte)ch);
                    _bytesSent++;
                }
            }
            FlushReceived();
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
            GetChannel();
            _listenTask = Task.Run(() => ListenLoop());

            _statusTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            _statusTimer.Tick += StatusTimerTick;
            _statusTimer.Start();

            _navigateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            _navigateTimer.Tick += NavigateTimerTick;
            _navigateTimer.Start();

            _flushTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            _flushTimer.Tick += FlushTimerTick;
            _flushTimer.Start();
        }

        private void ListenLoop() {
            while (!_source.IsCancellationRequested) {
                var bt = ReadByte();

                if (bt.HasValue) {
                    _receivedDateTime = DateTime.UtcNow;

                    var sb = _receivedBuffer;
                    if (sb == null) {
                        sb = new StringBuilder();
                        _receivedBuffer = sb;
                    }
                    sb.Append((char)bt);
                }
            }
        }

        protected override void OnClosed(EventArgs e) {
            _source.Cancel();

            _statusTimer.Tick -= StatusTimerTick;
            _statusTimer.Stop();

            _navigateTimer.Tick -= NavigateTimerTick;
            _navigateTimer.Stop();

            _flushTimer.Tick -= FlushTimerTick;
            _flushTimer.Stop();

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
            ChannelName.Text = _channel != null && _channel.IsOpen ? _channel.Name : "---";
            BytesReceived.Text = _bytesReceived.ToString();
            BytesSent.Text = _bytesSent.ToString();
        }

        private void NavigateTimerTick(object sender, EventArgs e) {
            var run = _runToFocus;
            _runToFocus = null;
            if (run != null && run.Text.Length > 0) {
                run.BringIntoView();
            }
        }

        private void FlushTimerTick(object sender, EventArgs e) {
            var ts = DateTime.UtcNow;
            var delta = ts - _receivedDateTime;

            ReceivedSync();

            if (delta > _threshold) {
                FlushReceived();
            }
        }

        private void ReceivedSync() {
            if (_receivedBuffer != null) {
                if (_receivedRun == null) {
                    _receivedRun = new Run();
                    var para = new Paragraph(_receivedRun) { Style = _otherStyle };
                    MessageLog.Document.Blocks.Add(para);
                }
                if (_receivedRun.Text.Length != _receivedBuffer.Length) {
                    _receivedRun.Text = _receivedBuffer.ToString();
                    _runToFocus = _receivedRun;
                }
            }

        }

        private void FlushReceived() {
            ReceivedSync();
            _runToFocus = _receivedRun;
            _receivedRun = null;
            _receivedBuffer = null;
        }


        #region channel operations

        private void CloseChannel() {
            lock (_channelSync) {
                var ch = _channel;
                _channel = null;
                if (ch != null) {
                    if (ch.IsOpen) {
                        ch.Close();
                    }
                    ch.Dispose();
                }
            }
        }

        private IAvrChannel GetChannel() {
            lock (_channelSync) {
                try {
                    if (_channel == null) {
                        _channel = FlasherConfig.Read().GetProgrammerConfig().CreateChannel();
                    }
                    if (!_channel.IsOpen) {
                        _channel.Open();
                    }
                    return _channel;
                } catch (Exception) {
                    return null;
                }
            }
        }

        private byte? ReadByte() {
            var channel = GetChannel();
            if (channel == null) {
                Thread.Sleep(50);
                return null;
            }
            byte? bt;
            try {
                bt = channel.ReceiveByte();
                _bytesReceived++;
            } catch (Exception) {
                bt = null;
            }

            return bt;
        }

        #endregion
    }
}
