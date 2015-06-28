using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MicroFlasher.Models;

namespace MicroFlasher.Views.Operations {
    public abstract class BaseDeviceOperationWindow : Window {

        private Task _task;
        private readonly CancellationTokenSource _source = new CancellationTokenSource();
        private DateTime _start;

        protected BaseDeviceOperationWindow() {
            Loaded += BaseDeviceOperationWindow_Loaded;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;
        }

        private async void BaseDeviceOperationWindow_Loaded(object sender, RoutedEventArgs e) {
            var op = DeviceOperation;
            try {
                var res = await WatchAndExecute(op);
                if (res) {
                    Close();
                } else {
                    op.Status = DeviceOperationStatus.Error;
                    op.Complete();
                }
            } catch (OperationCanceledException) {
                op.CurrentState = "Operation is cancelled";
                op.Status = DeviceOperationStatus.Error;
            } catch (UnauthorizedAccessException) {
                op.CurrentState = "Cannot open communication channel";
                op.Status = DeviceOperationStatus.Error;
            } catch (Exception) {
                op.CurrentState = "Device is not ready";
                op.Status = DeviceOperationStatus.Error;
            }
            if (op.Status == DeviceOperationStatus.Error) {
                op.Complete();
            }
        }

        protected DeviceOperation DeviceOperation {
            get { return ((FlasherOperationModel)DataContext).DeviceOperation; }
        }


        protected FlasherModel Model {
            get { return ((FlasherOperationModel)DataContext).Flasher; }
        }

        protected override void OnClosed(EventArgs e) {
            DeviceOperation.Cancel();
            base.OnClosed(e);
        }

        private async Task<bool> WatchAndExecute(DeviceOperation op) {
            _start = DateTime.Now;
            var disp = Dispatcher;
            var watchTask = Task.Run(() => Loop(disp, op));
            bool res;
            try {
                res = await Execute(op);
            } finally {
                _source.Cancel();
            }
            await watchTask;
            return res;
        }

        private void Loop(Dispatcher disp, DeviceOperation op) {
            while (!_source.IsCancellationRequested) {
                Thread.Sleep(50);
                var delta = DateTime.Now - _start;
                disp.Invoke(() => {
                    op.ExecutionTime = delta;
                });
            }
        }


        protected abstract Task<bool> Execute(DeviceOperation op);

    }
}
