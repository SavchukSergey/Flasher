using System.Globalization;

namespace MicroFlasher {
    public struct AvrSignature {

        public AvrSignature(byte vendor, byte middle, byte low) {
            Vendor = vendor;
            Middle = middle;
            Low = low;
        }

        public byte Vendor;

        public byte Middle;

        public byte Low;

        public static AvrSignature Parse(string val) {
            var all = int.Parse(val, NumberStyles.HexNumber);
            return new AvrSignature((byte)(all >> 16), (byte)(all >> 8), (byte)all);
        }
    }
}
