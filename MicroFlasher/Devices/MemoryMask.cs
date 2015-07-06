using System;
using System.Xml.Linq;

namespace MicroFlasher.Devices {
    public class MemoryMask {

        public AvrMemoryType MemoryType { get; set; }

        public int WordSize { get; set; }

        public int AddressStart { get; set; }

        public int? AddressEnd { get; set; }

        public uint Mask { get; set; }

        public bool Process(int address, AvrMemoryType memType, ref byte bt) {
            if (memType != MemoryType) return false;
            if (address < AddressStart) return false;
            if (AddressEnd.HasValue) {
                if (address > AddressEnd.Value) return false;
            } else {
                if (address > (AddressStart + WordSize - 1)) return false;
            }
            switch (WordSize) {
                case 1:
                    bt = (byte)(bt & Mask);
                    return true;
                case 2:
                    if ((address & 0x01) == 0) {
                        bt = (byte)(bt & Mask);
                    } else {
                        bt = (byte)(bt & Mask >> 8);
                    }
                    return true;
                default:
                    return false;
            }
        }

        public static MemoryMask From(XElement node) {
            var xWordSize = node.Attribute("wordSize");
            var xMemoryType = node.Attribute("memoryType");
            var xStart = node.Attribute("start");
            var xEnd = node.Attribute("end");
            var xMask = node.Attribute("mask");

            var memType = AvrMemoryType.Flash;
            if (xMemoryType != null) {
                Enum.TryParse(xMemoryType.Value, true, out memType);
            }

            var start = 0;
            if (xStart != null) {
                DeviceInfoUtils.TryParseInt(xStart.Value, out start);
            }

            int? end = null;
            if (xEnd != null) {
                int endVal;
                if (DeviceInfoUtils.TryParseInt(xEnd.Value, out endVal)) end = endVal;
            }

            var wordSize = 1;
            if (xWordSize != null) {
                DeviceInfoUtils.TryParseInt(xWordSize.Value, out wordSize);
            }

            var mask = 0xffffffff;
            if (xMask != null) {
                DeviceInfoUtils.TryParseUInt(xMask.Value, out mask);
            }

            var res = new MemoryMask {
                MemoryType = memType,
                AddressStart = start,
                AddressEnd = end,
                WordSize = wordSize,
                Mask = mask
            };

            return res;
        }

    }
}
