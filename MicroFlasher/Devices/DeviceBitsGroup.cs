using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MicroFlasher.Hex;

namespace MicroFlasher.Devices {
    public class DeviceBitsGroup {

        private readonly IList<DeviceBit> _bits = new List<DeviceBit>();

        public IList<DeviceBit> Bits {
            get { return _bits; }
        }

        public string Name { get; set; }

        public int StartAddress {
            get {
                return _bits.Count == 0 ? 0 : _bits.Min(item => item.Address);
            }
        }

        public int EndAddress {
            get {
                return _bits.Count == 0 ? 0 : _bits.Max(item => item.Address);
            }
        }

        public IList<DeviceBit> VisibleBits {
            get { return _bits.Where(item => !item.Hidden).ToList(); }
        }

        public static DeviceBitsGroup From(XElement xBits) {
            var res = new DeviceBitsGroup();
            var xName = xBits.Attribute("name");
            res.Name = xName != null ? xName.Value : "";
            foreach (var xBit in xBits.Elements()) {
                res.Bits.Add(DeviceBit.From(xBit));
            }
            return res;
        }

        public void ApplyFrom(HexBoard data) {
            var max = data.MaxAddress;
            foreach (var bit in _bits) {
                if (bit.Address <= max) {
                    var bt = data[bit.Address];
                    if (bt.HasValue) {
                        bit.SetValueFrom(bt.Value);
                    }
                }
            }
        }

        public void ApplyTo(HexBoard board) {
            foreach (var bit in _bits) {
                var val = board[bit.Address] ?? 0;
                val = bit.Apply(val);
                board[bit.Address] = val;
            }
        }
    }
}
