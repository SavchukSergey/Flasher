using System;
using System.Threading.Tasks;
using System.Windows;
using MicroFlasher.Models;

namespace MicroFlasher.Views.Operations {
    public abstract class BaseDeviceOperationWindow : Window {

        protected BaseDeviceOperationWindow() {
            Loaded += BaseDeviceOperationWindow_Loaded;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;
        }

        private async void BaseDeviceOperationWindow_Loaded(object sender, RoutedEventArgs e) {
            var op = DeviceOperation;
            try {
                var res = await Execute(op);
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

        protected abstract Task<bool> Execute(DeviceOperation op);

    }
}
