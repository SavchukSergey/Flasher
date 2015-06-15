namespace MicroFlasher.AvrSpi {
    public class AvrSpiProgrammer : IProgrammer {
        private readonly AvrSpiClient _client;

        public AvrSpiProgrammer(AvrSpiClient client) {
            _client = client;
        }

        public void Dispose() {
            _client.Dispose();
        }

        public void Start() {
            _client.Open();
            _client.StartProgram();
        }

        public void Stop() {
            _client.EndProgram();
            _client.Close();
        }

        public void ReadPage(int address, AvrMemoryType memType, byte[] data, int dataStart, int dataLength) {
            for (var i = 0; i < dataLength; i++) {
                switch (memType) {
                    case AvrMemoryType.Flash:
                        data[i + dataStart] = _client.ReadFlashByte((ushort)(address + i));
                        break;
                    case AvrMemoryType.Eeprom:
                        data[i + dataStart] = _client.ReadEepromMemory((ushort)(address + i));
                        break;
                    case AvrMemoryType.LockBits:
                        data[i + dataStart] = ReadLockByte(address + i);
                        break;
                    case AvrMemoryType.FuseBits:
                        data[i + dataStart] = ReadFuseByte(address + i);
                        break;
                }
            }
        }

        public void WritePage(int address, AvrMemoryType memType, byte[] data, int dataStart, int dataLength) {
            for (var i = 0; i < dataLength; i++) {
                switch (memType) {
                    case AvrMemoryType.Flash:
                        _client.LoadProgramMemoryPageByte((ushort)(address + i), data[i + dataStart]);
                        break;
                    case AvrMemoryType.Eeprom:
                        _client.WriteEepromMemory((ushort)(address + i), data[i + dataStart]);
                        break;
                    case AvrMemoryType.FuseBits:
                        WriteFuseByte(address + i, data[i + dataStart]);
                        break;
                    case AvrMemoryType.LockBits:
                        WriteLockByte(address + i, data[i + dataStart]);
                        break;
                }
            }
            if (memType == AvrMemoryType.Flash) {
                _client.WriteProgramMemoryPage((ushort)address);
            }
        }

        private byte ReadFuseByte(int address) {
            switch (address) {
                case 0:
                    return _client.ReadFuseBits();
                case 1:
                    return _client.ReadFuseHighBits();
                case 2:
                    return _client.ReadExtendedHighBits();
                default:
                    return 0;
            }
        }

        private byte ReadLockByte(int address) {
            switch (address) {
                case 0:
                    return _client.ReadLockBits();
                default:
                    return 0;
            }
        }

        private void WriteFuseByte(int address, byte val) {
            switch (address) {
                case 0:
                    _client.WriteFuseBits(val);
                    break;
                case 1:
                    _client.WriteHighFuseBits(val);
                    break;
                case 2:
                    _client.WriteExtendedFuseBits(val);
                    break;
            }
        }

        private void WriteLockByte(int address, byte val) {
            switch (address) {
                case 0:
                    _client.WriteLockBits(val);
                    break;
            }
        }

        public void EraseDevice() {
            _client.ChipErase();
        }
    }
}
