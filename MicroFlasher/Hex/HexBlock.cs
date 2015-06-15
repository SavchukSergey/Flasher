namespace MicroFlasher.Hex {
    public class HexBlock {

        public int Address { get; set; }

        public byte[] Data { get; set; }

    }

    public struct HexBlockByte {

        public int Address { get; set; }

        public byte? Byte { get; set; }

    }
}
