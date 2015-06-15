using System.Collections.Generic;

namespace MicroFlasher.Devices {
    public class DeviceByteBits {

        public int Address { get; set; }

        public IList<DeviceBit> Bits { get; set; }

    }
}
