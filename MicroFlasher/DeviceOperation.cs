using System;
using System.Threading;

namespace MicroFlasher {
    public class DeviceOperation {

        private readonly CancellationTokenSource _cancellationTokenSource;

        public DeviceOperation(CancellationTokenSource cancellationTokenSource) {
            _cancellationTokenSource = cancellationTokenSource;
        }

        public virtual int FlashDone { get; set; }

        public virtual int EepromDone { get; set; }

        public virtual int LocksDone { get; set; }

        public virtual int FusesDone { get; set; }

        public virtual int FlashSize { get; set; }

        public virtual int EepromSize { get; set; }

        public virtual int LocksSize { get; set; }

        public virtual int FusesSize { get; set; }

        public int Done {
            get { return FlashDone + EepromDone + LocksDone + FusesDone; }
        }

        public int Total {
            get { return FlashSize + EepromSize + LocksSize + FusesSize; }
        }

        public double Progress {
            get {
                if (Total == 0) return 100;
                return 100.0 * Done / Total;
            }
        }

        public double ProgressFraction {
            get { return Progress / 100; }
        }

        public virtual DeviceOperationStatus Status { get; set; }

        public virtual string CurrentState { get; set; }

        public void IncrementDone(int count, AvrMemoryType memType) {
            switch (memType) {
                case AvrMemoryType.Flash:
                    FlashDone += count;
                    break;
                case AvrMemoryType.Eeprom:
                    EepromDone += count;
                    break;
                case AvrMemoryType.LockBits:
                    LocksDone += count;
                    break;
                case AvrMemoryType.FuseBits:
                    FusesDone += count;
                    break;
            }
        }

        public void Cancel() {
            _cancellationTokenSource.Cancel();
        }

        public void Complete() {
            FlashDone = FlashSize;
            EepromDone = EepromSize;
            LocksDone = LocksSize;
            FusesDone = FusesSize;
        }

        public CancellationToken CancellationToken {
            get { return _cancellationTokenSource.Token; }
        }

        public virtual TimeSpan ExecutionTime { get; set; }

    }
}
