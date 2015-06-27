using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Atmega.Hex;
using MicroFlasher.Hex;

namespace MicroFlasher.Models {
    public class FlasherModel : INotifyPropertyChanged {

        private HexBoard _eepromHexBoard = new HexBoard();
        private HexBoard _flashHexBoard = new HexBoard();
        private HexBoard _fusesHexBoard = new HexBoard();
        private HexBoard _locksHexBoard = new HexBoard();

        public FlasherModel() {
            _eepromHexBoard[0] = null;
            _flashHexBoard[0] = null;
            _fusesHexBoard[0] = null;
            _locksHexBoard[0] = null;
        }

        public void OpenFlash(string filePath) {
            var hexFile = HexFile.Load(filePath);
            FlashHexBoard = HexBoard.From(hexFile);
        }

        public void OpenEeprom(string filePath) {
            var hexFile = HexFile.Load(filePath);
            EepromHexBoard = HexBoard.From(hexFile);
        }

        public HexBoard EepromHexBoard {
            get {
                return _eepromHexBoard;
            }
            set {
                _eepromHexBoard = value;
                OnPropertyChanged();
            }
        }

        public HexBoard FlashHexBoard {
            get {
                return _flashHexBoard;
            }
            set {
                _flashHexBoard = value;
                OnPropertyChanged();
            }
        }

        public HexBoard FusesHexBoard {
            get {
                return _fusesHexBoard;
            }
            set {
                _fusesHexBoard = value;
                OnPropertyChanged();
            }
        }

        public HexBoard LocksHexBoard {
            get {
                return _locksHexBoard;
            }
            set {
                _locksHexBoard = value;
                OnPropertyChanged();
            }
        }

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

        public bool WriteDevice(DeviceOperation op) {
            var device = Config.Device;

            var flashBlocks = FlashHexBoard.SplitBlocks(device.Flash.PageSize);
            var eepromBlocks = EepromHexBoard.SplitBlocks(device.Eeprom.PageSize);

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

        public bool VerifyDevice(DeviceOperation op) {
            var eepromBlocks = EepromHexBoard.SplitBlocks();
            var flashBlocks = FlashHexBoard.SplitBlocks();

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

        public bool EraseDevice(DeviceOperation op) {
            var eepromBlocks = EepromHexBoard.SplitBlocks();
            var flashBlocks = FlashHexBoard.SplitBlocks();

            op.FlashSize += flashBlocks.TotalBytes;
            op.EepromSize += eepromBlocks.TotalBytes;

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
            if (blocks.Blocks.All(block => VerifyBlock(programmer, block, memType))) {
                return true;
            }
            op.Complete();
            op.CurrentState = string.Format("{0} memory verification failed", memType);
            op.Status = DeviceOperationStatus.Error;
            return false;
        }

        private static bool VerifyBlock(IProgrammer programmer, HexBlock block, AvrMemoryType memType) {
            var actual = new byte[block.Data.Length];
            programmer.ReadPage(block.Address, memType, actual, 0, actual.Length);
            return !block.Data.Where((t, i) => t != actual[i]).Any();
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

        public async Task<bool> EraseDeviceAsync(DeviceOperation op) {
            return await Task.Run(() => EraseDevice(op), op.CancellationToken);
        }

        public async Task<bool> ReadFusesAsync(DeviceOperation op) {
            return await Task.Run(() => ReadFuses(op), op.CancellationToken);
        }

        public async Task<bool> ReadLocksAsync(DeviceOperation op) {
            return await Task.Run(() => ReadLocks(op), op.CancellationToken);
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

    }
}
