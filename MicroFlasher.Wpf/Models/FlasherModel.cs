using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Atmega.Hex;
using MicroFlasher.Hex;

namespace MicroFlasher.Models {
    public class FlasherModel : INotifyPropertyChanged {

        public const int EEPROM_MAX_SUPERPAGE = 32;

        private HexBoard _eepromHexBoard;
        private HexBoard _flashHexBoard;
        private HexBoard _fusesHexBoard;
        private HexBoard _locksHexBoard;

        public FlasherModel() {
            FlashHexBoard = new HexBoard();
            EepromHexBoard = new HexBoard();
            FusesHexBoard = new HexBoard();
            LocksHexBoard = new HexBoard();
        }

        public void OpenFlash(string filePath) {
            var hexFile = HexFile.Load(filePath);
            FlashHexBoard = HexBoard.From(hexFile);
        }

        public void OpenEeprom(string filePath) {
            var hexFile = HexFile.Load(filePath);
            EepromHexBoard = HexBoard.From(hexFile);
        }

        private void board_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "Size") {
                OnPropertyChanged("TotalSize");
            }
        }

        public int TotalSize {
            get { return FlashHexBoard.Size + EepromHexBoard.Size + FusesHexBoard.Size + LocksHexBoard.Size; }
        }

        #region Boards

        public HexBoard EepromHexBoard {
            get {
                return _eepromHexBoard;
            }
            set {
                if (_eepromHexBoard != value) {
                    if (_eepromHexBoard != null) {
                        _eepromHexBoard.PropertyChanged -= board_PropertyChanged;
                    }
                    _eepromHexBoard = value;
                    if (_eepromHexBoard != null) {
                        _eepromHexBoard.PropertyChanged += board_PropertyChanged;
                    }
                    OnPropertyChanged();
                    OnPropertyChanged("TotalSize");
                }
            }
        }

        public HexBoard FlashHexBoard {
            get {
                return _flashHexBoard;
            }
            set {
                if (_flashHexBoard != value) {
                    if (_flashHexBoard != null) {
                        _flashHexBoard.PropertyChanged -= board_PropertyChanged;
                    }
                    _flashHexBoard = value;
                    if (_flashHexBoard != null) {
                        _flashHexBoard.PropertyChanged += board_PropertyChanged;
                    }
                    OnPropertyChanged();
                    OnPropertyChanged("TotalSize");
                }
            }
        }

        public HexBoard FusesHexBoard {
            get {
                return _fusesHexBoard;
            }
            set {
                if (_fusesHexBoard != value) {
                    if (_fusesHexBoard != null) {
                        _fusesHexBoard.PropertyChanged -= board_PropertyChanged;
                    }
                    _fusesHexBoard = value;
                    if (_fusesHexBoard != null) {
                        _fusesHexBoard.PropertyChanged += board_PropertyChanged;
                    }
                    OnPropertyChanged();
                    OnPropertyChanged("TotalSize");
                }
            }
        }

        public HexBoard LocksHexBoard {
            get {
                return _locksHexBoard;
            }
            set {
                if (_locksHexBoard != value) {
                    if (_locksHexBoard != null) {
                        _locksHexBoard.PropertyChanged -= board_PropertyChanged;
                    }
                    _locksHexBoard = value;
                    if (_locksHexBoard != null) {
                        _locksHexBoard.PropertyChanged += board_PropertyChanged;
                    }
                    OnPropertyChanged();
                    OnPropertyChanged("TotalSize");
                }
            }
        }

        #endregion

        private FlasherConfig _config;
        public FlasherConfig Config {
            get {
                if (_config == null) {
                    _config = FlasherConfig.Read();
                    OnPropertyChanged();
                }
                return _config;
            }
        }

        public FlasherConfig ReloadConfig() {
            _config = null;
            return Config;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null) {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool ReadFuses(DeviceOperation op) {
            var device = Config.Device;
            var fusesSize = device.FuseBits.Size;
            op.FusesSize += fusesSize;

            var fusesData = new byte[fusesSize];
            using (var programmer = CreateProgrammer(op)) {
                using (programmer.Start()) {
                    programmer.ReadPage(device.FuseBits.StartAddress, device.FuseBits.Location ?? AvrMemoryType.FuseBits, fusesData, 0, fusesSize);
                }
            }
            op.CurrentState = "Everything is done";

            FusesHexBoard = HexBoard.From(fusesData, device.FuseBits.StartAddress);

            return true;
        }

        public bool ReadLocks(DeviceOperation op) {
            var device = Config.Device;
            var locksSize = device.LockBits.Size;
            op.LocksSize += locksSize;

            var locksData = new byte[locksSize];
            using (var programmer = CreateProgrammer(op)) {
                using (programmer.Start()) {
                    programmer.ReadPage(device.LockBits.StartAddress, device.LockBits.Location ?? AvrMemoryType.LockBits, locksData, 0, locksSize);
                }
            }
            op.CurrentState = "Everything is done";

            LocksHexBoard = HexBoard.From(locksData, device.LockBits.StartAddress);

            return true;
        }

        #region Read

        public bool ReadDevice(DeviceOperation op) {
            var device = Config.Device;
            var flashSize = device.Flash.Size;
            var eepromSize = device.Eeprom.Size;
            var fusesSize = device.FuseBits.Size;
            var locksSize = device.LockBits.Size;
            op.FlashSize += flashSize;
            op.EepromSize += eepromSize;
            op.FusesSize += fusesSize;
            op.LocksSize += locksSize;

            var flashData = new byte[flashSize];
            var eepData = new byte[eepromSize];
            var fusesData = new byte[fusesSize];
            var locksData = new byte[locksSize];
            using (var programmer = CreateProgrammer(op)) {
                using (programmer.Start()) {
                    programmer.ReadPage(0, AvrMemoryType.Flash, flashData, 0, flashSize);
                    programmer.ReadPage(0, AvrMemoryType.Eeprom, eepData, 0, eepromSize);
                    programmer.ReadPage(device.FuseBits.StartAddress, device.FuseBits.Location ?? AvrMemoryType.FuseBits, fusesData, 0, fusesSize);
                    programmer.ReadPage(device.LockBits.StartAddress, device.LockBits.Location ?? AvrMemoryType.LockBits, locksData, 0, locksSize);
                }
            }
            op.CurrentState = "Everything is done";

            EepromHexBoard = HexBoard.From(eepData);
            FlashHexBoard = HexBoard.From(flashData);
            FusesHexBoard = HexBoard.From(fusesData, device.FuseBits.StartAddress);
            LocksHexBoard = HexBoard.From(locksData, device.LockBits.StartAddress);

            return true;
        }

        public bool ReadFlash(DeviceOperation op) {
            var device = Config.Device;
            var flashSize = device.Flash.Size;
            op.FlashSize += flashSize;

            var flashData = new byte[flashSize];
            using (var programmer = CreateProgrammer(op)) {
                using (programmer.Start()) {
                    programmer.ReadPage(0, AvrMemoryType.Flash, flashData, 0, flashSize);
                }
            }
            op.CurrentState = "Everything is done";

            FlashHexBoard = HexBoard.From(flashData);

            return true;
        }

        public bool ReadEeprom(DeviceOperation op) {
            var device = Config.Device;
            var eepromSize = device.Eeprom.Size;
            op.EepromSize += eepromSize;

            var eepData = new byte[eepromSize];
            using (var programmer = CreateProgrammer(op)) {
                using (programmer.Start()) {
                    programmer.ReadPage(0, AvrMemoryType.Eeprom, eepData, 0, eepromSize);
                }
            }
            op.CurrentState = "Everything is done";

            EepromHexBoard = HexBoard.From(eepData);

            return true;
        }

        #endregion

        #region Write

        public bool WriteDevice(DeviceOperation op) {
            var device = Config.Device;

            var flashBlocks = FlashHexBoard.SplitBlocks(device.Flash.PageSize);
            var eepromBlocks = EepromHexBoard.SplitBlocks(device.Eeprom.PageSize, EEPROM_MAX_SUPERPAGE);

            op.FlashSize += flashBlocks.TotalBytes;
            op.EepromSize += eepromBlocks.TotalBytes;

            using (var programmer = CreateProgrammer(op)) {
                using (programmer.Start()) {
                    foreach (var block in flashBlocks.Blocks) {
                        programmer.WritePage(block.Address, AvrMemoryType.Flash, block.Data, 0, block.Data.Length);
                    }

                    foreach (var block in eepromBlocks.Blocks) {
                        programmer.WritePage(block.Address, AvrMemoryType.Eeprom, block.Data, 0, block.Data.Length);
                    }
                }
            }
            op.CurrentState = "Everything is done";

            return true;
        }

        public bool WriteFlash(DeviceOperation op) {
            var device = Config.Device;

            var flashBlocks = FlashHexBoard.SplitBlocks(device.Flash.PageSize);

            op.FlashSize += flashBlocks.TotalBytes;

            using (var programmer = CreateProgrammer(op)) {
                using (programmer.Start()) {
                    foreach (var block in flashBlocks.Blocks) {
                        programmer.WritePage(block.Address, AvrMemoryType.Flash, block.Data, 0, block.Data.Length);
                    }
                }
            }
            op.CurrentState = "Everything is done";

            return true;
        }

        public bool WriteEeprom(DeviceOperation op) {
            var device = Config.Device;

            var eepromBlocks = EepromHexBoard.SplitBlocks(device.Eeprom.PageSize, EEPROM_MAX_SUPERPAGE);

            op.EepromSize += eepromBlocks.TotalBytes;

            using (var programmer = CreateProgrammer(op)) {
                using (programmer.Start()) {
                    foreach (var block in eepromBlocks.Blocks) {
                        programmer.WritePage(block.Address, AvrMemoryType.Eeprom, block.Data, 0, block.Data.Length);
                    }
                }
            }
            op.CurrentState = "Everything is done";

            return true;
        }

        public bool WriteFuses(DeviceOperation op) {
            var device = Config.Device;

            var fusesBlocks = FusesHexBoard.SplitBlocks(device.FuseBits.PageSize);
            op.FusesSize += fusesBlocks.TotalBytes;

            using (var programmer = CreateProgrammer(op)) {
                using (programmer.Start()) {
                    foreach (var block in fusesBlocks.Blocks) {
                        programmer.WritePage(block.Address, device.FuseBits.Location ?? AvrMemoryType.FuseBits, block.Data, 0, block.Data.Length);
                    }
                }
            }
            op.CurrentState = "Everything is done";

            return true;
        }

        public bool WriteLocks(DeviceOperation op) {
            var device = Config.Device;

            var locksBlocks = LocksHexBoard.SplitBlocks(device.LockBits.PageSize);
            op.LocksSize += locksBlocks.TotalBytes;

            using (var programmer = CreateProgrammer(op)) {
                using (programmer.Start()) {
                    foreach (var block in locksBlocks.Blocks) {
                        programmer.WritePage(block.Address, device.LockBits.Location ?? AvrMemoryType.LockBits, block.Data, 0, block.Data.Length);
                    }
                }
            }
            op.CurrentState = "Everything is done";

            return true;
        }

        #endregion

        #region Verify

        public bool VerifyDevice(DeviceOperation op) {
            var device = Config.Device;

            var flashBlocks = FlashHexBoard.SplitBlocks(device.Flash.PageSize);
            var eepromBlocks = EepromHexBoard.SplitBlocks(device.Eeprom.PageSize);

            op.FlashSize += flashBlocks.TotalBytes;
            op.EepromSize += eepromBlocks.TotalBytes;

            using (var programmer = CreateProgrammer(op)) {
                using (programmer.Start()) {
                    if (!VerifyBlocks(programmer, flashBlocks, AvrMemoryType.Flash, op)) {
                        return false;
                    }

                    if (!VerifyBlocks(programmer, eepromBlocks, AvrMemoryType.Eeprom, op)) {
                        return false;
                    }
                }
            }
            op.Complete();
            op.CurrentState = "Everything is done";

            return true;
        }

        public bool VerifyFlash(DeviceOperation op) {
            var device = Config.Device;

            var flashBlocks = FlashHexBoard.SplitBlocks(device.Flash.PageSize);

            op.FlashSize += flashBlocks.TotalBytes;

            using (var programmer = CreateProgrammer(op)) {
                using (programmer.Start()) {
                    if (!VerifyBlocks(programmer, flashBlocks, AvrMemoryType.Flash, op)) {
                        return false;
                    }
                }
            }
            op.Complete();
            op.CurrentState = "Everything is done";

            return true;
        }

        public bool VerifyEeprom(DeviceOperation op) {
            var device = Config.Device;

            var eepromBlocks = EepromHexBoard.SplitBlocks(device.Eeprom.PageSize);

            op.EepromSize += eepromBlocks.TotalBytes;

            using (var programmer = CreateProgrammer(op)) {
                using (programmer.Start()) {
                    if (!VerifyBlocks(programmer, eepromBlocks, AvrMemoryType.Eeprom, op)) {
                        return false;
                    }
                }
            }
            op.Complete();
            op.CurrentState = "Everything is done";

            return true;
        }

        public bool VerifyFuseBits(DeviceOperation op) {
            var device = Config.Device;

            var fuseBlocks = FusesHexBoard.SplitBlocks(1);

            op.FusesSize += fuseBlocks.TotalBytes;

            using (var programmer = CreateProgrammer(op)) {
                using (programmer.Start()) {
                    if (!VerifyBlocks(programmer, fuseBlocks, device.FuseBits.Location ?? AvrMemoryType.FuseBits, op)) {
                        return false;
                    }
                }
            }
            op.Complete();
            op.CurrentState = "Everything is done";

            return true;
        }

        public bool VerifyLockBits(DeviceOperation op) {
            var device = Config.Device;

            var lockBlocks = LocksHexBoard.SplitBlocks(1);

            op.LocksSize += lockBlocks.TotalBytes;

            using (var programmer = CreateProgrammer(op)) {
                using (programmer.Start()) {
                    if (!VerifyBlocks(programmer, lockBlocks, device.LockBits.Location ?? AvrMemoryType.LockBits, op)) {
                        return false;
                    }
                }
            }
            op.Complete();
            op.CurrentState = "Everything is done";

            return true;
        }

        #endregion

        public bool EraseDevice(DeviceOperation op) {
            var device = Config.Device;

            op.FlashSize += device.Flash.Size;
            op.EepromSize += device.Eeprom.Size;

            using (var programmer = CreateProgrammer(op)) {
                using (programmer.Start()) {
                    programmer.EraseDevice();
                }
            }

            op.Complete();
            op.CurrentState = "Everything is done";

            return true;
        }

        private static bool VerifyBlocks(IProgrammer programmer, HexBlocks blocks, AvrMemoryType memType, DeviceOperation op) {
            if (blocks.Blocks.All(block => VerifyBlock(programmer, block, memType, op))) {
                return true;
            }
            op.Complete();
            op.Status = DeviceOperationStatus.Error;
            return false;
        }

        private static bool VerifyBlock(IProgrammer programmer, HexBlock block, AvrMemoryType memType, DeviceOperation op) {
            var actualData = new byte[block.Data.Length];
            programmer.ReadPage(block.Address, memType, actualData, 0, actualData.Length);
            for (var i = 0; i < block.Data.Length; i++) {
                var actual = actualData[i];
                var expected = block.Data[i];
                var address = i + block.Address;
                if (!op.Device.Verify(memType, address, actual, expected)) {
                    op.CurrentState = string.Format("Verification failed at {0}:0x{1:x4}.\r\nExpected 0x{2:x2} but was 0x{3:x2}", memType, i + block.Address, expected, actual);
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> ReadDeviceAsync(DeviceOperation op) {
            return await Task.Run(() => ReadDevice(op), op.CancellationToken);
        }

        public async Task<bool> WriteDeviceAsync(DeviceOperation op) {
            return await Task.Run(() => WriteDevice(op), op.CancellationToken);
        }

        public async Task<bool> VerifyDeviceAsync(DeviceOperation op) {
            return await Task.Run(() => VerifyDevice(op), op.CancellationToken);
        }

        public async Task<bool> VerifyFlashAsync(DeviceOperation op) {
            return await Task.Run(() => VerifyFlash(op), op.CancellationToken);
        }

        public async Task<bool> VerifyEepromAsync(DeviceOperation op) {
            return await Task.Run(() => VerifyEeprom(op), op.CancellationToken);
        }

        public async Task<bool> VerifyLockBitsAsync(DeviceOperation op) {
            return await Task.Run(() => VerifyLockBits(op), op.CancellationToken);
        }

        public async Task<bool> VerifyFuseBitsAsync(DeviceOperation op) {
            return await Task.Run(() => VerifyFuseBits(op), op.CancellationToken);
        }

        public async Task<bool> EraseDeviceAsync(DeviceOperation op) {
            return await Task.Run(() => EraseDevice(op), op.CancellationToken);
        }

        public async Task<bool> ReadFlashAsync(DeviceOperation op) {
            return await Task.Run(() => ReadFlash(op), op.CancellationToken);
        }

        public async Task<bool> ReadEepromAsync(DeviceOperation op) {
            return await Task.Run(() => ReadEeprom(op), op.CancellationToken);
        }
        public async Task<bool> ReadFusesAsync(DeviceOperation op) {
            return await Task.Run(() => ReadFuses(op), op.CancellationToken);
        }

        public async Task<bool> ReadLocksAsync(DeviceOperation op) {
            return await Task.Run(() => ReadLocks(op), op.CancellationToken);
        }

        public async Task<bool> WriteFlashAsync(DeviceOperation op) {
            return await Task.Run(() => WriteFlash(op), op.CancellationToken);
        }

        public async Task<bool> WriteEepromAsync(DeviceOperation op) {
            return await Task.Run(() => WriteEeprom(op), op.CancellationToken);
        }

        public async Task<bool> WriteLocksAsync(DeviceOperation op) {
            return await Task.Run(() => WriteLocks(op), op.CancellationToken);
        }

        public async Task<bool> WriteFusesAsync(DeviceOperation op) {
            return await Task.Run(() => WriteFuses(op), op.CancellationToken);
        }

        public void SaveFile(string fileName) {
            var hfb = new HexFileBuilder();
            foreach (var sourceLine in FlashHexBoard.Lines) {
                hfb.SetAddress(sourceLine.Address);
                foreach (var bt in sourceLine.Bytes) {
                    hfb.WriteByte(bt.Value);
                }
            }
            var hf = hfb.Build();
            hf.Save(fileName);
        }

        private IProgrammer CreateProgrammer(DeviceOperation deviceOperation) {
            var settings = Config;
            var inner = CreateProgrammerFromConfig(settings);
            var programmer = new ProgressTrackerProgrammer(inner, deviceOperation);
            return programmer;
        }

        private static IProgrammer CreateProgrammerFromConfig(FlasherConfig settings) {
            var device = settings.Device;

            var programmerConfig = settings.GetProgrammerConfig();
            return programmerConfig.CreateProgrammer(device);
        }

        public void ClearAll() {
            FlashHexBoard.Clear();
            EepromHexBoard.Clear();
            FusesHexBoard.Clear();
            LocksHexBoard.Clear();
        }

        public async Task ResetDeviceAsync() {
            var settings = Config;
            using (var channel = settings.GetProgrammerConfig().CreateChannel()) {
                channel.Open();
                channel.ToggleReset(true);
                await Task.Delay(100);
                channel.ToggleReset(false);
                await Task.Delay(100);
                channel.Close();
            }
        }

    }
}
