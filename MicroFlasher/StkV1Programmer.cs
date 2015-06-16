using System;
using System.Threading;
using MicroFlasher.Devices;
using MicroFlasher.STKv1;

namespace MicroFlasher {
    public class StkV1Programmer : IProgrammer {

        private readonly StkV1Client _client;
        private readonly DeviceInfo _device;
        private readonly bool _useReset;
        private const int BLOCK_SIZE = 1024;

        public StkV1Programmer(StkV1Client client, DeviceInfo device, bool useReset) {
            _client = client;
            _device = device;
            _useReset = useReset;
        }

        public void Start() {
            _client.Open();
            if (_useReset) {
                _client.ResetDevice();
            } else {
                Thread.Sleep(1500);
            }
            _client.GetSyncLoop();
            _client.SetDeviceParameters(new StkV1DeviceParameters {
                DeviceCode = _device.StkCode,
                Revision = 0,
                ProgType = 0,
                ParMode = 1,
                Polling = 1,
                SelfTimed = 1,
                LockBytes = 1,
                FuseBytes = 3,
                FlashPollVal1 = 0xff,
                FlashPollVal2 = 0xff,
                EepromPollVal1 = 0xff,
                EepromPollVal2 = 0xff,
                PageSize = (ushort)_device.Flash.PageSize,
                EepromSize = (ushort)_device.Eeprom.Size,
                FlashSize = (uint)_device.Flash.Size
            });
            _client.SetDeviceParametersExt(new StkV1DeviceParametersExt {
                EepromPageSize = (byte)_device.Eeprom.PageSize,
                SignalPageL = 0xd7,
                SignalBs2 = 0xc2,
                ResetDisable = 0
            });
            _client.EnterProgramMode();
        }

        public void Stop() {
            _client.LeaveProgramMode();
            _client.Close();
        }

        public void ReadPage(int address, AvrMemoryType memType, byte[] data, int dataStart, int dataLength) {
            switch (memType) {
                case AvrMemoryType.Eeprom:
                    ReadEeprom(address, data, dataStart, dataLength);
                    break;
                case AvrMemoryType.Flash:
                    ReadFlash(address, data, dataStart, dataLength);
                    break;
                case AvrMemoryType.LockBits:
                    for (var i = 0; i < dataLength; i++) {
                        data[i + dataStart] = ReadLockByte(address + i);
                    }
                    break;
                case AvrMemoryType.FuseBits:
                    for (var i = 0; i < dataLength; i++) {
                        data[i + dataStart] = ReadFuseByte(address + i);
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public void WritePage(int address, AvrMemoryType memType, byte[] data, int dataStart, int dataLength) {
            switch (memType) {
                case AvrMemoryType.Eeprom:
                    WriteEeprom(address, data, dataStart, dataLength);
                    break;
                case AvrMemoryType.Flash:
                    WriteFlash(address, data, dataStart, dataLength);
                    break;
                case AvrMemoryType.LockBits:
                    for (var i = 0; i < dataLength; i++) {
                        WriteLockByte(address + i, data[i + dataStart]);
                    }
                    break;
                case AvrMemoryType.FuseBits:
                    for (var i = 0; i < dataLength; i++) {
                        WriteFuseByte(address + i, data[i + dataStart]);
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private byte ReadLockByte(int address) {
            switch (address) {
                case 0:
                    return _client.Universal(0x58, 0x00, 0x00, 0x00);
                default:
                    return 0;
            }
        }

        private void WriteLockByte(int address, byte val) {
            switch (address) {
                case 0:
                    _client.Universal(0xac, 0xe0, 0x00, val);
                    break;
            }
        }

        private byte ReadFuseByte(int address) {
            switch (address) {
                case 0:
                    return _client.Universal(0x50, 0x00, 0x00, 0x00);
                case 1:
                    return _client.Universal(0x58, 0x08, 0x00, 0x00);
                case 2:
                    return _client.Universal(0x50, 0x08, 0x00, 0x00);
                default:
                    return 0;
            }
        }

        private void WriteFuseByte(int address, byte val) {
            switch (address) {
                case 0:
                    _client.Universal(0xac, 0xa0, 0x00, val);
                    break;
                case 1:
                    _client.Universal(0xac, 0xa8, 0x00, val);
                    break;
                case 2:
                    _client.Universal(0xac, 0xa4, 0x00, val);
                    break;
            }
        }

        public void EraseDevice() {
            _client.Universal(0xac, 0x80, 0x00, 0x00);
        }

        private void WriteEeprom(int address, byte[] data, int dataStart, int dataLength) {
            var offset = address;
            var end = address + dataLength;
            while (offset < end) {
                _client.LoadAddress((ushort)(offset >> 1));
                var cnt = Math.Min(end - offset, BLOCK_SIZE);

                _client.ProgramPage(data, offset - address + dataStart, cnt, AvrMemoryType.Eeprom);
                offset += cnt;
            }
        }

        private void ReadEeprom(int address, byte[] data, int dataStart, int dataLength) {
            var offset = address;
            var end = address + dataLength;
            while (offset < end) {
                _client.LoadAddress((ushort)(offset >> 1));
                var cnt = Math.Min(end - offset, BLOCK_SIZE);
                _client.ReadPage(data, offset - address + dataStart, cnt, AvrMemoryType.Eeprom);
                offset += cnt;
            }
        }

        private void WriteFlash(int start, byte[] data, int dataStart, int dataLength) {
            var offset = start;
            var end = start + dataLength;
            while (offset < end) {
                _client.LoadAddress((ushort)(offset >> 1));
                var cnt = Math.Min(end - offset, BLOCK_SIZE);

                _client.ProgramPage(data, offset - start + dataStart, cnt, AvrMemoryType.Flash);
                offset += cnt;
            }
        }

        private void ReadFlash(int address, byte[] data, int dataStart, int dataLength) {
            var offset = address;
            var end = address + dataLength;
            while (offset < end) {
                _client.LoadAddress((ushort)(offset >> 1));
                var cnt = Math.Min(end - offset, BLOCK_SIZE);
                _client.ReadPage(data, offset - address + dataStart, cnt, AvrMemoryType.Flash);
                offset += cnt;
            }
        }

        public void Dispose() {
            _client.Dispose();
        }
    }
}
