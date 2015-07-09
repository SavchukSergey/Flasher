using System.Windows.Input;

namespace MicroFlasher.Commands {
    public class FlasherCommands {

        private readonly static RoutedCommand _openEeprom = new RoutedCommand();
        private readonly static RoutedCommand _readDevice = new RoutedCommand();
        private readonly static RoutedCommand _writeDevice = new RoutedCommand();
        private readonly static RoutedCommand _eraseDevice = new RoutedCommand();
        private readonly static RoutedCommand _verifyDevice = new RoutedCommand();
        private readonly static RoutedCommand _lockBitsDevice = new RoutedCommand();
        private readonly static RoutedCommand _fuseBitsDevice = new RoutedCommand();
        private readonly static RoutedCommand _resetDevice = new RoutedCommand();
        private readonly static RoutedCommand _serialMonitor = new RoutedCommand();
        private readonly static RoutedCommand _settings = new RoutedCommand();
        private readonly static RoutedCommand _writeLockBits = new RoutedUICommand();
        private readonly static RoutedCommand _writeFuseBits = new RoutedUICommand();
        private readonly static RoutedCommand _verifyLockBits = new RoutedUICommand();
        private readonly static RoutedCommand _verifyFuseBits = new RoutedUICommand();
        private readonly static RoutedCommand _verifyFlash = new RoutedUICommand();
        private readonly static RoutedCommand _verifyEeprom = new RoutedUICommand();
        private readonly static RoutedCommand _clearLog = new RoutedUICommand();
        private readonly static RoutedCommand _about = new RoutedUICommand();

        public static RoutedCommand OpenEeprom {
            get { return _openEeprom; }
        }

        public static RoutedCommand ReadDevice {
            get { return _readDevice; }
        }

        public static RoutedCommand WriteDevice {
            get { return _writeDevice; }
        }

        public static RoutedCommand EraseDevice {
            get { return _eraseDevice; }
        }

        public static RoutedCommand VerifyDevice {
            get { return _verifyDevice; }
        }

        #region Flash

        public static RoutedCommand VerifyFlash {
            get { return _verifyFlash; }
        }

        #endregion

        #region Eeprom
        
        public static RoutedCommand VerifyEeprom {
            get { return _verifyEeprom; }
        }

        #endregion

        #region Fuses

        public static RoutedCommand FuseBits {
            get { return _fuseBitsDevice; }
        }

        public static RoutedCommand WriteFuseBits {
            get { return _writeFuseBits; }
        }

        public static RoutedCommand VerifyFuseBits {
            get { return _verifyFuseBits; }
        }

        #endregion

        #region Locks

        public static RoutedCommand LockBits {
            get { return _lockBitsDevice; }
        }

        public static RoutedCommand WriteLockBits {
            get { return _writeLockBits; }
        }

        public static RoutedCommand VerifyLockBits {
            get { return _verifyLockBits; }
        }

        #endregion

        public static RoutedCommand ResetDevice {
            get { return _resetDevice; }
        }

        public static RoutedCommand Settings {
            get { return _settings; }
        }

        public static RoutedCommand SerialMonitor {
            get { return _serialMonitor; }
        }

        public static RoutedCommand ClearLog {
            get { return _clearLog; }
        }

        public static RoutedCommand About {
            get { return _about; }
        }
    }
}
