using System;
using System.Threading;

namespace MicroFlasher {
    public class StubProgrammer : IProgrammer {

        private readonly byte[] _flash = new byte[32768];
        private readonly byte[] _eeprom = new byte[1024];
        private readonly byte[] _fuses = new byte[1024 * 16];
        private readonly byte[] _locks = new byte[1024 * 16];

        public void Dispose() {
        }

        public void Start() {
        }

        public void Stop() {
        }

        public void ReadPage(int address, AvrMemoryType memType, byte[] data, int dataStart, int dataLength) {
            var mem = GetMemory(memType);
            Thread.Sleep(50);
            for (var i = 0; i < dataLength; i++) {
                data[i + dataStart] = mem[(i + address) % mem.Length];
            }
        }

        public void WritePage(int address, AvrMemoryType memType, byte[] data, int dataStart, int dataLength) {
            Thread.Sleep(50);
            var mem = GetMemory(memType);
            for (var i = 0; i < dataLength; i++) {
                mem[(i + address) % mem.Length] = data[i + dataStart];
            }
        }

        private byte[] GetMemory(AvrMemoryType memType) {
            switch (memType) {
                case AvrMemoryType.Flash:
                    return _flash;
                case AvrMemoryType.Eeprom:
                    return _eeprom;
                case AvrMemoryType.FuseBits:
                    return _fuses;
                case AvrMemoryType.LockBits:
                    return _locks;
                default:
                    throw new NotImplementedException();
            }
        }
        public void EraseDevice() {
        }

        public static StubProgrammer Instance = new StubProgrammer();
    }
}
